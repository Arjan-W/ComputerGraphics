using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
    float value;
    float prevValue;

    private Slider _Slider;
    public GameObject[] shadedObjects;


    void Start() {
        _Slider = GameObject.FindObjectOfType<Slider>();
    }


    private void OnGUI() {
        value = _Slider.value;
    }


    void Update() {
        if (prevValue != value) {
            prevValue = value;

            foreach (var comp in shadedObjects) {
                foreach (var mat in comp.GetComponent<Renderer>().sharedMaterials) {
                    mat.SetFloat("_Snow", value);
                }
            }
                
                
        }
    }
}


