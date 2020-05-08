using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : MonoBehaviour 
{
    public Vector3 focus_point = new Vector3(0, 0, 0);
    public Vector3 offset = new Vector3(0, 0, 0);
    public Vector3 amplitude = new Vector3(150, 50, 150);
    public Vector3 speed = new Vector3(10, 0, 10);
    private float zoom_speed = 0.1f;

    void Start() {
        if (speed.x != 0) { speed.x = 1 / speed.x; }
        if (speed.y != 0) { speed.y = 1 / speed.y; }
        if (speed.z != 0) { speed.z = 1 / speed.z; }
        Vector3 cam_pos = get_pos();
        gameObject.transform.position = get_pos();
        gameObject.transform.rotation = get_angles(focus_point, cam_pos);
    }

    void Update() {
        // Always change the amplitude if scrolled
        amplitude -= amplitude * zoom_speed * Input.mouseScrollDelta[1];  

        Vector3 cam_pos = get_pos();
        // Update position and rotation
        gameObject.transform.position = cam_pos;
        gameObject.transform.rotation = get_angles(focus_point, cam_pos);
    }

    private Vector3 get_pos() {
        Vector3 time = Time.time * speed * 2 * Mathf.PI;
        Vector3 pos = new Vector3(Mathf.Sin(time.x), Mathf.Sin(time.y), Mathf.Cos(time.z));
        pos = Vector3.Scale(pos, amplitude);
        return focus_point + pos + offset;
    }

    private Quaternion get_angles(Vector3 cd, Vector3 co) {
        Vector3 d = cd - co;
        return Quaternion.LookRotation(d.normalized);
    }
}
