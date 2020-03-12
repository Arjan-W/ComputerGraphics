using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mandelbrod.Assets.Scripts {

    public class Exercise02 : MonoBehaviour {

        [SerializeField] private ComputeShader computeShader;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private Texture texture;

        private int iterations = 100;
        private float bailout = 2;
        private float bottom = -1;
        private float left = -2.5f;
        private float right = -1f;
        private float top = 1;


        // declare fields (see the Reset method for the required fields)
        private void Start() {
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
            var kernelIndex = computeShader.FindKernel("CSMain");
            // Set shader variables (ints and floats)
            computeShader.SetTexture(kernelIndex, "_ColorGradient", texture);
            computeShader.SetTexture(kernelIndex, "_MandelbrotSet", renderTexture);
            computeShader.SetInt(kernelIndex, iterations);
            computeShader.SetInt(kernelIndex, renderTexture.width);
            computeShader.SetInt(kernelIndex, renderTexture.height);
            computeShader.SetFloat(kernelIndex, bailout);
            computeShader.SetFloat(kernelIndex, left);
            computeShader.SetFloat(kernelIndex, right);
            computeShader.SetFloat(kernelIndex, top);
            computeShader.SetFloat(kernelIndex, bottom);
            computeShader.Dispatch(kernelIndex, renderTexture.width / 8, renderTexture.height / 8, 1);
        }

        private void OnDestroy() {
            renderTexture.Release();
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
