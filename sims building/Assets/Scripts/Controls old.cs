using OutlineNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsOld: MonoBehaviour {
    public LayerMask wallCreationLayerMask;
    public GameController gameController;
    private Vector3 screenPoint;
    private Vector3 offset;
    Transform hoveredObject;
    Transform selectedObject;
    List<Transform> selectedList = new List<Transform>();
    Outline hoverOutline = null;
    Outline selectedOutline = null;

    public bool isSelecting = false;
    public RaycastHit mousePosition1;
    public RaycastHit mousePosition2;

    public enum DrawMode { WALL, FLOOR };
    public DrawMode drawMode;

    private void Update() {
        if (drawMode == DrawMode.WALL) {
            // If we press the left mouse button, save mouse location and begin selection
            if (Input.GetMouseButtonDown(0)) {
                isSelecting = true;
                if (Vector3.Distance(mousePosition1.point, MouseRay(wallCreationLayerMask).point) > 0.1f) {
                    gameController.CreateWall(mousePosition1.point);
                    Debug.Log("created wall on hold");
                }
                mousePosition1 = MouseRay(wallCreationLayerMask);
            }
            // If we let go of the left mouse button, end selection
            if (Input.GetMouseButtonUp(0) && isSelecting) {
                mousePosition2 = MouseRay(wallCreationLayerMask);
                isSelecting = false;

                
            }
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            if (hoveredObject != hit.transform) {
                if (hoveredObject != null) {
                    if (hoverOutline != null) {
                        if (hoverOutline.transform != selectedObject) {
                            Destroy(hoverOutline);
                        }
                    }
                    hoveredObject = null;
                }
                if (hit.transform.gameObject.tag == "selectable") {
                    hoveredObject = hit.transform;
                    Debug.Log("hit" + hoveredObject.name);
                    // Do something with the object that was hit by the raycast.
                    hoverOutline = hoveredObject.gameObject.AddComponent<Outline>();
                    hoverOutline.color = 2;
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && hoveredObject != null) {
            selectedObject = hoveredObject;
            RemoveOutlines();
            selectedList.Clear();
            selectedList.Add(selectedObject);
            UpdateOutlines();
        } else if (Input.GetMouseButtonDown(0) && hoveredObject == null) {
            RemoveOutlines();
            selectedList.Clear();
        }
    }

    void UpdateOutlines() {
        foreach (Transform obj in selectedList) {
            Outline outline;
            if (obj.transform.GetComponent<Outline>() == null) {
                outline = obj.gameObject.AddComponent<Outline>();
            } else {
                outline = obj.gameObject.GetComponent<Outline>();
            }
            outline.color = 1;
        }
    }

    void RemoveOutlines() {
        foreach (Transform obj in selectedList) {
            if (obj.GetComponents<Outline>() != null) {
                Outline[] outlines = obj.gameObject.GetComponents<Outline>();
                foreach (Outline outline in outlines) {
                    Destroy(outline);
                }
            }
        }
    }

    RaycastHit MouseRay(LayerMask mask) {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
        return hit;
    }
}
