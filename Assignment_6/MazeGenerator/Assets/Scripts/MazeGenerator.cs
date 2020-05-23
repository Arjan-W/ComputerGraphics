using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MazeGenerator.Assest.Scripts {

    public class Step {
        public int r;
        public int c;
        public string dir;

        public Step(int c, int r, string dir){
            this.r = r;
            this.c = c;
            this.dir = dir;
        }
    }
    
    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] Gradient gradient;
        public GameObject cell_prefab;
        public GameObject wall_prefab;
        public GameObject path_prefab;
        
        private Vector2 size;
        private Vector2 grid_size;
        private Vector3 offset;
        private GameObject grid;
        private GameObject[,] cells;
        private GameObject[,,] walls;

        private List<GameObject> path;

        void Awake() {
            // Find Maze background for dims
            RectTransform maze_bg = GameObject.Find("MazeBackground").gameObject.GetComponent<RectTransform>();
            size = new Vector2(maze_bg.rect.width-20, maze_bg.rect.height-20);

            // Find actual grid object
            grid = gameObject.transform.Find("Grid").gameObject;

            // Add box collider to grid
            gameObject.AddComponent<BoxCollider2D>();
            gameObject.GetComponent<BoxCollider2D>().size = grid.GetComponent<RectTransform>().localScale;
            gameObject.GetComponent<BoxCollider2D>().offset = grid.GetComponent<RectTransform>().localScale / 2;
        }

        public void OnMouseDown() {
            // Find which cell was clicked for seeding
            Vector2 mouse_pos = mouse2Cell(Input.mousePosition.x, Input.mousePosition.y);
            flood((int) mouse_pos.x, (int) mouse_pos.y);

        }

        public void BinaryTree(int cols=15, int rows=10) {
            // Make grid
            MakeGrid(cols, rows);
            // For each cell carve either north or east
            for (int c=0; c < cols; c++) {
                for (int r=0; r < rows; r++) {
                    float choice = UnityEngine.Random.Range(0f,1f);
                    if (choice < 0.5 && r < rows-1) { RemoveWall(c, r, "Up"); }
                    else if (choice < 0.5 && c < cols-1) { RemoveWall(c, r, "right"); }
                    else if (choice >= 0.5 && c < cols-1) { RemoveWall(c, r, "right"); }
                    else if (choice >= 0.5 && r < rows-1) { RemoveWall(c, r, "up"); }
                }
            }
        }

        public void AldousBroder(int cols=5, int rows=5) {
            // Make grid
            MakeGrid(cols, rows);
            
            // Choose random point to start
            int c = UnityEngine.Random.Range(0, cols);
            int r = UnityEngine.Random.Range(0, rows);

            // Initialize matrix of visited cells
            int[,] visited = new int[cols, rows];
            visited[c, r] = 1;

            // Initialize remaining nr of cells
            int remaining = rows*cols-1;
            // Loop until all cells visited
            
            while (remaining > 0) {
                // Pick random direction
                float choice = UnityEngine.Random.Range(0, 4);
                // Up
                if (choice  == 0 && r < rows-1) {
                    // Always move to cell
                    r++;
                    // If not visited, also carve
                    if(visited[c, r] != 1) { RemoveWall(c, r-1, "Up"); remaining--; visited[c, r] = 1;
                    }
                }
                // Down
                else if (choice == 1 && r > 0) {
                    // Always move to cell
                    r--;
                    // If not visited, also carve
                    if (visited[c, r] != 1){ RemoveWall(c, r+1, "Down"); remaining--; visited[c, r] = 1; 
                    }
                }
                // Left
                else if (choice == 2 && c > 0) {
                    // Always move to cell
                    c--;
                    // If not visited, also carve
                    if (visited[c, r] != 1) { RemoveWall(c+1, r, "Left"); remaining--; visited[c, r] = 1;
                    }       
                }
                // Right
                else if (choice == 3 && c < rows-1) {
                    // Always move to cell
                    c++;
                    // If not visited, also carve
                    if (visited[c, r] != 1){ RemoveWall(c-1, r, "Right"); remaining--; visited[c, r] = 1;
                    } 
                }
            }

        }

        /*public void Wilsons(int cols, int rows)
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
                }
            }
        }*/

        public void RecursiveBacktracker(int cols, int rows) {
            // Make grid
            MakeGrid(cols, rows);

            // Choose random point to start
            int c = UnityEngine.Random.Range(0, cols);
            int r = UnityEngine.Random.Range(0, rows);

            // Initialize matrix of visited cells
            // Includes backtracked direction
            int[,] visited = new int[cols, rows];
            visited[c, r] = 1;
            CarvePassagesFrom(c, r, cols, rows, visited);    
        }

        void reshuffle(string[] texts) {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < texts.Length; t++ ) {
                string tmp = texts[t];
                int r = UnityEngine.Random.Range(t, texts.Length);
                texts[t] = texts[r];
                texts[r] = tmp;
            }
        }

        private void CarvePassagesFrom(int c, int r, int cols, int rows, int[,] visited) {
            // Create shuffled list of directions
            string[] directions = new string[4]{"Up", "Down", "Left", "Right"};
            reshuffle(directions);
            foreach (string dir in directions) {
                if (dir == "Up" && r < rows-1 && visited[c, r+1] == 0) { RemoveWall(c, r, "Up"); r++; visited[c, r] = 1; CarvePassagesFrom(c, r, cols, rows, visited);}
                else if (dir == "Down" && r > 0 && visited[c, r-1] == 0) { RemoveWall(c, r, "Down"); r--; visited[c, r] = 1; CarvePassagesFrom(c, r, cols, rows, visited);}
                else if (dir == "Left" && c > 0 && visited[c-1, r] == 0) { RemoveWall(c, r, "Left"); c--; visited[c, r] = 1; CarvePassagesFrom(c, r, cols, rows, visited);}
                else if (dir == "Right" && c < cols-1 && visited[c+1, r] == 0) { RemoveWall(c, r, "Right"); c++; visited[c, r] = 1; CarvePassagesFrom(c, r, cols, rows, visited);}
            }
        }

        private List<Step> RandomWalk(int cols, int rows, int[,] visited) {
            print("Start randomwalk");
            int c, r = 0;
            List<Step> path = new List<Step>();
            // Loop until random cell is selected that is not in the maze (i.e. visited)
            do {
                c = UnityEngine.Random.Range(0, cols-1);
                r = UnityEngine.Random.Range(0, rows-1);
            }
            while (visited[c, r] == 1);
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
                    path.Add(new Step(c, r, "Up"));
                    print("path.add r: " + r + ", c: " + c + ", dir: Up");
                    // If that cell already in maze (i.e. visited), break out of loop as we've finished walk
                    if(visited[c, r+1] == 1) {
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
                    path.Add(new Step(c, r, "Down"));
                    print("path.add r: " + r + ", c: " + c + ", dir: Down");
                    // If that cell already in maze (i.e. visited), break out of loop as we've finished walk
                    if(visited[c, r-1] == 1) {
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
                    path.Add(new Step(c, r, "Left"));
                    print("path.add r: " + r + ", c: " + c + ", dir: Left");
                    // If that cell already in maze (i.e. visited), break out of loop as we've finished walk
                    if(visited[c-1, r] == 1) {
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
                    path.Add(new Step(c, r, "Right"));
                    print("path.add r: " + r + ", c: " + c + ", dir: Right");
                    // If that cell already in maze (i.e. visited), break out of loop as we've finished walk
                    if(visited[c+1, r] == 1) {
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
                    if (r < rows-1 && (c+1 == cols || UnityEngine.Random.Range(0,2) == 1)){
                        // End current run
                        // Randomly choose cell from current run set
                        int cell = run_start + UnityEngine.Random.Range(0, c - run_start + 1);
                        // Carve down from it
                        RemoveWall(cell, r, "up"); 
                        run_start = c+1;

                    }
                    // If not already in most right column
                    else if (c+1 < cols) {
                        // Carve east
                        RemoveWall(c, r, "right");
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
            offset = new Vector3(0, 0, 0);
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

            // Scale box
            gameObject.GetComponent<BoxCollider2D>().size = grid.GetComponent<RectTransform>().localScale * grid_size;
        }

        public void DrawPath() {
            // Clear current path
            clear_elements(path);
            // Make list
            path = new List<GameObject>();
            // Get longest path
            List<Vector2> l_path = longest_path();
            // Loop over points in the path
            foreach (Vector2 p in l_path) {
                // Create wall
                GameObject dot = (GameObject) Instantiate(path_prefab);
                        dot.transform.SetParent(grid.transform);
                        dot.transform.localPosition = new Vector3((int) p.x + 0.5f, (int) p.y + 0.5f, 0) + offset;
                        dot.transform.localScale = new Vector3(0.2f, 0.2f, 1);
                        path.Add(dot);
            }
        }

        private bool HasWall (int x, int y, string dir) {
            if (dir.ToLower()=="up") { return walls[x, y+1, 0].activeSelf; }
            if (dir.ToLower()=="right") { return walls[x+1, y, 1].activeSelf; }
            if (dir.ToLower()=="down") { return walls[x, y, 0].activeSelf; }
            if (dir.ToLower()=="left") { return walls[x, y, 1].activeSelf; }
            return true;
        }

        private void RemoveWall (int x, int y, string dir) {
            if (dir.ToLower()=="up") { walls[x, y+1, 0].SetActive(false); }
            if (dir.ToLower()=="right") { walls[x+1, y, 1].SetActive(false); }
            if (dir.ToLower()=="down") { walls[x, y, 0].SetActive(false); }
            if (dir.ToLower()=="left") { walls[x, y, 1].SetActive(false); }
        }

        private GameObject[,] make_cells (int cols, int rows, Vector3 offset) {
            GameObject[,] cell_grid = new GameObject[cols, rows];
            for (int c=0; c < cols; ++c) {
                for (int r=0; r < rows; ++r) {
                    GameObject cell = (GameObject) Instantiate(cell_prefab);
                    cell.transform.SetParent(grid.transform);
                    cell.transform.localPosition = new Vector3(c, r, 0) + offset;
                    cell.transform.localScale = new Vector3(1, 1, 1);
                    cell_grid[c, r] = cell;
                }
            }
            return cell_grid;
        }

        private GameObject[,,] make_walls (int cols, int rows, Vector3 offset) {
            GameObject[,,] wall_grid = new GameObject[cols, rows, 2];
            for (int c=0; c < cols; ++c) {
                for (int r=0; r < rows; ++r) {
                    if (c < cols-1) {
                        GameObject h_wall = (GameObject) Instantiate(wall_prefab);
                        h_wall.transform.SetParent(grid.transform);
                        h_wall.transform.localPosition = new Vector3(c, r, 0) + offset;
                        h_wall.transform.eulerAngles = new Vector3(0f, 0f, -90f);
                        h_wall.transform.localScale = new Vector3(2, 1, 1);
                        wall_grid[c, r, 0] = h_wall;
                    }
                    if (r < rows-1) {
                        GameObject v_wall = (GameObject) Instantiate(wall_prefab);
                        v_wall.transform.SetParent(grid.transform);
                        v_wall.transform.localPosition = new Vector3(c, r, 0) + offset;
                        v_wall.transform.localScale = new Vector3(2, 1, 1);
                        wall_grid[c, r, 1] = v_wall;
                    }
                }
            }
            return wall_grid;
        }

        private void clear_elements(GameObject[,,] list) {
            if(list != null){
                for(int i=0; i < list.GetLength(0); ++i) {
                    for(int j=0; j < list.GetLength(1); ++j) {
                        for (int k=0; k < list.GetLength(2); ++k) {
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
                for(int i=0; i < list.GetLength(0); ++i) {
                    for(int j=0; j < list.GetLength(1); ++j) {
                        if (list[i, j] != null) {
                            Destroy(list[i, j].gameObject);
                        }
                    }
                }
            }
        }

        private void clear_elements(List<GameObject> list) {
            if(list != null) {
                foreach (GameObject obj in list) {
                    Destroy(obj);
                }
                list.Clear();
            }
        }

        private Vector2 mouse2Cell(float x, float y) {
            // Create output vector
            Vector2 cell_idx = new Vector2();
            // Transform screen coords to rect coords
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gameObject.GetComponent<RectTransform>(),
                new Vector2(x, y),
                GameObject.Find("UICamera").GetComponent<Camera>(),
                out cell_idx
                );
            // Shift pixels so bottom left becomes origin
            cell_idx += gameObject.GetComponent<BoxCollider2D>().size / 2;
            // Scale to grid
            cell_idx /= gameObject.GetComponent<BoxCollider2D>().size / grid_size;
            // Floor to get integers
            cell_idx = new Vector2(Mathf.FloorToInt(cell_idx.x), Mathf.FloorToInt(cell_idx.y));
            return cell_idx;
        }

        private float[,] compute_dist_grid(int x, int y, int dist=0, float[,] dist_grid=null) {
            // Make distance grid
            if (dist_grid == null) { dist_grid = new float[(int) grid_size.x, (int) grid_size.y]; }
            // Fill current dist in on pos
            dist_grid[x, y] = dist;
            // For each valid move, recursively fill in values
            string[] dirs = new string[4] {"up", "right", "down", "left"};
            foreach (string dir in dirs) {
                // Check if move is not blocked by wall
                if (HasWall(x, y, dir)) {continue; }
                // Set new pos
                int new_x = x;
                int new_y = y;
                if (dir == "up") {new_y += 1;}
                if (dir == "right") {new_x += 1;}
                if (dir == "down") {new_y -= 1;}
                if (dir == "left") {new_x -= 1;}
                // Check if position has been reached yet
                if (dist_grid[new_x, new_y] != 0f && dist_grid[new_x, new_y] <= dist) {continue; }
                // Recursively fill rest
                compute_dist_grid(new_x, new_y, dist+1, dist_grid);
            }
            // Reset original point to dist of 0
            if (dist == 0) { dist_grid[x, y] = dist; }
            // Return grid
            return dist_grid;
        }

        public void flood(int x=0, int y=0) {
            // Compute distances
            float[,] dist_grid = compute_dist_grid(x, y);

            // Get max dist
            float dist_max = max_dist(dist_grid).z;

            // Color based on normalized distance and gradient
            for(int c=0; c < cells.GetLength(0); ++c) {
                for(int r=0; r < cells.GetLength(1); ++r) {
                    Color color = gradient.Evaluate(dist_grid[c, r] / dist_max);
                    cells[c, r].GetComponent<SpriteRenderer>().color = color;
                }
            }
        }

        public Vector3 max_dist(float[,] dist_grid) {
            // Set values to zero
            float dist_max = 0;
            int max_x = 0;
            int max_y = 0;
            // Loop over cells to find max_dist and index values
            for(int c=0; c < cells.GetLength(0); ++c) {
                for(int r=0; r < cells.GetLength(1); ++r) {
                    if (dist_grid[c, r] > dist_max) { 
                        dist_max = dist_grid[c, r]; 
                        max_x = c;
                        max_y = r;    
                    }
                }
            }
            return new Vector3(max_x, max_y, dist_max);
        }

        public List<Vector2> longest_path() {
            // Take a random point
            int rand_x = UnityEngine.Random.Range(0, cells.GetLength(0));
            int rand_y = UnityEngine.Random.Range(0, cells.GetLength(1));
            // Compute distances
            float[,] dist_grid = compute_dist_grid(rand_x, rand_y);
            // Grab index of cell with max_dist
            Vector3 A = max_dist(dist_grid);
            // Compute distances
            dist_grid = compute_dist_grid((int) A.x, (int) A.y);
            // Grab index of cell with max_dist
            Vector3 B = max_dist(dist_grid);
            // Compute path from A to B
            return shortest_path(A, B, dist_grid);
        }

        public List<Vector2> shortest_path(Vector2 A, Vector2 B, float[,] dist_grid) {
            // Create empty path list
            List<Vector2> path = new List<Vector2>();
            // Record current position starting at B
            Vector2 curr_pos = B;
            // While not at A follow dist_grid back
            while (curr_pos != A) {
                // Add current position to path
                path.Add(curr_pos);
                // Look for the cell with a distance that is one lower.
                string[] dirs = new string[4] {"up", "right", "down", "left"};
                foreach (string dir in dirs) {
                    // Check if move is not blocked by wall
                    if (HasWall((int) curr_pos.x, (int) curr_pos.y, dir)) {continue; }
                    // Set new pos
                    int new_x = (int) curr_pos.x;
                    int new_y = (int) curr_pos.y;
                    if (dir == "up") {new_y += 1;}
                    if (dir == "right") {new_x += 1;}
                    if (dir == "down") {new_y -= 1;}
                    if (dir == "left") {new_x -= 1;}
                    // Check if value of neighboring cell is one less
                    if (dist_grid[new_x, new_y] == dist_grid[(int) curr_pos.x, (int) curr_pos.y] - 1) {
                        curr_pos = new Vector2(new_x, new_y);
                        break;
                    }
                }
            }
            path.Add(curr_pos);
            return path;
        }

    }
}