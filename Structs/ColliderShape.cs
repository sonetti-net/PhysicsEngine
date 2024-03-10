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
            Collision collision = new Collision();

            float distance = (other.position - this.position).magnitude;

            collision.intersection = (this.radius + other.radius) - distance;
            collision.colliding = (collision.intersection > 0f);

            if (collision.colliding)
            {
                collision.normal = (other.position - this.position).normalized;
            }
            return collision;
        }
    }

    public class AABB : ColliderShape
    {
        public enum Side
        {
            Left,
            Right,
            Top,
            Bottom
        }
        public float width, height;
        public Vector3 min, max;
        public AABB(Vector3 position, float width, float height) : base(position)
        {
            this.width = width;
            this.height = height;
            min = new Vector3(position.x - width / 2f, position.y - height / 2f);
            max = new Vector3(position.x + width / 2f, position.y + height / 2f);

            base.shapeType = ShapeType.AABB;
        }


        public Vector3 closestPoint(Vector3 point)
        {
            Vector3 result = new Vector2(
                Mathf.Max(Mathf.Min(point.x, this.position.x + this.width / 2f), this.position.x - this.width / 2f), //	    X
                Mathf.Max(Mathf.Min(point.y, this.position.y + this.height / 2f), this.position.y - this.height / 2f)); //  Y

            //Debug.DrawLine(point, result, Color.cyan);
            return result;
        }

        public Collision CircleCollision(Circle circle)
		{
            Collision collision = new Collision();

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
                collision.intersection = circle.radius - distance.magnitude;
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

        Vector3 getDirection(Vector3 b)
		{
            Vector3 result = Vector3.zero;

            //Horizontal

            if (this.position.x < b.x) result.x = -1;
            if (this.position.x > b.x) result.x = 1;

            //Vertical
            if (this.position.y < b.y) result.y = -1;
            if (this.position.y > b.y) result.y = 1;

            return result;
		}

        public Side overlapDirection(AABB other)
		{

            Side result = Side.Left;

            if (this.position.x < other.position.x)
			{
                result = Side.Left;
			}
            if (this.position.x > other.position.x)
			{
                result = Side.Right;
			}
            if (this.position.y < other.position.y)
			{
                result = Side.Bottom;
			}
            if (this.position.y > other.position.y)
			{
                result = Side.Top;
			}
            return result;
        }

        public Vector3 getSide(Side side)
		{
            Vector3 result = this.position;

            if (side == Side.Left)
			{
                result.x -= this.width / 2f;
			}
            if (side == Side.Right)
			{
                result.x += this.width / 2f;
			}
            if (side == Side.Top)
			{
                result.y += this.height / 2f;
			}
            if (side == Side.Bottom)
			{
                result.y -= this.height / 2f;
			}

            return result;
		}
        public Collision AABBAABB(AABB other)
        {
            Collision collision = new Collision();

            Debug.DrawLine(this.position - this.getDirection(other.position), this.position, Color.black);

            collision.colliding = (this.position.x - this.width / 2f < other.position.x + other.width / 2f &&
                this.position.x + this.width / 2f > other.position.x - other.width / 2f &&
                this.position.y - this.height / 2f < other.position.y + other.height / 2f &&
                this.position.y + this.height / 2f > other.position.y - other.height / 2f);

            if (!collision.colliding)
                return collision;

            Side overlap = this.overlapDirection(other);
            
            Vector3 closestA = this.closestPoint(other.position);
            Vector3 closestB = other.closestPoint(this.position);

            
            Debug.DrawLine(other.position, closestA, Color.yellow);
            Debug.DrawLine(this.position, closestB, Color.cyan);
           
            return collision;
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
}
