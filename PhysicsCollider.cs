using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
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
        public bool isStatic = false;
        public bool isTrigger = false;


        public Collision Colliding(PhysicsCollider other)
		{
            Collision collision = Collision.Collide(this, other);

            if (PhysicsEngine.debugMode)
            {
                Debug.DrawLine(this.transform.position, other.transform.position, collision.colliding == true ? PhysicsConfig.CollidingLineColour : PhysicsConfig.DefaultLineColour);
            }

            if (collision.colliding)
			{
                if (!isTrigger)
				{
                    ResolveCollision(collision, other);
                }
			}

            return collision;
		}

        public void ResolveCollision(Collision collision, PhysicsCollider other)
        {
            PhysicsBody bodyA = this.GetComponent<PhysicsBody>();
            PhysicsBody bodyB = other.GetComponent<PhysicsBody>();

            if (bodyA.bodyType == PhysicsBody.BodyType.Static)
			{
                bodyB.Move((collision.normal * collision.depth));
            }
            if (bodyB.bodyType == PhysicsBody.BodyType.Static)
            {
                bodyA.Move((collision.normal * collision.depth));
            }
            else
			{
                bodyA.Move((-collision.normal * collision.depth / 2f));
                bodyB.Move((collision.normal * collision.depth / 2f));
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

            if (this.GetComponent<PhysicsBody>())
			{
                if (this.GetComponent<PhysicsBody>().bodyType == PhysicsBody.BodyType.Static)
                {
                    this.isStatic = true;
                }
            }

            this.colliderShape.position = this.transform.position;
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
