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

        public static void DrawDebugLines(AARectangle colliderShape)
        {
            Vector3 lh = colliderShape.GetSide(AARectangle.Side.Left);
            Vector3 rh = colliderShape.GetSide(AARectangle.Side.Right);
            Vector3 top = colliderShape.GetSide(AARectangle.Side.Top);
            Vector3 bot = colliderShape.GetSide(AARectangle.Side.Bottom);

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

