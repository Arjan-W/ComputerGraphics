using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Exercise1 : MonoBehaviour {

    public int width = 256, height = 256, iterations;
    private float botLeftX = -2.5f, botLeftY = -2.5f, valSize = 5;
    private Renderer rend;
    private Vector3 offset = new Vector3(0f, 0f, 0f);

    void Start() {
        rend = GetComponent<Renderer>();
        rend.material.mainTexture = drawMandelbrot();
    }

    Texture2D drawMandelbrot() {
        Texture2D texture = new Texture2D(width, height);
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                float temp1 = botLeftX + ((valSize * (float) i) / width);
                float temp2 = botLeftY + ((valSize * (float) j) / height);
                Color color = mandelBrot(temp1, temp2);              
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply();
        return texture;
    }

    Color mandelBrot(float a, float b) {
        float x = 0, y = 0, x2 = 0, y2 = 0;
        float i = 0;
        for(; i < iterations && x2 + y2 <= 4; i++) {
            y = 2 * x * y + b;
            x = x2 - y2 + a;
            x2 = x * x;
            y2 = y * y;
        }

        if (x2 + y2 >= 4) {
            float log_zn = Mathf.Log(x2 + y2) / 2;
            float nu = Mathf.Log(log_zn / Mathf.Log(2)) / Mathf.Log(2);
            i += (1 - nu);
            HSBColor col1 = new HSBColor(i % 1f, 1f, 1f);
            return col1.ToColor();
        } else {
            return Color.black;
        }
        
    }
    

    void Update() {
        if (Input.GetMouseButtonDown(0)) {            
            Vector3 mPos = (Camera.main.ScreenToWorldPoint(Input.mousePosition) / (5f / valSize)) + offset;            
            valSize *= 0.5f;
            botLeftX = ((float) mPos.x) / (8f / 5f) - valSize * 0.5f;
            botLeftY = ((float) mPos.y) / (8f / 5f) - valSize * 0.5f;
            offset = mPos;
            rend.material.mainTexture = drawMandelbrot();
        } else if (Input.GetMouseButtonDown(1)) {
            valSize  = 5f;
            botLeftX = -2.5f;
            botLeftY = -2.5f;
            offset.x = offset.y = offset.z = 0f;
            rend.material.mainTexture = drawMandelbrot();
        }
    }

}