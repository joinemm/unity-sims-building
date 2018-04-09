using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject[] floorsArray = new GameObject[2];
    int currentFloor = 0;
    Controls controls;

    Wall currentWall;
    Vector2 floorSize;
    List<GameObject> floorPlanes = new List<GameObject>();
    Dictionary<Vector3, Floor> floorDictionary = new Dictionary<Vector3, Floor>();
    Dictionary<Vector3, Wall> wallDictionary = new Dictionary<Vector3, Wall>();

    // Use this for initialization
    void Start () {
        controls = FindObjectOfType<Controls>();
	}

    public void FloorUp() {
        if (currentFloor < floorsArray.Length) {
            currentFloor += 1;
            foreach (GameObject floor in floorsArray) {
                floor.SetActive(false);
            }
            floorsArray[currentFloor - 1].SetActive(true);
        } else {
            Debug.Log("reached max floor");
        }
    }
    public void FloorDown() {
        if (currentFloor > 0) {
            currentFloor -= 1;
            foreach (GameObject floor in floorsArray) {
                floor.SetActive(false);
            }
            if (currentFloor > 0) {
                floorsArray[currentFloor - 1].SetActive(true);
            }
        } else {
            Debug.Log("reached min floor");
        }
    }
	
    public void CreateWall(Vector3 pos1) {
        GameObject newWall = Instantiate(wallPrefab);
        currentWall = newWall.GetComponent<Wall>();
        currentWall.SetPosition1(pos1);
        currentWall.SetPosition2(pos1);
        currentWall.UpdateWall();
    }

    public void ChangeWallPosition2(Vector3 newPos) {
        currentWall.SetPosition2(newPos);
        currentWall.UpdateWall();
    }

    public void FinalizeWall() {
        currentWall.UpdateWall();
        currentWall = null;
    }

    public void DragFloor(Vector3 startPos, Vector3 pos2) {
        
        //TODO: destroy floors outside the selection
        foreach (GameObject floorPlane in floorPlanes) {
            floorDictionary.Remove(floorPlane.transform.position);
            floorPlane.GetComponent<Floor>().Destroy();
            Debug.Log("doin stuf");
        }
        floorPlanes.Clear();
        //TODO: don't make floors  on top of another / make the old one disappear
        for (int x = 0; x <= Mathf.Abs(pos2.x - startPos.x); x++) {
            for (int z = 0; z <= Mathf.Abs(pos2.z - startPos.z); z++) {
                float newX;
                float newZ;
                if (pos2.x >= startPos.x) {
                    newX = startPos.x + x;
                } else {
                    newX = startPos.x - x;
                }
                if (pos2.z >= startPos.z) {
                    newZ = startPos.z + z;
                } else {
                    newZ = startPos.z - z;
                }
                Vector3 newPos = new Vector3(newX, startPos.y, newZ);
                if (!floorDictionary.ContainsKey(newPos)) {
                    Debug.Log("created new floortile");
                    CreateFloorPlane(newPos);
                }
            }
        }
    }

    public void FinalizeFloor() {
        floorPlanes.Clear();
    }

    void CreateFloorPlane(Vector3 pos) {
        GameObject newFloor = Instantiate(floorPrefab, pos, Quaternion.identity);
        newFloor.GetComponent<Floor>().SetPosition(pos);
        floorPlanes.Add(newFloor);
        floorDictionary[pos] = newFloor.GetComponent<Floor>();
    }
}
