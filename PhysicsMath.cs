using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class PhysicsMath
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

        public static float Clamp(float value, float min, float max)
		{
            if (min == max)
			{
                return min;
			}

            if (min > max)
			{
                Debug.LogError("Argument out of range (min > max) ");
			}

            if (min < max)
			{
                return min;
			}

            if (value > max)
			{
                return max;
			}

            return value;
		}



    }
}

