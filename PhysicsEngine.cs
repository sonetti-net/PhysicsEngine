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
        [SerializeField] public Vector3 Gravity = new Vector3(0, -9.8f, 0);
        [SerializeField] float G = 6.67430e-11f;
        [SerializeField] bool objectGravity = true;


        List<PhysicsBody> physicsBodies = new List<PhysicsBody>();
        List<PhysicsCollider> physicsColliders = new List<PhysicsCollider>();

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
            Debug.Log("Num colliders: " + (physicsColliders.Count-1));
            for (int i = 0; i < physicsColliders.Count - 1; i++)
            {
                PhysicsCollider bodyA = physicsColliders[i];

                for (int j = i + 1; j < physicsColliders.Count; j++)
                {
                    
                    PhysicsCollider bodyB = physicsColliders[j];

                    // Do not do collision if both are static.
                    if (bodyA.GetComponent<PhysicsBody>().bodyType == PhysicsBody.BodyType.Static && bodyB.GetComponent<PhysicsBody>().bodyType == PhysicsBody.BodyType.Static)
                        continue;

                    if (bodyB.colliderShape != null)
					{
                        Collision collision = bodyA.Colliding(bodyB);
                    }
                }
            }
        }

        void HandleGravity()
		{
            for (int i = 0; i < physicsBodies.Count - 1; i++)
            {
                PhysicsBody bodyA = physicsBodies[i];

                for (int j = i + 1; j < physicsBodies.Count; j++)
                {
                    PhysicsBody bodyB = physicsBodies[j];

                    Vector3 direction = (bodyB.transform.position - bodyA.transform.position);
                    float d = direction.magnitude;

                    float F = (G * bodyA.mass * bodyB.mass)/(d * d);
                    Vector3 gravityForce = (direction.normalized * F);
                    bodyA.AddForce(gravityForce * Time.deltaTime);
                    bodyB.AddForce(-gravityForce * F * Time.deltaTime);

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

        public void AddNewBody(GameObject go)
		{
            physicsBodies.Add(go.GetComponent<PhysicsBody>());
            physicsColliders.Add(go.GetComponent<PhysicsCollider>());
		}

        // Start is called before the first frame update
        void Start()
        {
            physicsDebug = this.GetComponent<PhysicsDebug>();

            physicsBodies.AddRange(GameObject.FindObjectsOfType<PhysicsBody>());
            physicsColliders.AddRange(GameObject.FindObjectsOfType<PhysicsCollider>());
        }

        void NarrowPhase()
		{

		}

        void BroadPhase()
		{

		}

        // Update is called once per frame
        void Update()
        {
            UpdateVelocity();
            if (objectGravity)
            {
                HandleGravity();
            }
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
