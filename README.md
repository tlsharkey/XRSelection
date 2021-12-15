# XR Selection

Helpful tools for 3D selection in XR Environments

## Importing into Project
Go to the Package Manager window, press the add button, and select "Add package from Git URL..."
![image](https://user-images.githubusercontent.com/33668799/116729324-e3b13a80-a99b-11eb-9009-ade4d52a5aee.png)

Paste in this link: `https://github.com/tlsharkey/XRSelection.git`
You may get an error saying import failed. **Paste in the link a second time.**
Unity will then import the package and display a series of errors in the Console. Unity will automatically fix all of these, you can safely clear them.

## Getting Started
Once you've imported the package, 
1. add the `Selection.cs` Component to any GameObject in your Scene.
![](https://github.com/tlsharkey/XRSelection/raw/main/Resources/Selection%20Component.png)
2. Drag and drop in the point/tip/whatever of your controllers or hands into the Selection component.
3. Add your custom code to Start and Stop the selection process. This will likely be in response to Input events. You will call `GetComponent<Selection>().StartSelecting()` to start and `GetComponent<Selection>().StopSelecting()` to end your selection. `StopSelecting()` will return an array of selected `Transform`s. You code might look like:
``` C#
if (Input.GetKeyDown(KeyCode.space)) {
    GetComponent<Selection>().StartSelecting();
}
if (Input.GetKeyUp(KeyCode.space)) {
    Transform[] selectedElements = GetComponent<Selection>().StopSelecting();
    // do something with selectedElements
}
```
Alternatively, you can also use the UnityEvents to handle the selected elements. Regardless you will still need to call `StartSelecting()` and `StopSelecting()`. 
4. Optionally, you can customize which objects are checked to see if they are in the space, and how they're checked
   1. You can filter objects by Tag, Type, or by a custom function that you write.
      1. by Tag: just change the TagName property in the Inspector
      2. by Type: set `GetComponent<Selection>().SelectableTypes = new System.Type[] { typeof(LineRenderer) };` with a list of Types that you want to check
      3. by custom function: set `GetComponent<Selection>.GameObjectFilter = () => { return GameObject.FindGameObjectsWithTag("asdf"); }` to a custom function that returns an array of GameObjects.
   2. You can customize how objects are determined to be inside the selection space. Currently, if the object has a MeshFilter / children with MeshFilters, the vertices of all of the meshes will be used. If all vertices are within in the selection region, the object it selected. Alternatively, if no MeshFilter exists, the GameObject position is used.
   If you want to add more functionality beyond this (e.g. to allow for the selection of LineRenderer objects), you can set `ObjectPointsFunction` to a function that takes a `GameObject` parameter and returns a `Vector3[]` which is an array of points to check against the selection region.
   3. Checkout `SampleSelectionHandler.cs` for examples of how to use these custom functions (code pasted below)

``` C#
private XRSelection.Selection selection;
public KeyCode startSelectionKey = KeyCode.X;

private void Start()
{
    this.selection = this.GetComponent<XRSelection.Selection>();
    //this.selection.OnSelection.AddListener(this.OnSelection); // either call through unity actions (can be done through GUI)

    // Only allow selection of objects of a type
    this.selection.SelectableTypes = new System.Type[] { typeof(LineRenderer) };

    // Set Custom list of GameObjects
    // this.selection.GameObjectFilter = () => { return GameObject.FindGameObjectsWithTag("asdf"); };

    // Set Custom GameObject locations
    this.selection.ObjectPointsFunction = (gameObject) => {
       LineRenderer lr = gameObject.GetComponent<LineRenderer>();
       Vector3[] ret = new Vector3[lr.positionCount]; 
       lr.GetPositions(ret);
       return ret;
    };
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
        OnSelection(elements); // or call through return from StopSelecting
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
```

## Road Forward

Here are some of the various selection strategies/metaphors that may be implemented by this repo (unlikely I'll have time, but feel free to contribute)

![](https://github.com/tlsharkey/XRSelection/raw/main/Resources/selection%20metaphors.png)
