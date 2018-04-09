using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using OutlineNamespace;

public class Controls : MonoBehaviour {

    public enum DrawMode { WALL, FLOOR, DELETE, NONE };
    public DrawMode drawMode;

    public LayerMask wallCreationLayerMask;
    public LayerMask deleteMask;
    public GameObject selector;
    public Mesh planeMesh;
    public Mesh selectorMesh;
    GameController gameController;
    UIController UIcontroller;

    bool mouseDown = false;
    bool dragging = false;
    bool lockToGrid = true;

    Vector3 mouseWorldPos1;
    Vector3 mouseWorldPos2;
    Vector3 mousePosLastDrag;

    Transform hoveredTransform;

    // Use this for initialization
    void Start() {
        gameController = FindObjectOfType<GameController>();
        UIcontroller = FindObjectOfType<UIController>();
        DrawModeDelete();
    }

    // Update is called once per frame
    void Update() {

        //get mousePosition this frame
        Vector3 mousePosThisFrame = GetMouseWorldPos(wallCreationLayerMask);

        //toggle drawmode
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            DrawModeWall();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            DrawModeFloor();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            DrawModeDelete();
        }

        // TODO: // build height changes
        if (Input.GetKeyDown(KeyCode.Y)) {
            //height UP
            gameController.FloorUp();
        } else if (Input.GetKeyDown(KeyCode.H)) {
            //height DOWN
            gameController.FloorDown();
        }

        //drawmode == WALL

        //TODO: if wall length == 0 don't build
        if (drawMode == DrawMode.WALL) {
            if (Input.GetMouseButtonDown(0)) {
                mouseDown = true;
                mouseWorldPos1 = mousePosThisFrame;
            }
            if (Input.GetMouseButtonUp(0)) {
                mouseDown = false;
                gameController.FinalizeWall();
                dragging = false;
            }
            if (mouseDown) {
                mouseWorldPos2 = mousePosThisFrame;
                if (dragging) {
                    //change wall position 2
                    gameController.ChangeWallPosition2(mouseWorldPos2);
                } else {
                    //create wall
                    gameController.CreateWall(mouseWorldPos1);
                    dragging = true;
                }
            } else {
                //mouse is hovering
                mouseWorldPos1 = mousePosThisFrame;
            }
        }

        //drawmode == FLOOR
        else if (drawMode == DrawMode.FLOOR) {
            if (Input.GetMouseButtonDown(0)) {
                mouseDown = true;
                mouseWorldPos1 = mousePosThisFrame;
            }
            if (Input.GetMouseButtonUp(0)) {
                mouseDown = false;
                gameController.FinalizeFloor();
                dragging = false;
            }
            if (mouseDown) {
                mouseWorldPos2 = mousePosThisFrame;
                if (Vector3.Distance(mouseWorldPos2, mousePosLastDrag) > 0.1f) {
                    if (dragging) {
                        //change floor position 2
                        mousePosLastDrag = mousePosThisFrame;
                        gameController.DragFloor(mouseWorldPos1, mouseWorldPos2);
                    } else {
                        dragging = true;
                    }
                }
            } else {
                //mouse is hovering
                mouseWorldPos1 = mousePosThisFrame;
            }
        }

        //drawmode == DELETE
        else if (drawMode == DrawMode.DELETE) {
            Transform mouseOverTransformThisFrame = GetMouseOverTransform(deleteMask);
            if (mouseOverTransformThisFrame != null) {
                if (hoveredTransform != mouseOverTransformThisFrame) {
                    if (hoveredTransform != null) {
                        if (hoveredTransform.GetComponent<Outline>() != null) {
                            if (hoveredTransform != mouseOverTransformThisFrame) {
                                Destroy(hoveredTransform.GetComponent<Outline>());
                            }
                        }
                        hoveredTransform = null;
                    } else if (mouseOverTransformThisFrame.gameObject.tag == "selectable") {
                        hoveredTransform = mouseOverTransformThisFrame;
                        hoveredTransform.gameObject.AddComponent<Outline>();
                    }
                }
                if (Input.GetMouseButtonDown(0)) {
                    Debug.Log(mouseOverTransformThisFrame.name);
                    mouseOverTransformThisFrame.parent.GetComponent<Building>().Destroy();
                }
            } else if (hoveredTransform != null) {
                Destroy(hoveredTransform.GetComponent<Outline>());
                hoveredTransform = null;
            }
        }

        selector.transform.position = mousePosThisFrame;
    }

    void DrawModeWall() {
        drawMode = DrawMode.WALL;
        UIcontroller.UpdateIcon();
        selector.transform.GetChild(0).GetComponent<MeshFilter>().mesh = selectorMesh;
        if (hoveredTransform != null) {
            Destroy(hoveredTransform.GetComponent<Outline>());
            hoveredTransform = null;
        }
    }

    void DrawModeFloor() {
        drawMode = DrawMode.FLOOR;
        UIcontroller.UpdateIcon();
        selector.transform.GetChild(0).GetComponent<MeshFilter>().mesh = planeMesh;
        if (hoveredTransform != null) {
            Destroy(hoveredTransform.GetComponent<Outline>());
            hoveredTransform = null;
        }
    }

    void DrawModeDelete() {
        drawMode = DrawMode.DELETE;
        UIcontroller.UpdateIcon();
        selector.transform.GetChild(0).GetComponent<MeshFilter>().mesh = null;
        if (hoveredTransform != null) {
            Destroy(hoveredTransform.GetComponent<Outline>());
            hoveredTransform = null;
        }
    }

    Vector3 GetMouseWorldPos(LayerMask mask) {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
            if (lockToGrid) {
                if (drawMode == DrawMode.WALL) {
                    return new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
                } else {
                    float newX = Mathf.Round(hit.point.x);
                    float newZ = Mathf.Round(hit.point.z);
                    if (newX >= hit.point.x) {
                        newX -= .5f;
                    } else if (newX < hit.point.x) {
                        newX += .5f;
                    }
                    if (newZ >= hit.point.z) {
                        newZ -= .5f;
                    } else if (newZ < hit.point.z) {
                        newZ += .5f;
                    }
                    return new Vector3(newX, Mathf.Round(hit.point.y), newZ);
                }
            } else {
                return hit.point;
            }
        } else {
            //Debug.Log("!! MouseWorldPos returned null !!");
            if (mouseDown) {
                return mouseWorldPos2;
            } else {
                return mouseWorldPos1;
            }
        }
    }

    Transform GetMouseOverTransform(LayerMask mask) {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
            return hit.transform;
        } else {
            return null;
        }
    }
}
