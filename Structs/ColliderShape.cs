using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ColliderShapes.cs
 * 
 *  Stores classes for collider shapes using a mix of polymorphism 
 */

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
        public bool transformUpdateRequired = false;

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

        public Vector3 min, max;
        public float width;
        public float height;

        public AABB(Vector3 position, Vector3 min, Vector3 max) : base(position)
		{
            this.min = min;
            this.max = max;

            width = max.x - min.x;
            height = max.y - min.y;

            base.shapeType = ShapeType.AABB;
		}

        public AABB MinkowskiSum(AABB other)
        {
            Vector3 min = new Vector3(this.min.x - other.width / 2f, this.min.y - other.height / 2f, 0);
            Vector3 max = new Vector3(this.max.x + other.width / 2f, this.max.y + other.height / 2f, 0);

            return new AABB(this.position, min, max);
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

    }
}
