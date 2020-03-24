using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Mandelbrod.Assets.Scripts {

    public class Exercise02 : MonoBehaviour {

        public float zoomspeed = 0.95f;
        private bool zoom = true;

        [SerializeField] private ComputeShader computeShader;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private Texture texture;

        private int iterations = 100;
        private float bailout = 2;
        private float bottom = -1;
        private float left = -2.5f;
        private float right = 1f;
        private float top = 1;


        // declare fields (see the Reset method for the required fields)
        private void Start() {
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
            var kernelIndex = computeShader.FindKernel("CSMain");
            // Set shader variables (ints and floats)
            computeShader.SetTexture(kernelIndex, "_ColorGradient", texture);
            computeShader.SetTexture(kernelIndex, "_MandelbrotSet", renderTexture);
            computeShader.SetInt("_Iterations", iterations);
            computeShader.SetInt("_Width", renderTexture.width);
            computeShader.SetInt("_Height", renderTexture.height);
            computeShader.SetFloat("_Bailout", bailout);
            computeShader.SetFloat("_Left", left);
            computeShader.SetFloat("_Right", right);
            computeShader.SetFloat("_Top", top);
            computeShader.SetFloat("_Bottom", bottom);
            computeShader.Dispatch(kernelIndex, renderTexture.width / 8, renderTexture.height / 8, 1);
        }

        private void OnDestroy() {
            renderTexture.Release();
        }

        private void Update() {
            if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && zoom) {
                var width = right - left;
                var height = top - bottom;

                // Record mouse position
                var mousePos = Input.mousePosition;
                // Translate to mandelbrod scale
                mousePos = new Vector3((mousePos.x / Screen.width) * width + left, ((Screen.height - mousePos.y) / Screen.height) * height + bottom);

                if (Input.GetButtonDown("Fire1")) {
                    width =  width * zoomspeed;
                    height = height * zoomspeed;
                }
                if (Input.GetButtonDown("Fire2")) {
                    width = width * (1 / zoomspeed);
                    height = height * (1 / zoomspeed);
                }
                // Compute new values
                left   = mousePos.x - width / 2;
                right  = mousePos.x + width / 2;
                bottom = mousePos.y - height / 2;
                top    = mousePos.y + height / 2;
                // Update shader
                var kernelIndex = computeShader.FindKernel("CSMain");
                computeShader.SetFloat("_Left", left);
                computeShader.SetFloat("_Right", right);
                computeShader.SetFloat("_Top", top);
                computeShader.SetFloat("_Bottom", bottom);
                computeShader.Dispatch(kernelIndex, renderTexture.width / 8, renderTexture.height / 8, 1);
            }
        }


        public void toggleClickig(BaseEventData eventData) {
            zoom = !zoom;
        }

        public void setZoomScale(string value) {
            zoomspeed = float.Parse(value, CultureInfo.InvariantCulture);
        }

        private void Reset() {
            iterations = 100;
            bailout = 2;
            bottom = -1;
            left = -2.5f;
            right = 1;
            top = 1;
            computeShader = null;
            renderTexture = null;
            texture = null;
        }
    }
}
