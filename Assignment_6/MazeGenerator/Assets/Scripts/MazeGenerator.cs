using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGenerator.Assest.Scripts {

    public class Step {
        public int r;
        public int c;
        public string dir;

        public Step(int r, int c, string dir){
            this.r = r;
            this.c = c;
            this.dir = dir;
        }
    }
    
    public class MazeGenerator : MonoBehaviour
    {
        public GameObject cell_prefab;
        public GameObject wall_prefab;
        
        private Vector2 size;
        private Vector2 grid_size;
        private GameObject grid;
        private GameObject[,] cells;
        private GameObject[,,] walls;

        void Awake() {
            // Find Maze background for dims
            RectTransform maze_bg = GameObject.Find("MazeBackground").gameObject.GetComponent<RectTransform>();
            size = new Vector2(maze_bg.rect.width-20, maze_bg.rect.height-20);

            // Find actual grid object
            grid = gameObject.transform.Find("Grid").gameObject;
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        public void BinaryTree(int cols=15, int rows=10) {
            print("Bin!");
            // Make grid
            MakeGrid(cols, rows);
            // For each cell carve either north or east
            for (int r=0; r < rows; r++) {
                for (int c=0; c < cols; c++) {
                    float choice = UnityEngine.Random.Range(0f,1f);
                    if (choice < 0.5 && r < rows-1) {RemoveWall(r, c, "Up"); }
                    else if (choice < 0.5 && c < cols-1) {RemoveWall(r, c, "right"); }
                    else if (choice >= 0.5 && c < cols-1) {RemoveWall(r, c, "right"); }
                    else if (choice >= 0.5 && r < rows-1) {RemoveWall(r, c, "up"); }
                }
            }
        }

        public void AldousBroder(int cols=5, int rows=5) {
            // Make grid
            MakeGrid(cols, rows);
            
            // Choose random point to start
            int r = UnityEngine.Random.Range(0, rows);
            int c = UnityEngine.Random.Range(0, cols);

            // Initialize matrix of visited cells
            int[,] visited = new int[rows,cols];
            visited[r,c] = 1;

            // Initialize remaining nr of cells
            int remaining = rows*cols-1;
            // Loop until all cells visited
            
            while (remaining > 0) {
                // Pick random direction
                float choice = UnityEngine.Random.Range(0,4);
                // Up
                if (choice  == 0 && r < rows-1) {
                    // Always move to cell
                    r++;
                    // If not visited, also carve
                    if(visited[r,c] != 1) {
                        RemoveWall(r-1, c, "Up"); 
                        remaining--;
                        visited[r,c] = 1;
                    }
                }
                // Down
                else if (choice == 1 && r > 0) {
                    // Always move to cell
                    r--;
                    // If not visited, also carve
                    if (visited[r,c] != 1){
                        RemoveWall(r+1, c, "Down"); 
                        remaining--; 
                        visited[r,c] = 1; 
                    }
                }
                // Left
                else if (choice == 2 && c > 0) {
                    // Always move to cell
                    c--;
                    // If not visited, also carve
                    if (visited[r,c] != 1) {
                        RemoveWall(r, c+1, "Left"); 
                        remaining--;
                        visited[r,c] = 1;
                    }       
                }
                // Right
                else if (choice == 3 && c < rows-1) {
                    // Always move to cell
                    c++;
                    // If not visited, also carve
                    if (visited[r,c] != 1){
                        RemoveWall(r, c-1, "Right"); 
                        remaining--; 
                        visited[r,c] = 1;
                    } 
                }
            }

        }

        public void Wilsons(int cols, int rows)
        {
            print("Start");
            MakeGrid(cols, rows);
            
            // Choose random point to start
            int r = UnityEngine.Random.Range(0, rows);
            int c = UnityEngine.Random.Range(0, cols);
            print("r: " + r + ", c: " + c);
            int[,] visited = new int[rows,cols];
            // Set random cell as IN
            visited[r,c] = 1;
            print("visited[r,c]: " + visited[r,c]);

            // Count nr of remaining cells
            int remaining = rows*cols-1;
            print("remaining: " + remaining);

            while (remaining > 0) {
                // For each cell in RandomWalk perform carve
                Step[] walkArray = RandomWalk(rows, cols, visited).ToArray();
                remaining--;
                /*foreach (Step step in walkArray) {
                    RemoveWall(step.x,step.y,step.dir);
                    visited[step.x,step.y] = 1;
                    remaining--;
                }*/
            }
        }

        private List<Step> RandomWalk(int rows, int cols, int[,] visited) {
            print("Start randomwalk");
            int r, c = 0;
            List<Step> path = new List<Step>();
            // Loop until random cell is selected that is not in the maze (i.e. visited)
            do {
                r = UnityEngine.Random.Range(0, rows-1);
                c = UnityEngine.Random.Range(0, cols-1);
            }
            while (visited[r,c] == 1);
            print("Randomwalk r: " + r + ", c: " + c);
            // Initialize list of visited cells in random walk
            bool walking = true;
            while (walking) {
                print("keep walking");
                // Be optimistic: next move will be visited cell
                walking = false;
                // Pick random direction
                int choice = UnityEngine.Random.Range(0,4);
                print("dir choice: " + choice);
                // Check for chosen direction whether it reaches a valid cell
                // Up
                if (choice  == 0 && r < rows-1) {
                    path.Add(new Step(r,c,"Up"));
                    print("path.add r: " + r + ", c: " + c + ", dir: Up");
                    // If that cell already in maze (i.e. visited), break out of loop as we've finished walk
                    if(visited[r+1,c] == 1) {
                        print("visited is true for r w/o +1: " + r + " and c: " + c);
                        break;
                    }
                    // Else, set neighbour to be current cell and continue walking
                    else {
                        r++;
                        walking = true;
                        continue;
                    }
                }
                // Down
                else if (choice == 1 && r > 0) {
                    path.Add(new Step(r,c,"Down"));
                    print("path.add r: " + r + ", c: " + c + ", dir: Down");
                    // If that cell already in maze (i.e. visited), break out of loop as we've finished walk
                    if(visited[r-1,c] == 1) {
                        print("visited is true for r w/o -1: " + r + " and c: " + c);
                        break;
                    }
                    // Else, set neighbour to be current cell and continue walking
                    else {
                        r--;
                        walking = true;
                        continue;
                    }
                }
                // Left
                else if (choice == 2 && c > 0) {
                    path.Add(new Step(r,c,"Left"));
                    print("path.add r: " + r + ", c: " + c + ", dir: Left");
                    // If that cell already in maze (i.e. visited), break out of loop as we've finished walk
                    if(visited[r,c-1] == 1) {
                        print("visited is true for r: " + r + " and c w/o -1: " + c);
                        break;
                    }
                    // Else, set neighbour to be current cell and continue walking
                    else {
                        c--;
                        walking = true;
                        continue;
                    }
                }
                // Right
                else if (choice == 3 && c < rows-1) {
                    path.Add(new Step(r,c,"Right"));
                    print("path.add r: " + r + ", c: " + c + ", dir: Right");
                    // If that cell already in maze (i.e. visited), break out of loop as we've finished walk
                    if(visited[r,c+1] == 1) {
                        print("visited is true for r: " + r + " and c w/o +1: " + c);
                        break;
                    }
                    // Else, set neighbour to be current cell and continue walking
                    else {
                        c++;
                        walking = true;
                        continue;
                    }
                }
                
            }
            print("path[0]: " + path[0]);
            return path;
        }

        public void SideWinder(int cols=15, int rows=15)
        {
            // Make grid
            MakeGrid(cols, rows);
            // For each cell carve either north or east
            for (int r=0; r < rows; r++) {
                int run_start = 0;
                for (int c=0; c < cols; c++) {
                    // If not on bottom row (can't go down there)
                    // and (either already right column or random select down)
                    if (r > 0 && (c+1 == cols || UnityEngine.Random.Range(0,2) == 1)){
                        // End current run
                        // Randomly choose cell from current run set
                        int cell = run_start + UnityEngine.Random.Range(0, c - run_start + 1);
                        // Carve down from it
                        RemoveWall(r, cell, "Down"); 
                        run_start = c+1;

                    }
                    // If not already in most right column
                    else if (c+1 < cols) {
                        // Carve east
                        RemoveWall(r, c, "Right");
                    }
                }
            }
        }

        public void MakeGrid(int cols=15, int rows=10) {
            // Store grid size
            grid_size = new Vector2Int(cols, rows);
            // Scales the grid to fit all cells in the image
            float x = size[0] / cols;
            float y = size[1] / rows;
            float s = Math.Min(x, y);
            Vector3 offset = new Vector3(0, 0, 0);
            if (x < y) { offset = new Vector3(0, (size[1] - s * rows) / s / 2, 0); }
            if (x > y) { offset = new Vector3((size[0] - s * cols) / s / 2, 0, 0); }

            // Scale grid
            grid.transform.localScale = new Vector3(s, s, 0);

            // Clear objects
            clear_elements(cells);
            clear_elements(walls);

            // Add cells and walls
            cells = make_cells(rows, cols, offset);
            walls = make_walls(rows+1, cols+1, offset);
        }

        private void RemoveWall (int x, int y, string dir) {
            if (dir.ToLower()=="up") { walls[x+1, y, 0].SetActive(false); }
            if (dir.ToLower()=="right") { walls[x, y+1, 1].SetActive(false); }
            if (dir.ToLower()=="down") { walls[x, y, 0].SetActive(false); }
            if (dir.ToLower()=="left") { walls[x, y, 1].SetActive(false); }
        }

        private GameObject[,] make_cells (int rows, int cols, Vector3 offset) {
            GameObject[,] cell_grid = new GameObject[rows, cols];
            for (int r=0; r < rows; r++) {
                for (int c=0; c < cols; c++) {
                    GameObject cell = (GameObject) Instantiate(cell_prefab);
                    cell.transform.SetParent(grid.transform);
                    cell.transform.localPosition = new Vector3(c, r, 0) + offset;
                    cell.transform.localScale = new Vector3(1, 1, 1);
                    cell_grid[r, c] = cell;
                }
            }
            return cell_grid;
        }

        private GameObject[,,] make_walls (int rows, int cols, Vector3 offset) {
            GameObject[,,] wall_grid = new GameObject[rows, cols, 2];
            for (int r=0; r < rows; r++) {
                for (int c=0; c < cols; c++) {
                    if (c < cols-1) {
                        GameObject h_wall = (GameObject) Instantiate(wall_prefab);
                        h_wall.transform.SetParent(grid.transform);
                        h_wall.transform.localPosition = new Vector3(c, r, 0) + offset;
                        h_wall.transform.eulerAngles = new Vector3(0f, 0f, -90f);
                        h_wall.transform.localScale = new Vector3(2, 1, 1);
                        wall_grid[r, c, 0] = h_wall;
                    }
                    if (r < rows-1) {
                        GameObject v_wall = (GameObject) Instantiate(wall_prefab);
                        v_wall.transform.SetParent(grid.transform);
                        v_wall.transform.localPosition = new Vector3(c, r, 0) + offset;
                        v_wall.transform.localScale = new Vector3(2, 1, 1);
                        wall_grid[r, c, 1] = v_wall;
                    }
                }
            }
            return wall_grid;
        }

        private void clear_elements(GameObject[,,] list) {
            if(list != null){
                for(int i=0; i < list.GetLength(0); i++) {
                    for(int j=0; j < list.GetLength(1); j++) {
                        for (int k=0; k < list.GetLength(2); k++) {
                            if (list[i, j, k] != null) {
                                Destroy(list[i, j, k].gameObject);
                            }
                        }
                    }
                }
            }
        }

        private void clear_elements(GameObject[,] list) {
            if(list != null){
                for(int i=0; i < list.GetLength(0); i++) {
                    for(int j=0; j < list.GetLength(1); j++) {
                        if (list[i, j] != null) {
                            Destroy(list[i, j].gameObject);
                        }
                    }
                }
            }
        }

    }
}