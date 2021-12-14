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
        /// <summary>
        /// Only select objects with this tag
        /// </summary>
        [Tooltip("Only selects objects with this tag")]
        public string TagName = "Selectable";
        /// <summary>
        /// Only allow selection of objects of a particular type
        /// </summary>
        public System.Type[] SelectableTypes;
        /// <summary>
        /// Custom function to generate an array of selectable GameObjects
        /// </summary>
        public System.Func<GameObject[]> GameObjectFilter;


        public Transform Hand1;
        public Transform Hand2;

        public UnityEvent<Transform[]> OnSelection = new UnityEvent<Transform[]>();
        /// <summary>
        /// Optional function to customize how GameObjects are detected as inside a selected region.
        /// By default, GameObjects without meshes are 'inside' a region if their position is inside that region
        /// GameObjects with meshes are 'inisde' if all of their vertices are inside that region.
        /// 
        /// This function allows you to pass a custom set of points for each GameObject
        /// to be checked if inside the selection region.
        /// You may want to implement a custom function if, for instance,
        /// you need to select LineRenderers or other custom objects.
        /// </summary>
        public System.Func<GameObject, Vector3[]> ObjectPointsFunction = SelectionModality.GetPointsFromGameObject;

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

            if (this.GameObjectFilter is null) this.GameObjectFilter = this.GetGameObjects;
            if (!(ObjectPointsFunction is null)) mode.GetPointsToCheck = ObjectPointsFunction;
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

        public GameObject[] GetGameObjects()
        {
            if (this.TagName.Length > 0)
            {
                return GameObject.FindGameObjectsWithTag(TagName);
            }

            if (this.SelectableTypes.Length > 0)
            {
                List<GameObject> objs = new List<GameObject>();
                foreach (System.Type t in this.SelectableTypes)
                {
                    foreach (var obj in GameObject.FindObjectsOfType(t))
                    {
                        objs.Add((GameObject)obj);
                    }
                }
                return objs.ToArray();
            }

            Debug.LogWarning("No Tag or Type has been set for checking which GameObjects can be selected. No objects will be selected.\nIf you want to select all objects, pass MonoBehaviour");
            return new GameObject[0];
        }
    }
}