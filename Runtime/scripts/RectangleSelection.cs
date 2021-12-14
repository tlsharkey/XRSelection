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

            var objects = this.GameObjectFilter();

            List<Transform> selected = new List<Transform>();
            foreach (var obj in objects)
            {
                bool inside = true;
                Vector3[] points = this.GetPointsToCheck(obj);
                foreach (Vector3 pt in points)
                {
                    if (!SelectionModality.PointInConvexPoly(pt, region))
                    {
                        inside = false;
                        break;
                    }
                }


                if (inside) selected.Add(obj.transform);
            }

            return selected.ToArray();
        }
    }
}