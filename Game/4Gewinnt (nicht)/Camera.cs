using OpenTK.Mathematics;

namespace Shaders
{
    public class Camera
    {
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        private float _pitch;
        private float _yaw = -MathHelper.PiOver2;
        private float _fov = MathHelper.PiOver2;

        public float nearPlane = 0.01f;
        public float farPlane = 100f;

        public float orthographicWidth, orthographicHeight;

        public Camera(Vector3 position, float aspectRatio)
        {
            this.position = position;
            this.aspectRatio = aspectRatio;
        }

        public Vector3 position { get; set; }

        public float aspectRatio { get; set; }

        public Vector3 front => _front;
        public Vector3 up => _up;
        public Vector3 right => _right;

        public float pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + _front, _up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, aspectRatio, nearPlane, farPlane);
        }

        public Matrix4 GetProjectionMatrixOrtho()
        {
            return Matrix4.CreateOrthographic(orthographicWidth, orthographicHeight, nearPlane, farPlane);
        }

        public Matrix4 CalculateLightSpaceMatrix()
        {

            // Create a view matrix for the light source
            Matrix4 lightViewMatrix = Matrix4.LookAt(position, position + _front, _up);

            Matrix4 lightProjectionMatrix = Matrix4.CreateOrthographic(orthographicWidth, orthographicHeight, nearPlane, farPlane);

            return lightViewMatrix * lightProjectionMatrix;
        }


        private void UpdateVectors()
        {
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}