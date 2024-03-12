using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class ColliderShape
    {
        public enum ShapeType
        {
            Point,
            Circle,
            Polygon,
            AABB,
            Rectangle
        }

        public Vector3 position;
        public ShapeType shapeType;

        public ColliderShape(Vector3 position)
        {
            this.position = position;
        }

        public T GetShape<T>() where T : ColliderShape
		{
            return (T) this;
		}

    }

    public class Point : ColliderShape
    {
        public float collisionThreshold;
        public Point(Vector3 position, float threshold) : base(position)
        {
            this.collisionThreshold = threshold;
            base.shapeType = ShapeType.Point;
        }
    }


    public class Circle : ColliderShape
    {
        public float radius;
        public Circle(Vector3 position, float radius) : base(position)
        {
            this.radius = radius;
            base.shapeType = ShapeType.Circle;
        }

        public Collision circleCollision(Circle other)
		{
            Collision collision = null;

            float distance = (other.position - this.position).magnitude;

            //collision.intersection = (this.radius + other.radius) - distance;
            //collision.colliding = (collision.intersection > 0f);

            if (collision.colliding)
            {
                collision.normal = (other.position - this.position).normalized;
            }
            return collision;
        }
    }



    public class AABB : ColliderShape
    {

        public class Edge
		{
            public float magnitude;
            public Side side;

            public Edge(float magnitude, Side side)
			{
				this.magnitude = magnitude;
				this.side = side;
			}

            public static Edge closest(Edge[] edges)
			{
                int n = edges.Length;

                for (int i = 0; i < n - 1; i++)
				{
                    for (int j = 0; j < n - i - 1; j++)
					{
                        if (edges[j].magnitude > edges[j+1].magnitude)
						{
                            Edge temp = edges[j];
                            edges[j] = edges[j+1];
                            edges[j+1] = temp;
						}  
					}
				}
                return edges[0];
			}
		}

        public enum Side
        {
            Left,
            Right,
            Top,
            Bottom
        }

        public Vector3 min, max;
        public float width;
        public float height;


        public AABB(Vector3 position, Vector3 min, Vector3 max) : base(position)
		{
            this.min = min;
            this.max = max;

            width = max.x - min.x;
            height = max.y - min.y;
		}

        public Vector3 left()
		{
            return new Vector3(this.position.x - this.width / 2f, this.position.y, 0);
		}
        public Vector3 right()
        {
            return new Vector3(this.position.x + this.width / 2f, this.position.y, 0);
        }
        public Vector3 top()
        {
            return new Vector3(this.position.x, this.position.y + this.height/2f, 0);
        }
        public Vector3 bottom()
        {
            return new Vector3(this.position.x, this.position.y - this.height/2f, 0);
        }

        public AABB MinkowskiSum(AABB other)
        {
            Vector3 min = new Vector3(this.min.x - other.width/2f, this.min.y - other.height/2f, 0);
            Vector3 max = new Vector3(this.max.x + other.width/2f, this.max.y + other.height/2f, 0);

            return new AABB(this.position, min, max);
        }

        public Vector3 closestPoint(Vector3 point)
        {
            Vector3 result = new Vector2(
                Mathf.Max(Mathf.Min(point.x, this.position.x + this.width / 2f), this.position.x - this.width / 2f), //	    X
                Mathf.Max(Mathf.Min(point.y, this.position.y + this.height / 2f), this.position.y - this.height / 2f)); //  Y

            Debug.DrawLine(point, result, Color.cyan);
            return result;
        }

        public Vector3 closestPointOnBoundsToPoint(Vector3 point)
		{
            float minDist = Mathf.Abs(point.x - this.min.x);
            Vector3 boundsPoint = new Vector3(this.min.x, point.y);

            if (Mathf.Abs(max.x - point.x) < minDist)
            {
                minDist = Mathf.Abs(max.x - point.x);
                boundsPoint = new Vector3(max.x, point.y);
            }
            if (Mathf.Abs(max.y - point.y) < minDist)
            {
                minDist = Mathf.Abs(max.y - point.y);
                boundsPoint = new Vector3(point.x, max.y);
            }
            if (Mathf.Abs(min.y - point.y) < minDist)
            {
                minDist = Mathf.Abs(min.y - point.y);
                boundsPoint = new Vector3(point.x, min.y);
            }

            Debug.DrawLine(point, boundsPoint, Color.green);



            return boundsPoint;
		}

        public Collision CircleCollision(Circle circle)
		{
            Collision collision = null;

            if (circle.position.x > this.position.x + this.width / 2f || circle.position.x < this.position.x - this.width / 2f)
            {
                collision.colliding = false;
            }
            if (circle.position.y > this.position.y + this.height / 2f || circle.position.y < this.position.y - this.height / 2f)
            {
                collision.colliding = false;
            }

            Vector3 closest = this.closestPoint(circle.position);

            Vector3 distance = circle.position - closest;

            if (distance.magnitude <= circle.radius)
            {
                collision.colliding = true;
                //collision.intersection = circle.radius - distance.magnitude;
                collision.collisionPoint = closest;

                collision.normal = distance.normalized;
            }

            if (PhysicsEngine.detailedDebugMode == true)
            {
                Debug.DrawLine(circle.position, closest, Color.white);
                Debug.DrawLine(this.position, closest, Color.white);

                Debug.DrawLine(circle.position, closest, PhysicsConfig.SubLineColour);
                Debug.DrawLine(this.position, closest, PhysicsConfig.SubLineColour);
            }
            return collision;
        }
        
        public Collision AABBAABB(AABB other)
        {
            Collision collision = new Collision();

            collision.colliding = (this.position.x - this.width / 2f < other.position.x + other.width / 2f &&
                this.position.x + this.width / 2f > other.position.x - other.width / 2f &&
                this.position.y - this.height / 2f < other.position.y + other.height / 2f &&
                this.position.y + this.height / 2f > other.position.y - other.height / 2f);
            
            if (!collision.colliding)
                return collision;

            // Minkowski rectangle to help get intersection vector and depth

            AABB minkowski = this.MinkowskiSum(other);

            

            Vector3 m_closest = minkowski.closestPointOnBoundsToPoint(other.position) - other.position;

            Debug.DrawLine(other.position, m_closest, Color.magenta);
            PhysicsDebug.DrawRect(minkowski, Color.magenta);
            collision.normal = m_closest.normalized;
            collision.intersection = m_closest;
            //Debug.Log(m_closest);
            return collision;
        }
    }


}
