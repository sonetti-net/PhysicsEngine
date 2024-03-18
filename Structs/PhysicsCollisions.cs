using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class Collision
    {
        public Vector3 normal; // direction collision is taking place
        public float depth; // intersection amount

        public bool colliding = false;

        public Collision()
        {
            normal = Vector3.zero;
            colliding = false;
            depth = 0;
        }

        /// <summary>
        /// Handles collisions between multiple types of colliders. Disambiguates between the input colliders based on their colliding shapes.
        /// </summary>
        /// <param name="a">fist collider shape</param>
        /// <param name="b">second collider shape</param>
        /// <returns>Collision class which contains collision normal, and depth</returns>
        public static Collision Collide(PhysicsCollider a, PhysicsCollider b)
        {
            Collision collision = new Collision();

            if (a.colliderShape == null || b.colliderShape == null)
			{
                collision.colliding = false; 
                return collision;
            }
                
            if (a.colliderShape.shapeType == ColliderShape.ShapeType.Point)
			{
                if (b.colliderShape.shapeType == ColliderShape.ShapeType.AABB)
				{
                    collision = PointAABB((Point)a.colliderShape, (AABB)b.colliderShape); // point vs AABB
				}

                if (b.colliderShape.shapeType == ColliderShape.ShapeType.Circle)
				{
                    collision = PointCircle((Point)a.colliderShape, (Circle)b.colliderShape); // point vs circle
				}
			}

            if (a.colliderShape.shapeType == ColliderShape.ShapeType.Circle)
            {
                if (b.colliderShape.shapeType == ColliderShape.ShapeType.Circle)
                {
                    collision = CircleCollision((Circle)a.colliderShape, (Circle)b.colliderShape); // circle vs circle
                }

                if (b.colliderShape.shapeType == ColliderShape.ShapeType.AABB)
				{
                    collision = CircleAABBCollision((Circle)a.colliderShape, (AABB)b.colliderShape); // circle vs AABB
				}
            }

            if (a.colliderShape.shapeType == ColliderShape.ShapeType.AABB)
			{
                if (b.colliderShape.shapeType == ColliderShape.ShapeType.Circle)
				{
                    collision = CircleAABBCollision((AABB)a.colliderShape, (Circle)b.colliderShape); // AABB vs circle
				}

                if (b.colliderShape.shapeType == ColliderShape.ShapeType.AABB)
				{
                    collision = AABBCollision((AABB)a.colliderShape, (AABB)b.colliderShape, true); // AABB vs AABB
				}
			}
            return collision;
        }
     
        public static Collision PointAABB(Point a, AABB b)
		{
            Collision collision = new Collision();

            Debug.DrawLine(b.position, a.position, Color.magenta);

            if (a.position.x >= b.min.x && a.position.y >= b.min.y && a.position.x <= b.max.x && a.position.y <= b.max.y)
            {
                collision.colliding = true;
            }
            return collision;
		}

        public static Collision PointCircle(Point a, Circle b)
		{
            Collision collision = new Collision();

            Vector3 distance = b.position - a.position;

            if (distance.magnitude <= b.radius)
			{
                collision.colliding = true;
			}
            return collision;
		}

        public static Collision PointAABB(Vector3 a, AABB b)
        {
            Collision collision = new Collision();

            if (a.x > b.min.x && a.y > b.min.y && a.x < b.max.x && a.y < b.max.y)
			{
                collision.colliding = true;
			}

            Debug.Log(collision.colliding);
            return collision;
        }

        public static Collision CircleCollision(Circle a, Circle b)
		{
            Collision collision = new Collision();

            float distance = (b.position-a.position).magnitude;
            float radii = a.radius + b.radius;

            if (distance > radii)
			{
                collision.colliding = false;
                return collision;
			}
            
            collision.colliding = true;
            collision.normal = (b.position - a.position).normalized;
            collision.depth = radii - distance;

            return collision;
		}

        public static Collision CircleAABBCollision(Circle circle, AABB aabb)
		{
            Collision collision = new Collision();

            Vector3 closest = aabb.closestPoint(circle.position);

            Vector3 distance = closest - circle.position;

            if (distance.magnitude <= circle.radius)
            {
                collision.colliding = true;
                
                collision.depth = circle.radius - distance.magnitude;
                collision.normal = distance.normalized;
            }

            if (PhysicsEngine.detailedDebugMode == true)
            {
                Debug.DrawLine(circle.position, closest, Color.white);
                Debug.DrawLine(aabb.position, closest, Color.white);

                Debug.DrawLine(circle.position, closest, PhysicsConfig.SubLineColour);
                Debug.DrawLine(aabb.position, closest, PhysicsConfig.SubLineColour);
            }

            return collision;
		}

        public static Collision CircleAABBCollision(AABB aabb, Circle circle)
        {
            Collision collision = new Collision();

            Vector3 closest = aabb.closestPoint(circle.position);

            Vector3 distance = closest - circle.position;

            if (distance.magnitude <= circle.radius)
            {
                collision.colliding = true;

                collision.depth = circle.radius - distance.magnitude;
                collision.normal = -distance.normalized;
            }

            if (PhysicsEngine.detailedDebugMode == true)
            {
                Debug.DrawLine(circle.position, closest, Color.white);
                Debug.DrawLine(aabb.position, closest, Color.white);

                Debug.DrawLine(circle.position, closest, PhysicsConfig.SubLineColour);
                Debug.DrawLine(aabb.position, closest, PhysicsConfig.SubLineColour);
            }

            return collision;
        }

        public static Collision AABB(AABB a, AABB b)
		{
            Collision collision = new Collision();

            Vector3 delta = new Vector3(Mathf.Abs(b.position.x - a.position.x), Mathf.Abs(b.position.y - a.position.y));
            float ox = (a.width / 2f) + (b.width / 2f) - delta.x;
            float oy = (a.height / 2f) + (b.height / 2f) - delta.y;
            
            Vector3 o = new Vector3(ox, oy, 0);
            Debug.Log(o);
            Debug.DrawLine(Vector3.zero, o, Color.red);

            AABB rect = new AABB(o, new Vector3(a.width - b.width, a.height - b.height), new Vector3(a.width + b.width, a.height + b.height, 0));

            PhysicsDebug.DrawRect(rect, Color.magenta);

            return collision;
		}

        // https://www.youtube.com/watch?v=6j_yq5yRA54
        public static Collision AABBCollision(AABB a, AABB b, bool isStatic)
		{
            Collision collision = new Collision();

            collision.colliding = (a.position.x - a.width / 2f < b.position.x + b.width / 2f &&
                a.position.x + a.width / 2f > b.position.x - b.width / 2f &&
                a.position.y - a.height / 2f < b.position.y + b.height / 2f &&
                a.position.y + a.height / 2f > b.position.y - b.height / 2f);

            if (!collision.colliding)
                return collision;

            //collision = AABB(a, b);
            
            AABB minkowski = new AABB(Vector3.zero, Vector3.zero, Vector3.zero);
            Vector3 m_closest = Vector3.zero;

            if (isStatic)
			{
                minkowski = b.MinkowskiSum(a);

                m_closest = minkowski.closestPointOnBoundsToPoint(a.position);
                Debug.DrawLine(m_closest, a.position, Color.cyan);

                collision.normal = (m_closest - a.position);
                collision.depth = 1;
            }
            else
			{
                minkowski = b.MinkowskiSum(a);
                m_closest = minkowski.closestPointOnBoundsToPoint(a.position);

                Vector3 pushDir = (a.position - m_closest);

                if (pushDir.x > pushDir.y)
				{
                    pushDir.y = 0;
				}
                else
				{
                    pushDir.x = 0;
				}

                collision.normal = pushDir;

                collision.depth = collision.normal.magnitude;

                Debug.DrawLine(m_closest, a.position, Color.red);

                Debug.Log(collision.normal + " " + collision.depth);
            }

            PhysicsDebug.DrawRect(minkowski, Color.magenta);

            // Minkowski rectangle to help get intersection vector and depth
            
            return collision;
		}
    }
}

