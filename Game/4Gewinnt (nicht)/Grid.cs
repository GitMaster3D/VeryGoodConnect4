using OpenTK.Mathematics;
using Rendering;

namespace _4Gewinnt__nicht_
{
    public static class Grid
    {
        public static Vector2 topLeft = new Vector2(-1.2f, 1.7f);
        public static Vector2 bottomRight = new Vector2(1.25f, -0.35f);
        public static GameWindow form;
        public static Renderer renderer;

        /// <summary>
        /// Calculates the position on the grid, based on vec
        /// pos = wich column on the grid
        /// </summary>

        public static Vector3 GetGridPosX(Vector3 vec, float step, out int pos)
        {
            Vector3 result = new Vector3(MathF.Round(vec.X / step) * step, vec.Y, vec.Z); // Vector Snapped to grid
            pos = (int)Mathmatics.Lerp(0, 7, Mathmatics.InverseLerp(topLeft.X, bottomRight.X, vec.X)); // Index of grid Position

            return result;
        }


        /// <summary>
        /// Calculates the Y position of vec
        /// on the grid
        /// </summary>
        public static float GetGridPosY(Vector3 vec, float step, float offset)
        {
            return MathF.Round(vec.Y / step) * step + offset;
        }


        /// <summary>
        /// Checks if vec is inside the grid 
        /// (Based on topLeft and bottomRight)
        /// </summary>
        public static bool IsInsideGrid(Vector3 pos)
        {
            return ((pos.X >= topLeft.X) && (pos.X <= bottomRight.X)) &&
            ((pos.Y <= topLeft.Y) && (pos.Y >= bottomRight.Y));
        }


        /// <summary>
        /// Converts Mouse coordinates (top Left to bottom right) to
        /// World space coordinates in openGL, Projected on the clipPlane
        /// </summary>
        public static Vector3 MouseToWorldCoordinates(int mouseX, int mouseY, float clipPlane)
        {
            // Compute the normalized device coordinates from the mouse position
            float aspectRatio = Renderer.mainCam.aspectRatio;
            float fovY = Renderer.mainCam.fov * (float)Math.PI / 180.0f;
            float tanFovY = (float)Math.Tan(fovY / 2.0f);
            float tanFovX = aspectRatio * tanFovY;
            float x = (2.0f * mouseX / form.glControl_.Width - 1.0f) * tanFovX;
            float y = (1.0f - 2.0f * mouseY / form.glControl_.Height) * tanFovY;

            // Get the camera matrix
            Matrix4 cameraMatrix = Renderer.mainCam.GetViewMatrix();

            // Extract the camera position from the matrix
            Vector3 cameraPos = Renderer.mainCam.position;

            // Compute the direction of the ray in camera space
            Vector3 rayCamera = new Vector3(x, y, -1.0f);
            rayCamera.Normalize();

            // Rotate the direction of the ray by the camera matrix
            Vector4 rayWorld4 = cameraMatrix * new Vector4(rayCamera, 0.0f);
            Vector3 rayWorld = new Vector3(rayWorld4.X, rayWorld4.Y, rayWorld4.Z);
            rayWorld.Normalize();

            // Compute the intersection point of the ray with the plane at the given clipping plane distance
            float t = (clipPlane - cameraPos.Z) / rayWorld.Z;
            Vector3 intersectionPoint = cameraPos + t * rayWorld;

            return intersectionPoint;
        }
    }
}