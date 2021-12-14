using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(XRSelection.Selection))]
public class SampleSelectionHandler : MonoBehaviour
{
    private XRSelection.Selection selection;
    public KeyCode startSelectionKey = KeyCode.X;

    private void Start()
    {
        this.selection = this.GetComponent<XRSelection.Selection>();
        //this.selection.OnSelection.AddListener(this.OnSelection); // either call through unity actions (can be done through GUI)

        // Only allow selection of objects of a type
        //this.selection.SelectableTypes = new System.Type[] { typeof(LineRenderer) };

        // Set Custom list of GameObjects
        //this.selection.GameObjectFilter = () => { return GameObject.FindGameObjectsWithTag("asdf"); };

        // Set Custom GameObject locations
        //this.selection.ObjectPointsFunction = (gameObject) => {
        //    LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        //    Vector3[] ret = new Vector3[lr.positionCount]; 
        //    lr.GetPositions(ret);
        //    return ret;
        //};
    }

    private void Update()
    {
        if (Input.GetKeyDown(this.startSelectionKey))
        {
            this.selection.StartSelecting();
        }

        if (Input.GetKeyUp(this.startSelectionKey))
        {
            Transform[] elements = this.selection.StopSelecting();
            //OnSelection(elements); // or call through return from StopSelecting
        }
    }

    public void OnSelection(Transform[] elements)
    {
        Color c = new Color(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1));
        foreach (var elm in elements)
        {
            Debug.Log("Selected " + elm.gameObject.name);
        }
    }
}
