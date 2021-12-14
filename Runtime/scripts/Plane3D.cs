using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRSelection { 
    public class Plane3D : MonoBehaviour
    {
        private MathPlane _plane;
        public MathPlane plane
        {
            get { return this._plane; }
            set {
                this._plane = value;
                this.Draw();
            }
        }

        public void Draw()
        {
            Vector3[] points = new Vector3[]
            {
                plane.TransformPoint(new Vector2(1, 1)),
                plane.TransformPoint(new Vector2(1, -1)),
                plane.TransformPoint(new Vector2(-1, -1)),
                plane.TransformPoint(new Vector2(-1, 1)),
                plane.TransformPoint(new Vector2(1, 1)),
            };

            // draw lines
            LineRenderer lr = this.gameObject.GetComponent<LineRenderer>();
            if (lr is null) lr = this.gameObject.AddComponent<LineRenderer>();
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.positionCount = 5;
            lr.useWorldSpace = true;
            lr.SetPositions(points);
        }
    }



    public struct MathPlane
    {
        public UnityEngine.Plane plane { get; private set; }
        public Vector3 worldOrigin { get; private set; }
        public Vector2 localOrigin { get { return Vector2.zero; } }
        public Vector3 position { get { return worldOrigin; } }
        public Vector3 up { get; private set; }
        public Vector3 forward { get { return plane.normal; } }
        public Vector3 backward { get { return -forward; } }
        public Vector3 down { get { return -up; } }
        public Vector3 right { get { return Vector3.Cross(up, forward).normalized; } }
        public Vector3 left { get { return -right; } }
        public float A { get { return forward.x; } }
        public float B { get { return forward.y; } }
        public float C { get { return forward.z; } }
        public float D { get { return -(A * worldOrigin.x + B * worldOrigin.y + C * worldOrigin.z); } }
        public Quaternion Rotation { get { return Quaternion.LookRotation(forward, up); } }

        /// <summary>
        /// Creates a MathPlane object for converting between 3d and 2d coordinate systems
        /// </summary>
        /// <param name="pt1">a point on the plane (will become origin)</param>
        /// <param name="pt2">another point on the plane</param>
        /// <param name="pt3">a third point on the plane</param>
        /// <param name="up">an up direction vector to define the 2d coordinate y-axis</param>
        public MathPlane(Vector3 pt1, Vector3 pt2, Vector3 pt3, Vector3 up)
        {
            this.worldOrigin = pt1;
            this.plane = new UnityEngine.Plane(pt1, pt2, pt3);
            this.up = up;
        }

        /// <summary>
        /// Creates a MathPlane object for converting between 3d and 2d coordinate systems
        /// </summary>
        /// <param name="normal">vector normal to the plane</param>
        /// <param name="origin">a point on the plane that will serve as the 2d origin</param>
        /// <param name="up">an up direction vector to define the 2d coordinate y-axis</param>
        public MathPlane(Vector3 normal, Vector3 origin, Vector3 up)
        {
            this.worldOrigin = origin;
            this.plane = new UnityEngine.Plane(normal, origin);
            this.up = up;
        }

        /// <summary>
        /// Creates a MathPlane object for converting between 3d and 2d coordinate systems
        /// </summary>
        /// <param name="normal">vector normal to the plane</param>
        /// <param name="origin">a point on the plane that will serve as the 2d origin</param>
        public MathPlane(Vector3 normal, Vector3 origin)
        {
            this.worldOrigin = origin;
            this.plane = new UnityEngine.Plane(normal, origin);
            this.up = Vector3.up;
        }

        /// <summary>
        /// Creates a MathPlane object for converting between 3d and 2d coordinate systems
        /// </summary>
        /// <param name="normal">vector normal to the plane</param>
        public MathPlane(Vector3 normal)
        {
            this.worldOrigin = Vector3.zero;
            this.plane = new UnityEngine.Plane(normal, this.worldOrigin);
            this.up = Vector3.up;
        }

        /// <summary>
        /// Transforms a point from the local plane coordinate space
        /// to the world coordinate space
        /// </summary>
        /// <param name="point2D">the point on the plane to get the world coords for</param>
        /// <returns></returns>
        public Vector3 TransformPoint(Vector2 point2D)
        {
            float x = worldOrigin.x + point2D.x * right.x + point2D.y * up.x;
            float y = worldOrigin.y + point2D.x * right.y + point2D.y * up.y;
            float z = worldOrigin.z + point2D.x * right.z + point2D.y * up.z;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Transforms a point from the 3d world coordinate space
        /// to the 2d plane coordinate space
        /// </summary>
        /// <param name="worldPoint">the point to project on the plane and get the local coords for</param>
        /// <returns></returns>
        public Vector2 InverseTransformPoint(Vector3 worldPoint)
        {
            worldPoint = plane.ClosestPointOnPlane(worldPoint);

            Vector3 relative = worldPoint - worldOrigin;

            float denom = right.x * up.y - up.x * right.y;
            float u = (relative.x * up.y - relative.y * up.x) / denom;
            float v = (relative.y * right.x - relative.x * right.y) / denom;

            return new Vector2(u, v);
        }

        public Vector3[] TransformPoints(Vector2[] points)
        {
            Vector3[] ret = new Vector3[points.Length];
            for (int i=0; i<points.Length; i++)
            {
                ret[i] = this.TransformPoint(points[i]);
            }
            return ret;
        }

        public Vector2[] InverseTransformPoints(Vector3[] points)
        {
            Vector2[] ret = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                ret[i] = this.InverseTransformPoint(points[i]);
            }
            return ret;
        }
    }
}
