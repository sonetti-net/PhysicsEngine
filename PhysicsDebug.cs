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

