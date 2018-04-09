using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Building {

    public Vector2 position;

    public void SetPosition(Vector2 newPos) {
        this.position = newPos;
        //this.transform.position = position;
    }
}
