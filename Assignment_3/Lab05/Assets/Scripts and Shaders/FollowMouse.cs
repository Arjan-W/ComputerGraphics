using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10f);
        float angle = AngleBetweenPoints(transform.position, mousePos);
        transform.rotation = Quaternion.Euler(newPos(transform.position, mousePos));
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("MousePos: " + mousePos);
            Debug.Log("EyePos: " + transform.position);
        }
    }

    float AngleBetweenPoints(Vector3 a, Vector3 b) {
        return (Mathf.Atan2(a.z-b.z, a.y - (b.x+2)/2) * Mathf.Rad2Deg);
    }

    Vector3 newPos(Vector3 a, Vector3 b) {
        return new Vector3((Mathf.Atan2(a.z-b.z, (a.y-2)-(b.y-2)/2) * Mathf.Rad2Deg)+90, (Mathf.Atan2(a.z-b.z, (a.x-2)-(b.x-2)/2) * Mathf.Rad2Deg)-90, 0f);
    }
}
