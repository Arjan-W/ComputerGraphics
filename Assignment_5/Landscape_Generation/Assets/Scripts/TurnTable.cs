using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : MonoBehaviour
{
    public float Radius;
    public float Speed;

    private float rot = 0;
    // Start is called before the first frame update
    void Start()
    {

        gameObject.transform.position = new Vector3(0f, 60f, Radius);
        gameObject.transform.Rotate(new Vector3(20f, 180f, 0f), Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        float step = Mathf.Sin(Time.deltaTime * Speed);
        rot += step;
        rot = rot % 360;

        float x = Mathf.Sin((rot * Mathf.PI) / 180) * Radius;
        float z = Mathf.Cos((rot * Mathf.PI) / 180) * Radius;

        gameObject.transform.position = new Vector3(x, 60f, z);
        gameObject.transform.Rotate(new Vector3(0f, step, 0f), Space.World);
        
    }
}
