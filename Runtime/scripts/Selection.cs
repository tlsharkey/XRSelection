using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace XRSelection {
    public class Selection : MonoBehaviour {
        public enum SelectionType {
            Click, // Click on the individual objects
            Projection, // Draw a form and project it to select
            Rectangle, // Draws a rect between hands/controllers, as the controllers move, the area is used for selection
            Fishing, // A metaphor for a net that is thrown over the objects to select
            Screen, // Selects the objects that collide with a screen moved by the user
            Form, // User draws a form that is used for selecting objects
        }
        public enum SelectionBoundaryConidition {
            Contains, // entire object is contained in the selection
            // Intersects, // the object intersects with the selection boundary
            // Overlaps, // any part of the object overlaps with the selection boundary
        }

        public SelectionType selectionType = SelectionType.Rectangle;
        public SelectionBoundaryConidition BoundaryCondition = SelectionBoundaryConidition.Contains;
        [Tooltip("Only selects objects with this tag")]
        public string TagName = "Selectable";
        
        public Transform Hand1;
        public Transform Hand2;

        public UnityEvent<Transform[]> OnSelection = new UnityEvent<Transform[]>();

        private SelectionModality mode;

        public void StartSelecting() {
            if (this.mode is null)
            {
                switch (this.selectionType)
                {
                    case SelectionType.Rectangle:
                        this.mode = new RectangleSelection(this);
                        break;
                    case SelectionType.Click:
                        this.mode = new ClickSelection(this);
                        break;
                    case SelectionType.Projection:
                        this.mode = new ProjectionSelection(this);
                        break;
                    case SelectionType.Fishing:
                        this.mode = new FishingSelection(this);
                        break;
                    case SelectionType.Screen:
                        this.mode = new ScreenSelection(this);
                        break;
                    case SelectionType.Form:
                        this.mode = new FormSelection(this);
                        break;
                }
            }
            mode.Start();
        }

        private void Update()
        {
            if (!(this.mode is null)) this.mode.Update();
        }

        public Transform[] StopSelecting() {
            Transform[] selected = mode.Stop();
            this.OnSelection.Invoke(selected);
            Destroy(mode.visual);
            this.mode = null;
            return selected;
        }
    }
}