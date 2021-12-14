using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(XRSelection.Selection))]
public class SampleSelectionHandler : MonoBehaviour
{
    public XRSelection.Selection selection;
    public KeyCode startSelectionKey = KeyCode.X;

    private void Start()
    {
        this.selection = this.GetComponent<XRSelection.Selection>();
        //this.selection.OnSelection.AddListener(this.OnSelection); // either call through unity actions (can be done through GUI)
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
