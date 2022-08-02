using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace XRSelection {
    public abstract class SelectionModality {

        public Selection selection;
        protected Transform Hand1 { get { return this.selection.Hand1; } }
        protected Transform Hand2 { get { return this.selection.Hand2; } }
        protected Selection.SelectionBoundaryConidition BoundaryConidition { get { return this.selection.BoundaryCondition; } }
        protected string TagName { get { return this.selection.TagName; } }
        protected Type[] SelectableTypes { get { return this.selection.SelectableTypes; } }
        protected Func<GameObject[]> GameObjectFilter { get { return this.selection.GameObjectFilter; } }
        public GameObject visual;
        public Func<GameObject, Vector3[]> GetPointsToCheck = GetPointsFromGameObject;

        public abstract void Start();
        public abstract Transform[] Stop();
        public virtual void Update() { }

        public SelectionModality(Selection selection)
        {
            this.selection = selection;
        }

        public static bool PointInConcaveCollider(Vector3 point, MeshCollider col)
        {
            bool backface = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = true;
            // Raycast in all directions
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3 direction = new Vector3(x, y, z);
                        if (direction.magnitude < 0.5f) continue; // ignore (0, 0, 0) vector

                        Ray r = new Ray(point, direction);
                        RaycastHit[] hits = Physics.RaycastAll(r);

                        bool hit = false;
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].collider == col)
                            {
                                hit = true;
                            }
                        }
                        if (!hit)
                        {
                            Physics.queriesHitBackfaces = backface;
                            return false;
                        }
                    }
                }
            }

            Physics.queriesHitBackfaces = backface;
            return true;
        }

        public static bool PointInConvexMesh(Vector3 point, Mesh mesh)
        {
            for (int i = 0, j = 0; i < mesh.triangles.Length; i += 3, j += 1)
            {
                // Get Plane
                Vector3[] verts = {
                    mesh.vertices[mesh.triangles[i]],
                    mesh.vertices[mesh.triangles[i+1]],
                    mesh.vertices[mesh.triangles[i+2]]
                };
                Vector3 normal = mesh.normals[j];

                float infrontof = Vector3.Dot((point - verts[0]), normal);
                if (infrontof > 0) return false;
            }
            return true;
        }

        public static MathPlane PlaneFromControllers(Transform A, Transform B)
        {
            Vector3 up = (A.up + B.up) / 2;
            return new MathPlane(A.position, B.position, A.position + up, up);
        }

        /// <summary>
        /// Parses a GameObject to collect points used for region checking.
        /// When a selection region has been made,
        /// GameObjects need to be checked to see if they're inside that region.
        /// This function selects the points to used to check to see whether an object is in a region.
        /// 
        /// In this function, GameObject position are used for non-mesh objects
        /// and vertex positions are used for objects with meshes.
        /// 
        /// You may wish to implement your own version of this
        /// that adds LineRenderer positions or custom positions
        /// for custom objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Vector3[] GetPointsFromGameObject(GameObject obj)
        {
            MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter>();

            // For objects without meshes
            if (filters.Length == 0)
            {
                // just return position
                return new Vector3[] { obj.transform.position };
            }

            // For objects with meshes
            else
            {
                // return the positions of all the mesh vertices
                List<Vector3> points = new List<Vector3>();
                foreach (MeshFilter filter in filters)
                {
                    foreach (Vector3 vert in filter.mesh.vertices)
                    {
                        points.Add(filter.transform.TransformPoint(vert));
                    }
                }
                return points.ToArray();
            }
        }
    }
}