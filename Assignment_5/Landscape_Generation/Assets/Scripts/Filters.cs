using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Filters : MonoBehaviour {
    private float filter_val;
    private float max_height;
    private Script01 script;

    private void Awake() {
        script = gameObject.GetComponent<Script01>();
    }

    public Vector3[] update_landscape(Vector3[] landscape, int resolution, string filter) {
        max_height = script.height;
        filter_val = script.filter_val;
        for (int i = 0; i < landscape.Length; ++i) {
            float x = Mathf.Floor(i / resolution);
            float y = i % resolution;
            float h = landscape[i].y;
            switch (filter) {
                case "Terraces":
                    landscape[i].y = terraces(x, y, h);
                    break;
                case "Desert":
                    landscape[i].y = desert(x, y, h);
                    break;
                case "Glacier":
                    landscape[i].y = glacier(x, y, h);
                    break;
                default:
                    return landscape;
            }
        }

        return landscape;
    }

    private float terraces(float x, float y, float height) {
        float stepProportion = Mathf.Floor((height / max_height) * filter_val);
        float theta = stepProportion * 2 * Mathf.PI;
        float newHeight = theta - Mathf.Sin(theta);
        return (max_height * newHeight) / (2 * Mathf.PI * filter_val);
    }

    private float desert(float x, float y, float height) {
        float halfmax = max_height / 2;
        float scaledHeight = filter_val * (height - halfmax / halfmax);
        float logistic = 1 / (1 + Mathf.Exp(scaledHeight));
        return max_height * (1 + logistic / 2);
    }

    private float glacier(float x, float y, float height) {
        float scaledHeight = filter_val * (height - max_height / max_height);
        float logistic = 1 / (1 + Mathf.Exp(scaledHeight));
        return max_height * ((1 + logistic) / 2);
    }

}
