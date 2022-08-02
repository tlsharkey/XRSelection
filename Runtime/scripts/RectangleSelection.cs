using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace XRSelection
{
    public class RectangleSelection : SelectionModality
    {
        public Vector3 start1;
        public Vector3 start2;

        public Vector3 end1;
        public Vector3 end2;

        public LineRenderer line;

        public RectangleSelection(Selection selection): base(selection) { }

        private Vector3[] startRect;
        private Vector3[] endRect;

        public override void Start()
        {
            this.start1 = this.Hand1.position;
            this.start2 = this.Hand2.position;

            // Create Visual
            this.visual = new GameObject("Selection Visual");
            this.visual.transform.parent = this.selection.transform;
            this.line = visual.AddComponent<LineRenderer>();

            // Setup Visual
            this.line.startWidth = 0.01f;
            this.line.endWidth = 0.01f;
            this.line.positionCount = 16;
            Vector3[] rect = this.GetRect();
            this.startRect = rect;
            this.line.SetPositions(rect);
            this.line.SetPosition(4, rect[0]);
        }

        private Vector3[] GetRect()
        {
            Vector3 up = (Hand1.up + Hand2.up) / 2;
            MathPlane p = new MathPlane(Hand1.position, Hand2.position, Hand1.position + up, up);

            Vector2 tl = p.InverseTransformPoint(Hand1.position);
            Vector2 br = p.InverseTransformPoint(Hand2.position);
            Vector2 tr = new Vector2(br.x, tl.y);
            Vector2 bl = new Vector2(tl.x, br.y);

            return p.TransformPoints(new Vector2[] { tl, tr, br, bl });
        }

        public override void Update()
        {
            base.Update();

            Vector3[] rect = this.GetRect();
            this.endRect = rect;
            this.line.SetPosition(5, rect[0]);
            this.line.SetPosition(6, rect[1]);
            this.line.SetPosition(7, rect[2]);
            this.line.SetPosition(8, rect[3]);
            this.line.SetPosition(9, rect[0]);
            this.line.SetPosition(10, rect[1]);
            this.line.SetPosition(11, this.line.GetPosition(1));
            this.line.SetPosition(12, this.line.GetPosition(2));
            this.line.SetPosition(13, rect[2]);
            this.line.SetPosition(14, rect[3]);
            this.line.SetPosition(15, this.line.GetPosition(3));
        }

        public override Transform[] Stop()
        {
            this.end1 = this.Hand1.position;
            this.end2 = this.Hand2.position;

            Vector3[] region = new Vector3[] { start1, start2, end1, end2 };
            Mesh meshRegion = BoxFromQuads(startRect, endRect);
            MeshCollider col = ShowMesh(meshRegion).GetComponent<MeshCollider>();

            var objects = this.GameObjectFilter();

            List<Transform> selected = new List<Transform>();
            foreach (var obj in objects)
            {
                bool inside = true;
                Vector3[] points = this.GetPointsToCheck(obj);
                foreach (Vector3 pt in points)
                {
                    if (!SelectionModality.PointInConcaveCollider(pt, col))
                    {
                        inside = false;
                        break;
                    }
                }


                if (inside) selected.Add(obj.transform);
            }

            GameObject.Destroy(col.gameObject);
            return selected.ToArray();
        }

        // Taken from github.com/tlsharkey/LineColliders-Unity
        private static Mesh BoxFromQuads(Vector3[] a, Vector3[] b)
        {
            Mesh m = new Mesh();

            List<Vector3> positions = new List<Vector3>();
            positions.AddRange(a);
            positions.AddRange(b);
            m.vertices = positions.ToArray();

            // Triangles
            List<int> triangles = new List<int>();
            // Add front
            triangles.AddRange(new int[]
            {
            1, 3, 2,
            0, 3, 1
            });
            // Add sides
            for (int i = 0; i < a.Length; i++)
            {
                int p = (i - 1) >= 0 ? i - 1 : a.Length - 1; // previous point

                triangles.AddRange(new int[] {
                i, i + a.Length, p + a.Length,
                i, p + a.Length, p
            });
            }
            // Add back
            triangles.AddRange(new int[]
            {
            0+a.Length, 1+a.Length, 2+a.Length,
            0+a.Length, 2+a.Length, 3+a.Length
            });

            m.triangles = triangles.ToArray();
            return m;
        }
        // Taken from github.com/tlsharkey/LineColliders-Unity
        public static GameObject ShowMesh(Mesh m, Transform parent = null)
        {
            GameObject g = new GameObject();
            g.transform.SetParent(parent, false);

            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshCollider mc = g.AddComponent<MeshCollider>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();

            mf.sharedMesh = m;
            mc.sharedMesh = m;

            return g;
        }
    }
}