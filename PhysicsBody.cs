using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class PhysicsBody : MonoBehaviour
    {
        public enum ForceType
        {
            Impulse,
            Passive
        }

        public enum BodyType
        {
            Static,
            Kinetic,
            Dynamic
        }

        public BodyType bodyType = BodyType.Dynamic;

        public Vector3 linearVelocity = Vector3.zero;
        public Vector3 acceleration = Vector3.zero;
        public float rotation;
        public float rotationalVelocity;

        public float density;
        public float mass = 1f;
        public float invMass = 0f;

        public float angularVelocity;
        public float restitution;
        public float area;

        public void AddForce(Vector3 force)
        {
            if (this.bodyType != BodyType.Static)
			{
                Vector3 acceleration = force / this.mass;
                this.linearVelocity += acceleration * Time.deltaTime;
            }      
        }

        public void Move(Vector3 force)
        {
            Debug.DrawRay(this.transform.position, force, Color.magenta, 0.5f);
            force.z = 0;
            if (this.bodyType != BodyType.Static)
			{
                this.transform.position += force;
                if (this.GetComponent<PhysicsCollider>().colliderShape != null)
				{
                    this.GetComponent<PhysicsCollider>().colliderShape.transformUpdateRequired = true;
                }
            }
        }

        public void MoveTo(Vector3 position)
		{
            this.transform.position = position;
            this.GetComponent<PhysicsCollider>().colliderShape.transformUpdateRequired = true;
        }

		private void OnValidate()
		{
            invMass = (bodyType == BodyType.Static) ? 0 : 1f / mass;
		}
	}
}

