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

    public class AARectangle : ColliderShape
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
        public AARectangle(Vector3 position, float width, float height) :base(position)
		{
            this.width = width;
            this.height = height;
            base.shapeType = ShapeType.AABB;
		}

        public AARectangle(Transform transform) : base(transform.position)
        {
            Matrix4x4 thisT = transform.localToWorldMatrix;

            min = thisT.MultiplyPoint3x4(-0.5f * transform.localScale);
            max = thisT.MultiplyPoint3x4(0.5f * transform.localScale);
            base.shapeType = ShapeType.AABB;
        }

        public static Vector2 SetSize(float width, float height)
		{
            Vector2 result = Vector2.zero;

            return result;
        }

        public Vector3 GetSide(Side side)
		{
            Vector3 pos = this.position;

            switch (side)
			{
                case Side.Left:
                    pos.x -= this.width / 2f;
                    break;
                case Side.Right:
                    pos.x += this.width / 2f;
                    break;
                case Side.Top:
                    pos.y += this.height / 2f;
					break;
                case Side.Bottom:
                    pos.y -= this.height / 2f;
					break;
			}

            return pos;
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

    public class Collision
	{
        public Vector3 normal;
        public float intersection;
        public bool colliding = false;

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
            Collision collision = new Collision();
            
            if (this.colliderType == ColliderType.POINT)
			{
                Point point = new Point(this.transform.position, 0.2f);
			}

            if (this.colliderType == ColliderType.CIRCLE)
            {
                Circle thisCircleCollider = new Circle(this.transform.position, this.transform.localScale.x / 2f);

                if (other.colliderType == ColliderType.CIRCLE)
				{
                    Circle otherCircleCollider = new Circle(other.transform.position, other.transform.localScale.x / 2f);
                    collision = CircleCollision(thisCircleCollider, otherCircleCollider);
				}

                if (other.colliderType == ColliderType.AXIS_RECTANGLE)
				{
                    AARectangle otherRectCollider = new AARectangle(other.transform.position, other.transform.localScale.x, other.transform.localScale.y);

                    collision = CircleRectangleCollision(thisCircleCollider, otherRectCollider);
				}
            }
            
            if (this.colliderType == ColliderType.AXIS_RECTANGLE)
            {
                AARectangle thisRectCollider = new AARectangle(this.transform.position, this.transform.localScale.x, this.transform.localScale.y);

                if (other.colliderType == ColliderType.AXIS_RECTANGLE)
				{
                    AARectangle otherRectCollider = new AARectangle(other.transform.position, other.transform.localScale.x, other.transform.localScale.y);

                    collision = RectangleCollision(thisRectCollider, otherRectCollider);
				}


                if (other.colliderType == ColliderType.CIRCLE)
				{
                    Circle otherCircleCollider = new Circle(other.transform.position, other.transform.localScale.x / 2f);

                    collision = CircleRectangleCollision(otherCircleCollider, thisRectCollider, false);
				}
            }

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

        public Collision CircleCollision(Circle thisCircleCollider, Circle otherCircleCollider)
		{
            Collision collision = new Collision();

            float distance = (otherCircleCollider.position - thisCircleCollider.position).magnitude;

            collision.intersection = (thisCircleCollider.radius + otherCircleCollider.radius) - distance;
            collision.colliding = (collision.intersection > 0f);

            if (collision.colliding)
            {
                collision.normal = (otherCircleCollider.position - thisCircleCollider.position).normalized;
            }
            return collision;
		}

        public Collision RectangleCollision(AARectangle thisRectCollider, AARectangle otherRectCollider)
		{
            Collision collision = new Collision();


            collision.colliding = ((thisRectCollider.GetSide(AARectangle.Side.Right).x > otherRectCollider.GetSide(AARectangle.Side.Left).x) &&
                                    (thisRectCollider.GetSide(AARectangle.Side.Left).x < otherRectCollider.GetSide(AARectangle.Side.Right).x) &&
                                    (thisRectCollider.GetSide(AARectangle.Side.Bottom).y < otherRectCollider.GetSide(AARectangle.Side.Top).y) &&
                                    (thisRectCollider.GetSide(AARectangle.Side.Top).y > otherRectCollider.GetSide(AARectangle.Side.Bottom).y));

            if (collision.colliding)
            {
                Vector2 overlap = new Vector2(Mathf.Min(thisRectCollider.GetSide(AARectangle.Side.Right).x - otherRectCollider.GetSide(AARectangle.Side.Left).x,
                                           otherRectCollider.GetSide(AARectangle.Side.Right).x - thisRectCollider.GetSide(AARectangle.Side.Left).x),
                                           Mathf.Min(thisRectCollider.GetSide(AARectangle.Side.Top).y - otherRectCollider.GetSide(AARectangle.Side.Bottom).y,
                                           otherRectCollider.GetSide(AARectangle.Side.Top).y - thisRectCollider.GetSide(AARectangle.Side.Bottom).y));

                collision.intersection = overlap.x * overlap.y;
                collision.normal = (otherRectCollider.position - thisRectCollider.position).normalized;
            }

            if (PhysicsEngine.detailedDebugMode == true)
            {
                PhysicsDebug.DrawDebugLines(thisRectCollider);
                PhysicsDebug.DrawDebugLines(otherRectCollider);

            }
            return collision;
		}

        public Collision CircleRectangleCollision(Circle thisCircleCollider, AARectangle otherRectCollider, bool circleFirst = true)
        {

            Collision collision = new Collision();

            if (thisCircleCollider.position.x > otherRectCollider.position.x + otherRectCollider.width / 2f || thisCircleCollider.position.x < otherRectCollider.position.x - otherRectCollider.width / 2f)
            {
                collision.colliding = false;
            }
            if (thisCircleCollider.position.y > otherRectCollider.position.y + otherRectCollider.height / 2f || thisCircleCollider.position.y < otherRectCollider.position.y - otherRectCollider.height / 2f)
            {
                collision.colliding = false;
            }

            Vector3 closest = new Vector2(
                Mathf.Max(Mathf.Min(thisCircleCollider.position.x, otherRectCollider.position.x + otherRectCollider.width / 2f), otherRectCollider.position.x - otherRectCollider.width / 2f), //	x
                Mathf.Max(Mathf.Min(thisCircleCollider.position.y, otherRectCollider.position.y + otherRectCollider.height / 2f), otherRectCollider.position.y - otherRectCollider.height / 2f) //	y
                );


            Vector3 distance = closest - thisCircleCollider.position;

            if (distance.magnitude <= thisCircleCollider.radius)
            {
                collision.colliding = true;
                collision.intersection = thisCircleCollider.radius - distance.magnitude;

                if (circleFirst)
				{
                    collision.normal = (otherRectCollider.position - thisCircleCollider.position).normalized;
                }
                else
				{
                    collision.normal = (thisCircleCollider.position - otherRectCollider.position).normalized;
                }
            }


            if (PhysicsEngine.detailedDebugMode == true)
            {

                Debug.DrawLine(thisCircleCollider.position, closest, Color.white);
                Debug.DrawLine(otherRectCollider.position, closest, Color.white);

                Debug.DrawLine(thisCircleCollider.position, closest, PhysicsConfig.SubLineColour);
                Debug.DrawLine(otherRectCollider.position, closest, PhysicsConfig.SubLineColour);
            }
            return collision;
        }

        public void ResolveCollision(Collision collision, PhysicsCollider other)
		{

            this.GetComponent<PhysicsBody>().Move(-collision.normal * collision.intersection);
            other.GetComponent<PhysicsBody>().Move(collision.normal * collision.intersection);
		}

		private void OnValidate()
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
                    colliderShape = new AARectangle(this.transform.position, this.transform.localScale.x, this.transform.localScale.y);
                    break;
            }
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
