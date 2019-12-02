using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    class Camera
    {
        public CameraType CameraType { get; private set; }
        public Matrix Transform { get; private set; }
        public Matrix World { get; private set; }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public Vector3 TopLeft { get; private set; }
        public Core.Rectangle Bounds { get; private set; }
        public float Zoom { get; private set; }
        public float Rotation { get; private set; }
        public bool ForceReset { get; private set; }

        private Vector3 cameraPosition;
        private Vector3 cameraTarget;
        private Vector3 cameraCenter;

        private float zoomOffset;

        private bool movementRestricted;
        private float minX;
        private float minY;
        private float maxX;
        private float maxY;        

        public Camera(CameraType cameraType) : this(0, 0, cameraType)
        {

        }

        public Camera(float x, float y, float minX, float minY, float maxX, float maxY) : this(x, y, CameraType.Dynamic)
        {
            movementRestricted = true;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }

        public Camera(float x, float y, CameraType cameraType)
        {
            CameraType = cameraType;
            TopLeft = new Vector3(x, y, 0);
            Initialize();
        }

        public void Reset()
        {
            Initialize();
        }

        public void SetRotation(float rotation)
        {
            if (Rotation == rotation)
                return;

            Rotation = rotation;
            UpdateMatrices();
        }

        public void SetZoomOffset(float offset)
        {
            if (zoomOffset == offset)
                return;

            zoomOffset = offset;
            UpdateMatrices();
        }

        public void SetTopLeft(float x, float y)
        {
            if (TopLeft.X == x && TopLeft.Y == y)
                return;

            TopLeft = new Vector3(x, y, 0);
            Bounds = new Core.Rectangle((int)TopLeft.X, (int)TopLeft.Y, WindowManager.PixelWidth, WindowManager.PixelHeight);
            UpdateMatrices();
        }

        public void SetMovementRestriction(float minX, float minY, float maxX, float maxY)
        {
            movementRestricted = true;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }

        private void Initialize()
        {
            Zoom = WindowManager.Scale;
            Bounds = new Core.Rectangle((int)TopLeft.X, (int)TopLeft.Y, WindowManager.PixelWidth, WindowManager.PixelHeight);
            cameraCenter = new Vector3(Bounds.Width / 2, Bounds.Height / 2, 0);
            UpdateMatrices();
        }

        private void StayWithinBounds()
        {
            if (!movementRestricted)
                return;

            if (TopLeft.X < minX)
            {
                SetTopLeft(minX, TopLeft.Y);
            }

            if (TopLeft.X + Bounds.Width > maxX)
            {
                SetTopLeft(maxX - Bounds.Width, TopLeft.Y);
            }

            if (TopLeft.Y < minY)
            {
                SetTopLeft(TopLeft.X, minY);
            }

            if (TopLeft.Y + Bounds.Height > maxY)
            {
                SetTopLeft(TopLeft.X, maxY - Bounds.Height);
            }
        }

        private void UpdateMatrices()
        {
            cameraPosition = new Vector3(-TopLeft.X, -TopLeft.Y, -1);
            cameraTarget = new Vector3(cameraPosition.X, cameraPosition.Y, 0);

            Transform =
                    // M = R * T * S 
                    // Translate the transform matrix to the inverse of the camera's center.
                    Matrix.CreateTranslation(-cameraCenter) *
                    // Rotate the camera relative to the center of the camera bounds.
                    Matrix.CreateRotationZ(Rotation) *
                    // Translate the transform matrix to the transform matrix to the inverse of the camera's top left.
                    Matrix.CreateTranslation(-TopLeft) *
                    // Scale the transform matrix by the camera's zoom.
                    Matrix.CreateScale(Zoom + zoomOffset) *
                    // Anchor the transform matrix to the center of the screen instead of the top left.
                    Matrix.CreateTranslation(new Vector3(WindowManager.WindowWidth / 2, WindowManager.WindowHeight / 2, 0));

            World =
                // Set the origin of the world matrix to the camera's center.
                Matrix.CreateTranslation(cameraCenter) *
                // Rotate the camera relative to the origin.
                Matrix.CreateRotationZ(Rotation);

            View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            Projection = Matrix.CreateOrthographic(WindowManager.WindowWidth / (Zoom + zoomOffset), WindowManager.WindowHeight / (Zoom + zoomOffset), -1000, 1000);
        }

        public virtual void Update()
        {
            StayWithinBounds();
        }
    }
}
