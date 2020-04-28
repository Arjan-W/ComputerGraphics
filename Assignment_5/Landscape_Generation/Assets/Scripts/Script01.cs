using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Script01 : MonoBehaviour
{
    private bool _isDirty;
    private Mesh _mesh;
    [SerializeField] private Gradient gradient;

    [Range(0, 1)] [SerializeField] private float gain = 0.5f;
    [Range(1, 3)] [SerializeField] private float lacunarity = 2f;
    [Range(1, 8)] [SerializeField] private int octaves = 4;

    [SerializeField] private float scale = 5f;
    [SerializeField] private Vector2 shift = Vector2.zero;
    [SerializeField] private int state = 0;
    [SerializeField] private int resolution = 256;
    [SerializeField] private float length = 256f;
    [SerializeField] private float height = 50f;

    private void Awake() {
        (GetComponent<MeshFilter>().mesh = _mesh = new Mesh {name=name}).MarkDynamic();
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
        var colors = new Color[_mesh.vertices.Length];
        var triangles = new int[_mesh.triangles.Length * 2];
        var vertices = new Vector3[_mesh.vertices.Length];
      
        // Then, loop over the vertices and populate the data structures:
        for (var i = 0; i < _mesh.vertices.Length; i++) {
            Vector3 v = _mesh.vertices[i];
            var coords = new Vector2((float) v.x / (resolution - 1), (float) v.z / (resolution - 1));
            var elevation = 1.414214f * FractalNoise(coords, gain, lacunarity, octaves, scale, shift, state);
            colors[i]   = gradient.Evaluate(elevation);
            vertices[i] = new Vector3(length * coords.x, height * elevation, length * coords.y);
            triangles[i]= i;
        }

        // Assign the data structures to the mesh
        _mesh.Clear();
        _mesh.SetVertices(vertices);
        _mesh.SetColors(colors);
        _mesh.SetTriangles(triangles, 0);
        _mesh.RecalculateNormals();
    }

    private static float FractalNoise(Vector2 coords, float gain, float lacunarity, int octaves, float scale, Vector2 shift, int state) {
        /*
        * Tip:
        * Here, you can use the built-in Perlin noise implementation for each octave:
        * Mathf.PerlinNoise(x, y); such that:
        * x = coords.x * frequency.x * scale + some random number (seeded by state at the beginning) + shift.x; and
        * y = coords.y * frequency.y * scale + some random number (seeded by state at the beginning) + shift.y; and
        */
        throw new NotImplementedException();
    }
}
