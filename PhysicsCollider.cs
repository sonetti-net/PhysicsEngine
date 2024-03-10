using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class Collision
	{
        public Vector3 normal;
        public float intersection;
        public bool colliding = false;
        public Vector3 collisionPoint;
	}

    public class PhysicsCollider : MonoBehaviour
    {
        public enum ColliderType
        {
            POINT,
            CIRCLE,
            AXIS_RECTANGLE
        }

        [Header("ColliderType")]
        public ColliderType colliderType;
        public ColliderShape colliderShape;

        public Collision Colliding(PhysicsCollider other)
		{
            Collision collision = new Collision();


            if (this.colliderType == ColliderType.AXIS_RECTANGLE)
			{

                AABB rect = this.colliderShape.GetShape<AABB>();

                if (other.colliderType == ColliderType.AXIS_RECTANGLE)
				{
                    collision = rect.AABBAABB(other.colliderShape.GetShape<AABB>());
				}
			}

            if (PhysicsEngine.debugMode)
            {
                Debug.DrawLine(this.transform.position, other.transform.position, collision.colliding == true ? PhysicsConfig.CollidingLineColour : PhysicsConfig.DefaultLineColour);
            }

            if (collision.colliding)
			{
                ResolveCollision(collision, other);
			}

            return collision;
		}

        public void ResolveCollision(Collision collision, PhysicsCollider other)
        {
            PhysicsBody bodyA = this.GetComponent<PhysicsBody>();
            PhysicsBody bodyB = other.GetComponent<PhysicsBody>();

            if (bodyA.bodyType == PhysicsBody.BodyType.Static)
			{
                bodyB.Move(-collision.normal * collision.intersection);
			}
            else if(bodyB.bodyType == PhysicsBody.BodyType.Static)
			{
                bodyA.Move(-collision.normal * collision.intersection);
			}
            else
			{
                bodyA.Move((-collision.normal * collision.intersection) / 2f);
                bodyB.Move((collision.normal * collision.intersection) / 2f);
            }

            ApplyCollisionForces(collision, bodyA, bodyB);
        }

        public void ApplyCollisionForces(Collision collision, PhysicsBody bodyA, PhysicsBody bodyB)
		{
            Vector3 relativeVelocity = bodyB.linearVelocity - bodyA.linearVelocity;

            if (Vector3.Dot(relativeVelocity, collision.normal) > 0f)
            {
                return;
            }

            float e = Mathf.Min(bodyA.restitution, bodyB.restitution);

            float j = -(1f + e) * Vector3.Dot(relativeVelocity, collision.normal);

            j /= (bodyA.invMass) + (bodyB.invMass);

            Vector3 impulse = j * collision.normal;

            bodyA.AddForce(-1 * (impulse * bodyA.invMass));
            bodyB.AddForce(1 * (impulse * bodyB.invMass));

            Debug.DrawRay(bodyA.transform.position, impulse * bodyA.invMass, Color.cyan, 0.5f);
            Debug.DrawRay(bodyB.transform.position, -impulse * bodyB.invMass, Color.cyan, 0.5f);

        }

        public Collision PointCollision(Point thisPointCollider, Point otherPointCollider)
		{
            Collision collision = new Collision();

            float distance = (otherPointCollider.position - thisPointCollider.position).magnitude;

            if (distance < thisPointCollider.collisionThreshold)
			{
                collision.colliding = true;
                collision.intersection = distance;
			}
            return collision;
		}


		void Start()
		{
            switch (colliderType)
            {
                case ColliderType.POINT:
                    colliderShape = new Point(this.transform.position, 0.2f);
                    break;

                case ColliderType.CIRCLE:
                    colliderShape = new Circle(this.transform.position, this.transform.localScale.x / 2f);
                    break;

                case ColliderType.AXIS_RECTANGLE:
                    colliderShape = new AABB(this.transform.position, this.transform.localScale.x, this.transform.localScale.y);
                    break;
            }
        }

		void Update()
		{
			colliderShape.position = transform.position;
            colliderShape.GetShape<AABB>().width = transform.localScale.x;
            colliderShape.GetShape<AABB>().height = transform.localScale.y;

        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            if (colliderType == PhysicsCollider.ColliderType.AXIS_RECTANGLE)
            {
                Gizmos.DrawWireCube(this.transform.position, new Vector3(this.transform.localScale.x, this.transform.localScale.y, 0));
            }

            if (colliderType == PhysicsCollider.ColliderType.CIRCLE)
            {
                GizmoTools.DrawCircleGizmo(this.transform.position, this.transform.rotation, this.transform.localScale.x / 2, Color.green);
            }

            if (colliderType == PhysicsCollider.ColliderType.POINT)
            {
                GizmoTools.DrawCircleGizmo(this.transform.position, this.transform.rotation, this.transform.localScale.x / 2, Color.green);
            }

        }
    }
}
