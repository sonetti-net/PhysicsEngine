using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class PhysicsEngine : MonoBehaviour
    {

        [Header("Components")]
        PhysicsDebug physicsDebug;

        [Header("Debug")]
        static public bool debugMode;
        static public bool detailedDebugMode;
        public bool debug;
        public bool detailedDebug;

        [Header("Engine")]
        [SerializeField] Vector3 Gravity = new Vector3(0, -9.8f, 0);

        PhysicsCollider[] physicsColliders;
        PhysicsBody[] physicsBodies;

        void UpdateVelocity()
        {
            foreach (PhysicsBody body in physicsBodies)
            {
                body.AddForce(body.acceleration);
                body.AddForce(Gravity);
            }
        }

        void UpdatePositions()
        {
            foreach (PhysicsBody body in physicsBodies)
            {
                body.gameObject.transform.position += (Vector3)body.linearVelocity * Time.deltaTime;
            }
        }

        void UpdateCollisions()
        {
            for (int i = 0; i < physicsColliders.Length - 1; i++)
            {
                PhysicsCollider bodyA = physicsColliders[i];

                for (int j = i + 1; j < physicsColliders.Length; j++)
                {
                    PhysicsCollider bodyB = physicsColliders[j];

                    Collision collision = bodyA.Colliding(bodyB);

                }
            }
        }

        void DrawDebugLines()
		{
            foreach (PhysicsCollider collider in physicsColliders)
			{
                physicsDebug.DrawDebugLines(collider.colliderShape);
			}
		}

        // Start is called before the first frame update
        void Start()
        {
            physicsDebug = this.GetComponent<PhysicsDebug>();
            physicsBodies = GameObject.FindObjectsOfType<PhysicsBody>();
            physicsColliders = GameObject.FindObjectsOfType<PhysicsCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateVelocity();
            UpdatePositions();
            UpdateCollisions();
            DrawDebugLines();
        }

		void OnValidate()
		{
            debugMode = debug;
            detailedDebugMode = detailedDebug; 
		}
	}
}
