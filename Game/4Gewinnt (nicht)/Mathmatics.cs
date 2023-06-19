using OpenTK.Mathematics;

namespace _4Gewinnt__nicht_
{
    public static class Mathmatics
    {
        public static float Lerp(float a, float b, float t)
        {
            return (1f - t) * a + t * b;
        }

        public static float LerpClamped(float a, float b, float t)
        {
            return Clamp(a, b, (1f - t) * a + t * b);
        }

        public static float Clamp(float min, float max, float value)
        {
            return Clamp(min, max, value, out bool s);
        }

        public static float Clamp(float min, float max, float value, out bool clamped)
        {
            clamped = false;

            if (value > max)
            {
                clamped = true;
                return max;
            }

            if (value < min)
            {
                clamped = true;
                return min;
            }

            return value;
        }

        public static float InverseLerp(float a, float b, float t)
        {
            return (t - a) / (b - a);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 resultVector = a - b;
            return MathF.Sqrt(resultVector.X * resultVector.X + resultVector.Y * resultVector.Y + resultVector.Z * resultVector.Z);
        }
    }
}