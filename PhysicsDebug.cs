using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class PhysicsDebug : MonoBehaviour
    {
        public enum Axis
        {
            x,
            y,
            z
        }

        public enum MinMax
        {
            min,
            max
        }

        [SerializeField] float lineLength = 1f;
        static float _lineLength;
        public void DrawDebugLines(ColliderShape colliderShape)
		{

		}

        public static void DrawDebugLines(AABB colliderShape)
        {
            Vector3 lh = new Vector3(colliderShape.min.x, colliderShape.position.y, 0);
            Vector3 rh = new Vector3(colliderShape.max.x, colliderShape.position.y, 0);
            Vector3 top = new Vector3(colliderShape.position.x, colliderShape.max.y, 0);
            Vector3 bot = new Vector3(colliderShape.position.x, colliderShape.min.y, 0);

            Debug.DrawLine(lh, PhysicsDebug.AxisIntersection(lh, PhysicsDebug.Axis.y, PhysicsDebug.MinMax.max), PhysicsConfig.SubLineColour);
            Debug.DrawLine(lh, PhysicsDebug.AxisIntersection(lh, PhysicsDebug.Axis.y, PhysicsDebug.MinMax.min), PhysicsConfig.SubLineColour);

            Debug.DrawLine(rh, PhysicsDebug.AxisIntersection(rh, PhysicsDebug.Axis.y, PhysicsDebug.MinMax.max), PhysicsConfig.SubLineColour);
            Debug.DrawLine(rh, PhysicsDebug.AxisIntersection(rh, PhysicsDebug.Axis.y, PhysicsDebug.MinMax.min), PhysicsConfig.SubLineColour);

            Debug.DrawLine(bot, PhysicsDebug.AxisIntersection(bot, PhysicsDebug.Axis.x, PhysicsDebug.MinMax.max), PhysicsConfig.SubLineColour);
            Debug.DrawLine(bot, PhysicsDebug.AxisIntersection(bot, PhysicsDebug.Axis.x, PhysicsDebug.MinMax.min), PhysicsConfig.SubLineColour);

            Debug.DrawLine(top, PhysicsDebug.AxisIntersection(top, PhysicsDebug.Axis.x, PhysicsDebug.MinMax.max), PhysicsConfig.SubLineColour);
            Debug.DrawLine(top, PhysicsDebug.AxisIntersection(top, PhysicsDebug.Axis.x, PhysicsDebug.MinMax.min), PhysicsConfig.SubLineColour);
        }

        public static void DrawPoint(Vector3 point, float radius, Color col)
		{
            Vector2[] points = new Vector2[2];
			points[0] = point;
            points[1] = (point - point) * radius;

            Debug.DrawLine(points[0], points[1], col);

        }

        public static void DrawRect(AABB rect, Color col)
		{
            Vector2[] points = new Vector2[4];
            points[0] = rect.max;
            points[1] = new Vector2(rect.min.x, rect.max.y);
            points[2] = new Vector2(rect.min.x, rect.min.y);
			points[3] = new Vector2(rect.max.x, rect.min.y);


            for (int i = 0; i < points.Length -1; i++)
			{
                int next = (i + 1 > points.Length) ? 0 : i + 1;
                Debug.DrawLine(points[i], points[next], col);
			}
            Debug.DrawLine(points[3], points[0], col);
        }

        float GetLineLength()
		{
            return lineLength;
		}

        public static Vector3 AxisIntersection(Vector3 vector, Axis axis, MinMax minmax)
        {
            Vector3 result = vector;

            float offset = (minmax == MinMax.max) ? _lineLength : -_lineLength;

            if (axis == Axis.x)
            {
                result.x += offset;
            }

            if (axis == Axis.y)
            {
                result.y += offset;
            }

            if (axis == Axis.z)
            {
                result.z += offset;
            }

            return result;
        }


		private void Update()
		{
            _lineLength = lineLength;
		}
	}
}

