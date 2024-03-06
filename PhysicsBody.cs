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


        public BodyType bodyType;

        public Vector3 position;
        public Vector3 linearVelocity = Vector3.zero;
        public Vector3 acceleration = Vector3.zero;
        public float rotation;
        public float rotationalVelocity;

        public float density;
        public float mass = 1f;
        public float angularVelocity;
        public float restitution;
        public float area;
        

        public void AddForce(Vector3 force)
        {
            this.linearVelocity += force * Time.deltaTime;
        }

        public void Move(Vector3 force)
        {
            this.transform.position += force;
        }
	}
}

