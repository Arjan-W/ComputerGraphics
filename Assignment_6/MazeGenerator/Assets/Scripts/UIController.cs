using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace MazeGenerator.Assest.Scripts.UIController{
    public class UIController : MonoBehaviour {
        private GameObject grid;
        private GameObject algorithm;
        private GameObject generate;
        private MazeGenerator maze_gen;


        // Start is called before the first frame update
        void Awake() {

            // Find script
            maze_gen = GameObject.Find("Maze").GetComponent<MazeGenerator>();

            // Find settings
            grid = gameObject.transform.Find("Menu").Find("GridSize").gameObject;
            algorithm = gameObject.transform.Find("Menu").Find("AlgorithmSelect").gameObject;
            generate = gameObject.transform.Find("Menu").Find("Generate").gameObject; 
            
            // Add algorithm options
            Dropdown algorithm_options = algorithm.transform.Find("Algorithms").GetComponent<Dropdown>();
            algorithm_options.ClearOptions();
            algorithm_options.AddOptions(new List<string> {"Binary Tree"});
            algorithm_options.AddOptions(new List<string> {"Aldous-Broder"});
            algorithm_options.AddOptions(new List<string> {"Sidewinder"});
            algorithm_options.AddOptions(new List<string> {"Wilson's"});

            // Add callback to generate
            generate.transform.Find("GenerateButton").GetComponent<Button>().onClick.AddListener(delegate() {Start();} );
        }

        void Start() {
            // Read rows and cols
            int cols = int.Parse(grid.transform.Find("X").Find("X_val").GetComponent<InputField>().text);
            int rows = int.Parse(grid.transform.Find("Y").Find("Y_val").GetComponent<InputField>().text);

            // Read algorithm
            int alg = algorithm.transform.Find("Algorithms").GetComponent<Dropdown>().value;

            // Run algorithm
            if (alg == 0) { maze_gen.BinaryTree(cols, rows); }
            if (alg == 1) { maze_gen.AldousBroder(cols, rows); }
            if (alg == 2) { maze_gen.SideWinder(cols, rows); }
            if (alg == 3) { maze_gen.Wilsons(cols, rows); }
        }

    }

}
