using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
    
    void Update() {
        // added 0.5 inorder to made line renderer visible
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.5f);
    }
}
