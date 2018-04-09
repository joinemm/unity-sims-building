using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building{

    private Vector3 position1;
    private Vector3 position2;
    private float distance;

    public void SetPosition1(Vector3 pos1) {
        this.position1 = pos1;
        this.transform.position = position1;
    }

    public void SetPosition2(Vector3 pos2) {
        this.position2 = pos2;
    }

    public void UpdateWall() {        
        this.distance = Vector3.Distance(position1, position2);
        this.transform.localScale = new Vector3(1,1,distance);
        this.transform.LookAt(position2);
        this.transform.GetChild(0).GetComponent<Renderer>().material.mainTextureScale = new Vector2(distance/2, 1);
    }
}
