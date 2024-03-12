using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class Collision
	{
        public Vector3 normal;
        public Vector2 intersection;
        public bool colliding = false;
        public Vector3 collisionPoint;

        public Collision ()
        {
            normal = Vector3.zero;
            collisionPoint = Vector3.zero;
            colliding = false;
            intersection = Vector2.zero;
        }

        public static Collision Collide(PhysicsCollider a, PhysicsCollider b)
		{
            Collision collision = new Collision();

            if (a.colliderType == PhysicsCollider.ColliderType.POINT)
            {
                if (b.colliderType == PhysicsCollider.ColliderType.POINT)
                {
                    collision = Collide((Point)a.colliderShape, (Point)b.colliderShape);
                }

                if (b.colliderType == PhysicsCollider.ColliderType.CIRCLE)
                {
                    collision = Collide((Circle)a.colliderShape, (Point)b.colliderShape);
                }

                if (b.colliderType == PhysicsCollider.ColliderType.AXIS_RECTANGLE)
                {
                    collision = Collide((Point)a.colliderShape, (AABB)b.colliderShape);
                }
            }

            if (a.colliderType == PhysicsCollider.ColliderType.CIRCLE)
            {
                if (b.colliderType == PhysicsCollider.ColliderType.POINT)
                {
                    collision = Collide((Circle)a.colliderShape, (Point)b.colliderShape);
                }

                if (b.colliderType == PhysicsCollider.ColliderType.CIRCLE)
                {
                    collision = Collide((Circle)a.colliderShape, (Circle)b.colliderShape);
                }

                if (b.colliderType == PhysicsCollider.ColliderType.AXIS_RECTANGLE)
                {
                    collision = Collide((Circle)a.colliderShape, (AABB)b.colliderShape);
                }
            }

            if (a.colliderType == PhysicsCollider.ColliderType.AXIS_RECTANGLE)
            {
                if (b.colliderType == PhysicsCollider.ColliderType.POINT)
                {
                    collision = Collide((Point)a.colliderShape, (AABB)b.colliderShape);
                }

                if (b.colliderType == PhysicsCollider.ColliderType.CIRCLE)
                {
                    collision = Collide((Circle)a.colliderShape, (AABB)b.colliderShape);
                }

                if (b.colliderType == PhysicsCollider.ColliderType.AXIS_RECTANGLE)
                {
                    collision = Collide((AABB)a.colliderShape, (AABB)b.colliderShape);
                }
            }
            return collision;
        }
        public static Collision Collide(Point a, Point b)
		{
            Debug.Log("Point Point");
            Collision collision = null; // to do
            return collision;
        }

        public static Collision Collide(Point a, AABB b)
		{
            Debug.Log("Point AABB");
            Collision collision = null; // to do
            return collision;
        }

        public static Collision Collide(Circle a, Circle b)
		{
            Debug.Log("Circle Circle");
            Collision collision = a.circleCollision(b);
            return collision;
        }

        public static Collision Collide(Circle a, Point b)
		{
            Debug.Log("Circle Point");
            Collision collision = null; // to do
            return collision;
        }

        public static Collision Collide(Circle a, AABB b)
		{
            Debug.Log("Circle AABB");
            Collision collision = b.CircleCollision(a);
            return collision;
        }

        public static Collision Collide(AABB a, AABB b)
		{
            Collision collision = a.AABBAABB(b);
            return collision;
		}
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
            Collision collision = Collision.Collide(this, other);

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
                bodyB.Move(collision.intersection);
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

        /*
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
        */

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
                    colliderShape = new AABB(this.transform.position, new Vector3(this.transform.position.x - this.transform.localScale.x/2f, this.transform.position.y - this.transform.localScale.y/2f), new Vector3(this.transform.position.x + this.transform.localScale.x/2f, this.transform.position.y + this.transform.localScale.y/2f));
                    break;
            }
        }

		void Update()
		{
			colliderShape.position = transform.position;
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
