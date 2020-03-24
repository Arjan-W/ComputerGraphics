using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mandelbrod.Assets.Scripts {

    public class Exercise01 : MonoBehaviour {

        [SerializeField] private ComputeShader computeShader;
        [SerializeField] private RenderTexture renderTexture;

        // Start is called before the first frame update

        private void Start() {
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
            var kernelIndex = computeShader.FindKernel("CSMain");
            computeShader.SetTexture(kernelIndex, "Result", renderTexture);
            computeShader.Dispatch(kernelIndex, renderTexture.width / 8, renderTexture.height / 8, 1);
        }

        private void OnDestroy() {
            renderTexture.Release();
        }
    }
}

