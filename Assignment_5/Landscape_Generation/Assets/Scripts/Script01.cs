﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Script01 : MonoBehaviour {
    private bool _isDirty;
    private Mesh _mesh;
    [SerializeField] private Gradient gradient;

    [Range(0, 1)] [SerializeField] public float gain = 0.5f;
    [Range(1, 3)] [SerializeField] public float lacunarity = 2f;
    [Range(1, 8)] [SerializeField] public int octaves = 4;

    [SerializeField] public float scale = 5f;
    [SerializeField] public Vector2 shift = Vector2.zero;
    [SerializeField] public int state = 0;
    [SerializeField] public int resolution = 256;
    [SerializeField] public float length = 256f;
    [SerializeField] public float height = 50f;
    [SerializeField] public string filter = "Normal";
    [SerializeField] public float filter_val = 10f;
    private Filters filters;


    private void Awake() {
        (GetComponent<MeshFilter>().mesh = _mesh = new Mesh { name = name }).MarkDynamic();
        filters = gameObject.GetComponent<Filters>();
    }

    private void OnValidate() {
        _isDirty = true;
    }

    private void Update() {
        if (!_isDirty) return;
        GenerateLandscape();
        _isDirty = false;
    }

    private void GenerateLandscape() {
        // First, initialize the data structures:
        var colors = new Color[resolution * resolution];
        var triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        var vertices = new Vector3[resolution * resolution];

        int j = 0;

        // Then, loop over the vertices and populate the data structures:
        for (var i = 0; i < vertices.Length; i++) {

            float x = Mathf.Floor(i / length);
            float z = i % length;
            var coords = new Vector2((float)x / (resolution - 1), (float)z / (resolution - 1));
            var elevation = 1.414214f * FractalNoise(coords, gain, lacunarity, octaves, scale, shift, state);
            colors[i] = gradient.Evaluate(elevation + 0.5f);
            vertices[i] = new Vector3(length * coords.x, height * elevation, length * coords.y);

            if (i % resolution != resolution - 1 && i < (resolution * resolution) - resolution) {
                triangles[6 * j] = i;
                triangles[6 * j + 1] = i + 1;
                triangles[6 * j + 2] = i + resolution;

                triangles[6 * j + 3] = i + resolution + 1;
                triangles[6 * j + 4] = i + resolution;
                triangles[6 * j + 5] = i + 1;

                j++;
            }
        }

        vertices = filters.update_landscape(vertices, resolution, filter);

        for (var i = 0; i < vertices.Length; i++) {
            colors[i] = gradient.Evaluate(vertices[i].y / height);
        }

        // Assign the data structures to the mesh
        _mesh.Clear();
        _mesh.SetVertices(vertices);
        _mesh.SetColors(colors);
        _mesh.SetTriangles(triangles, 0);
        _mesh.RecalculateNormals();
    }

    private static float FractalNoise(Vector2 coords, float gain, float lacunarity, int octaves, float scale, Vector2 shift, int state) {
        // Tip:
        // Here, you can use the built-in Perlin noise implementation for each octave:
        // Mathf.PerlinNoise(x, y); such that:
        Random.InitState(state);
        float noise = 0f;
        for (int oct = 0; oct < octaves; oct++) {
            float x = coords.x * Mathf.Pow(lacunarity, oct) * scale + Random.value + shift.x;
            float y = coords.y * Mathf.Pow(lacunarity, oct) * scale + Random.value + shift.y;
            noise += (Mathf.PerlinNoise(x, y) - 0.5f) * Mathf.Pow(gain, oct); // -0.5 to modulate around 0. This prevents the terrain from going higher with every ocatve.
        }
        return noise;

    }

    public void UpdateGain(float value) { gain = value; OnValidate(); Update(); }
    public void UpdateLacunarity(float value) { lacunarity = value; OnValidate(); Update(); }
    public void UpdateOctaves(int value) { octaves = value; OnValidate(); Update(); }
    public void UpdateScale(float value) { scale = value; OnValidate(); Update(); }
    public void UpdateShift(Vector2 value) { shift = value; OnValidate(); Update(); }
    public void UpdateState(int value) { state = value; OnValidate(); Update(); }
    public void UpdateResolution(int value) { resolution = value; OnValidate(); Update(); }
    public void UpdateLength(float value) { length = value; OnValidate(); Update(); }
    public void UpdateHeight(float value) { height = value; OnValidate(); Update(); }
    public void UpdateFilter(string filt) { filter = filt; OnValidate(); Update(); }
    public void UpdateFilterVal(float value) { filter_val = value; OnValidate(); Update(); }
}
