using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MyNamespace;
using UnityEngine.AI;
using Unity.AI.Navigation;
//using UnityEngine.UIElements;

enum TileType
{
    WALL = 0,
    FLOOR = 1,
}
public class Result
{
    public bool result { get; set; }
    public int w { get; set; }
    public int l { get; set; }
}

public static class GameData
{
    public static int width = 16; 
    public static int length = 16; 
    public static float total_time = 50f; 
    // public static float spider_rest = 7.5f; 
}

public class Maze : MonoBehaviour
{
    // fields/variables you may adjust from Unity's interface
    // public int width = 16;   // size of level (default 16 x 16 blocks)
    // public int length = 16;
    public int width; 
    public int length; 
    public float storey_height = 2.5f;   // height of walls
    public GameObject fps_prefab;        // these should be set to prefabs as provided in the starter scene
    public GameObject coin_prefab;
    public GameObject time_prefab; 
    public GameObject magn_prefab; 
    public GameObject shield_prefab; 
    public GameObject rand_prefab; 
    public GameObject text_box;
    public Material coin_mat; 

    public Text score_text;
    public Text game_text; 
    private int score;
    private int single_score = 2;

    public NavMeshSurface surface;  // navmesh navigation

    public Text time_text; 
    public Text question_text;  
    private float remaining_time;
    private List<Button> math_options; 
    public Button but1; 
    public Button but2; 
    public Button but3; 
    public Button but4; 
    private int correct_answer; 

    //recreate map stuff
    private List<TileType>[,] old_grid;
    private int player_pos_w;
    private int player_pos_l;
    private bool first_pos = false;

    //in game menu
    public GameObject in_game_menu;
    public GameObject math_panel; 
    private bool ingame_menu_active = false;

    private float time_token = 5f; 
    private float health_token = 5f; 
    private bool health_changed = false; 

    //audio component
    public AudioSource audio_com;      
    public AudioClip coin_collide_sound;
    public AudioClip correct_sound; 
    public AudioClip wrong_sound; 
    public AudioClip counting_down_clip; 
    public AudioClip tool_sound;

    public Material wall_mat;

    // fields/variables accessible from other scripts
    internal GameObject fps_player_obj;   // instance of FPS template
    internal bool virus_landed_on_player_recently = false;  // has virus hit the player? if yes, a timer of 5sec starts before infection
    internal float timestamp_virus_landed = float.MaxValue; // timestamp to check how many sec passed since the virus landed on player
    internal bool coin_landed_on_player_recently = false;   // has coin collided with player?
    internal bool time_bot_landed_on_player_recently = false;
    internal bool coin_bot_landed_on_player_recently = false;
    internal bool shield_bot_landed_on_player_recently = false;
    internal float player_health; 

    // fields/variables needed only from this script
    private Bounds bounds;                   // size of ground plane in world space coordinates 
    private float timestamp_last_msg = 0.0f; // timestamp used to record when last message on GUI happened (after 7 sec, default msg appears)
    private int function_calls = 0;          // number of function calls during backtracking for solving the CSP

    // a helper function that randomly shuffles the elements of a list (useful to randomize the solution to the CSP)
    private void Shuffle<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        // initialize internal/private variables
        width = GameData.width; 
        length = GameData.length; 
        bounds = GetComponent<Collider>().bounds; 
        timestamp_last_msg = 0.0f;
        function_calls = 0;
        virus_landed_on_player_recently = false;
        timestamp_virus_landed = float.MaxValue;
        coin_landed_on_player_recently = false;
        in_game_menu.SetActive(false);  
        math_panel.SetActive(false); 
        math_options = new List<Button>(); 
        math_options.Add(but1); 
        math_options.Add(but2); 
        math_options.Add(but3); 
        math_options.Add(but4); 

        score = 0;  
        remaining_time = GameData.total_time;
        UpdateTime();

        // initialize 2D grid
        List<TileType>[,] grid = new List<TileType>[width, length];
        // useful to keep variables that are unassigned so far
        List<int[]> unassigned = new List<int[]>();

        // create the wall perimeter of the level, and let the interior as unassigned
        // then try to assign variables to satisfy all constraints
        // *rarely* it might be impossible to satisfy all constraints due to initialization
        // in this case of no success, we'll restart the random initialization and try to re-solve the CSP
        bool success = false;        
        while (!success)
        {
            for (int w = 0; w < width; w++)
            {
                for (int l = 0; l < length; l++)
                {
                    if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                        grid[w, l] = new List<TileType> { TileType.WALL };
                    else
                    {
                        if (grid[w, l] == null) // does not have virus already or some other assignment from previous run
                        {
                            // CSP will involve assigning variables to one of the following four values (VIRUS is predefined for some tiles)
                            List<TileType> candidate_assignments = new List<TileType> { TileType.WALL, TileType.FLOOR};
                            Shuffle<TileType>(ref candidate_assignments);

                            grid[w, l] = candidate_assignments;
                            unassigned.Add(new int[] { w, l });
                        }
                    }
                }
            }

            success = BackTrackingSearch(grid, unassigned);
            if (!success)
            {
                Debug.Log("Could not find valid solution - will try again");
                unassigned.Clear();
                grid = new List<TileType>[width, length];
                function_calls = 0; 
            }
        }
        old_grid = grid;
        DrawDungeon(grid);
        surface.BuildNavMesh();
    }

    // function to update the time on screen
    private void UpdateTime()
    {
        if (remaining_time <= 0.0f)
        {
            time_text.text = "Remaining time: 0";
        }
        else
        {
            int seconds = Mathf.FloorToInt(remaining_time);
            time_text.text = string.Format("Remaining time: {0}", seconds);
        }

    }

    // function to check whether there is too less wall
    bool DoWeHaveTooFewWalls(List<TileType>[,] grid)
    {
        int[] number_of_potential_assignments = new int[] { 0, 0, 0, 0, 0 };
        for (int w = 0; w < width; w++)
            for (int l = 0; l < length; l++)
            {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                    continue;
                for (int i = 0; i < grid[w, l].Count; i++)
                    number_of_potential_assignments[(int)grid[w, l][i]]++;
            }

        if ((number_of_potential_assignments[(int)TileType.WALL] < (width * length) / 4))
            return true;
        else
            return false;
    }

    // check if attempted assignment is consistent with the constraints or not
    bool CheckConsistency(List<TileType>[,] grid, int[] cell_pos, TileType t)
    {
        int w = cell_pos[0];
        int l = cell_pos[1];

        List<TileType> old_assignment = new List<TileType>();
        old_assignment.AddRange(grid[w, l]);
        grid[w, l] = new List<TileType> { t };

		// note that we negate the functions here i.e., check if we are consistent with the constraints we want
        bool areWeConsistent = !DoWeHaveTooFewWalls(grid);

        grid[w, l] = new List<TileType>();
        grid[w, l].AddRange(old_assignment);
        return areWeConsistent;
    }

    // backtracking 
    bool BackTrackingSearch(List<TileType>[,] grid, List<int[]> unassigned)
    {
        // if there are too many recursive function evaluations, then backtracking has become too slow (or constraints cannot be satisfied)
        // to provide a reasonable amount of time to start the level, we put a limit on the total number of recursive calls
        // if the number of calls exceed the limit, then it's better to try a different initialization
        if (function_calls++ > 100000)       
            return false;

        // we are done!
        if (unassigned.Count == 0)
            return true;

        int randomValue = Random.Range(0, unassigned.Count);  //choose random unassigned value
        int[] assigned = unassigned[randomValue];
        foreach (TileType t in grid[assigned[0], assigned[1]])
        {
            if (CheckConsistency(grid, assigned, t)){
                List<TileType> old_one = new List<TileType>();
                old_one.AddRange(grid[assigned[0], assigned[1]]);
                // head = cell which we assign successfully
                int[] head = unassigned[randomValue];
                grid[assigned[0], assigned[1]] = new List<TileType> { t };
                unassigned.RemoveAt(randomValue);
                bool result = BackTrackingSearch(grid, unassigned);
                if (result)
                {
                    return true;
                }
                else
                {
                    // go back
                    grid[assigned[0], assigned[1]] = old_one;
                    unassigned.Insert(randomValue, head);
                } 
            }
        }
        return false;
    }

    // This function is to check whether the maze is valid
    // to be valid: the maze shall not have any dead ends 
    // 1. for internal cells: 
    //    (1) if this cell of one of the four corners, its two other neighbors (not including the external border) shall be FLOOR as well. 
    //    (2) all other cells: at least two of its four neighbors shall be FLOOR. 

    // 2. the boarder of the grid should be WALL, and remain unchanged. 
    // void maze_generation_v2(ref List<TileType>[,] grid)
    // {
    //     int[] rowOffsets = { -1, 0, 1, 0 };
    //     int[] colOffsets = { 0, 1, 0, -1 };

    //     // for internal cells: 
    //     for (int r = 1; r < width-2; r++)
    //     {
    //         for (int c = 1; c < length-2; c++)
    //         {
    //             // replace all walls with floor on the outbound of the internal walls 
    //             if (r == 1 || r == width - 2 || c == 1 || c == length - 2)
    //             {
    //                 grid[r,c][0] = TileType.FLOOR; 
    //             }
    //             // for all other internal cells: check four neighbors and make sure at least two is FLOOR tiles 
    //             else if (grid[r,c].Count == 1 && !grid[r,c].Contains(TileType.WALL))
    //             {
    //                 int num_of_walls = 0;
    //                 for (int i = 0; i < 4; i++)
    //                 {
    //                     int r_neig = r + rowOffsets[i]; 
    //                     int c_neig = c + rowOffsets[i]; 
                        
    //                     // if neighbor cell is not external wall (border) and the cell is WALL 
    //                     if (!(r_neig == 1 || r_neig == width - 2 || c_neig == 1 || c_neig == length - 2)
    //                     && (grid[r_neig,c_neig].Count == 1 && grid[r_neig,c_neig].Contains(TileType.WALL)))
    //                     {
    //                         num_of_walls ++; 
    //                         if (num_of_walls > 2)
    //                         {
    //                             grid[r_neig,c_neig][0] = TileType.FLOOR; 
    //                             num_of_walls --; 
    //                         }
    //                     } 
    //                 }
    //             }
    //         }
    //     }
    // }


    // This function is to check whether the maze is valid
    // to be valid: the maze shall not have any dead ends 
    // 1. for internal cells: 
    //    (1) if this cell of one of the four corners, its two other neighbors (not including the external border) shall be FLOOR as well. 
    //    (2) all other cells: at least two of its four neighbors shall be FLOOR. 

    // 2. the boarder of the grid should be WALL, and remain unchanged. 
    void maze_generation_v2(ref List<TileType>[,] grid)
    {
        List<int[]> neighbors = new List<int[]>();
        neighbors.Add(new int[] {0,-1});
        neighbors.Add(new int[] {0,1});
        neighbors.Add(new int[] {-1,0});
        neighbors.Add(new int[] {1,0});
        for (int w = 0; w < width; w++)
        {
            for (int l = 0; l < length; l++)
            {
                int num_of_walls=0;
                // 2. for internal cell
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                {    
                    // 2(2) for four corner cell: don't care
                    if ((w == 0 && l == 0) || (w == 0 && l == length - 1) || (w == width - 1 && l == 0) || (w == width - 1 && l == length - 1))  
                    {

                        continue;
                    }  

                    // 2(1) not four corner cell: all its internal neighbors are not WALLWALL
                    else
                    {
                        for (int neiborghs_idx=0; neiborghs_idx<neighbors.Count; neiborghs_idx++)
                        {
                            // neiborgh_x,neiborgh_y: the location of current gird's neiborghs
                            int neiborgh_x = w+neighbors[neiborghs_idx][0];
                            int neiborgh_y = l+neighbors[neiborghs_idx][1];  

                            if ((neiborgh_x>=1)&&(neiborgh_x<width-1)&&(neiborgh_y>=1)&&(neiborgh_y<length-1))
                            {

                                if ((grid[neiborgh_x,neiborgh_y].Count==1)&&(grid[neiborgh_x,neiborgh_y].Contains(TileType.WALL)))
                                {
                                    grid[neiborgh_x,neiborgh_y].Remove(TileType.WALL);
                                    grid[neiborgh_x,neiborgh_y].Add(TileType.FLOOR);                                    
                                }

                            }

                        }  
                    }           

                }
                // 1. for internal cell
                else
                {
                    // 1(1) if this cell is close to two edges: all its internal neighbors are not WALL
                    if ((w == 1 && l == 1) || (w == 1 && l == length - 2) || (w == width - 2 && l == 1) || (w == width - 2 && l == length - 2))  
                    {
                        for (int neiborghs_idx=0; neiborghs_idx<neighbors.Count; neiborghs_idx++)
                        {
                            // neiborgh_x,neiborgh_y: the location of current gird's neiborghs
                            int neiborgh_x = w+neighbors[neiborghs_idx][0];
                            int neiborgh_y = l+neighbors[neiborghs_idx][1];  

                            if ((neiborgh_x>=1)&&(neiborgh_x<width-1)&&(neiborgh_y>=1)&&(neiborgh_y<length-1))
                            {

                                if ((grid[neiborgh_x,neiborgh_y].Count==1)&&(grid[neiborgh_x,neiborgh_y].Contains(TileType.WALL)))
                                {
                                    grid[neiborgh_x,neiborgh_y].Remove(TileType.WALL);
                                    grid[neiborgh_x,neiborgh_y].Add(TileType.FLOOR);                                    
                                }

                            }

                        }   
                    }

                    // 1(2) if this cell is close to zero edges: there are at most two internal neighbors are WALL
                    else
                    {
                        for (int neiborghs_idx=0; neiborghs_idx<neighbors.Count; neiborghs_idx++)
                        {
                            // neiborgh_x,neiborgh_y: the location of current gird's neiborghs
                            int neiborgh_x = w+neighbors[neiborghs_idx][0];
                            int neiborgh_y = l+neighbors[neiborghs_idx][1];  

                            if ((neiborgh_x>=1)&&(neiborgh_x<width-1)&&(neiborgh_y>=1)&&(neiborgh_y<length-1))
                            {
                                if ((grid[neiborgh_x,neiborgh_y].Count==1)&&(grid[neiborgh_x,neiborgh_y].Contains(TileType.WALL)))
                                {
                                    num_of_walls += 1;
                                }

                                if (num_of_walls > 2)
                                {
                                    grid[neiborgh_x,neiborgh_y].Remove(TileType.WALL);
                                    grid[neiborgh_x,neiborgh_y].Add(TileType.FLOOR);
                                    num_of_walls-=1;

                                }
                            }
                        }                          
                    }
                }
            }
        }  
    }

    void GenerateMathProblem()
    {
        int num1 = Random.Range(0, 20); 
        int num2 = Random.Range(0, 20); 
        bool is_add = Random.Range(0, 2) == 0; 
        if (is_add)
        {
            correct_answer = num1 + num2; 
            question_text.text = $"How much is {num1} + {num2}?";
        }
        else 
        {
            correct_answer = num1 - num2; 
            question_text.text = $"How much is {num1} - {num2}?";
        }

        int[] options = GenerateOptions(correct_answer); 
        for (int i = 0; i < math_options.Count; i++)
        {
            if (math_options[i] != null)
            {
                TextMeshProUGUI text_but = math_options[i].GetComponentInChildren<TextMeshProUGUI>(); 
                if (text_but != null)
                    text_but.text = options[i].ToString();
            }
            int option = options[i]; 
            math_options[i].onClick.AddListener(() => ChooseOption(option)); 
        }
    }

    int[] GenerateOptions(int correct_answer)
    {
        int[] options = new int[4]; 
        int correct_index = Random.Range(0, 4);
        for (int i = 0; i < 4; i++)
        {
            if (i == correct_index)
                options[i] = correct_answer; 
            else
            {
                do
                {
                    options[i] = Random.Range(-20, 40); 
                }
                while (options[i] == correct_answer || System.Array.IndexOf(options, options[i]) < i);
            }
        } 
        return options; 
    }

    void ChooseOption(int selected)
    {
        if (selected == correct_answer)
        {
            score += 6; 
            audio_com.clip = correct_sound;
            audio_com.Play();
            text_box.GetComponent<Text>().text = "Correct! Nicely done!";
            timestamp_last_msg = Time.time;
        }
        else 
        {
            score -= 4; 
            audio_com.clip = wrong_sound;
            audio_com.Play();
            text_box.GetComponent<Text>().text = "Oops, wrong answer~";
            timestamp_last_msg = Time.time;
        }
        score_text.text = "Score: " + score; 
        math_panel.SetActive(false); 
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 

        foreach (var but in math_options)
            but.onClick.RemoveAllListeners(); 
    }

    // places the primitives/objects according to the grid assignents
    // you will need to edit this function (see below)
    void DrawDungeon(List<TileType>[,] solution)
    {
        GetComponent<Renderer>().material.color = Color.grey; // ground plane will be grey

        // place character at random position (wr, lr) in terms of grid coordinates (integers)
        // make sure that this random position is a FLOOR tile (not wall, coin, or virus)

        int wr = 0;
        int lr = 0;

        int wrBrownSpider = 0;
        int lrBrownSpider = 0;

        int wrGreenSpider = 0;
        int lrGreenSpider = 0;


        GameObject brown_spider = GameObject.Find("spider_brown");
        GameObject green_spider = GameObject.Find("spider_green");

        if (!first_pos)  // try same map case, I will generate player position once, we don't need to generate again
        {
            while (true) // try until a valid position is sampled
            {
                wr = Random.Range(1, width - 1);
                lr = Random.Range(1, length - 1);
                // randomPoint = Random.Range(0, spawnPoint.Count);
                // wr = spawnPoint[randomPoint][0];
                // lr = spawnPoint[randomPoint][1];

                if (solution[wr, lr][0] == TileType.FLOOR)
                {
                    // Debug.Log("x index of spawn point is "+wr);
                    // Debug.Log("y index of spawn point is "+lr);
                    float x = bounds.min[0] + (float)wr * (bounds.size[0] / (float)width);
                    float z = bounds.min[2] + (float)lr * (bounds.size[2] / (float)length);
                    fps_player_obj = Instantiate(fps_prefab);
                    fps_player_obj.name = "PLAYER";
                    fps_player_obj.tag = "Tile";
                    fps_player_obj.layer = 3;
                    // character is placed above the level so that in the beginning, he appears to fall down onto the maze
                    fps_player_obj.transform.position = new Vector3(x + 0.5f, 2.0f * storey_height, z + 0.5f);
                    //BoxCollider boxCollider = fps_player_obj.AddComponent<BoxCollider>();

                    // fps_player_obj.AddComponent<AudioSource>();  //add audioSource component
                    audio_com = fps_player_obj.GetComponent<AudioSource>();
                    player_pos_w = wr;
                    player_pos_l = lr;
                    //first_pos = true;
                    break;
                } 
            }         

            Debug.Log("Player's spawn point are x =  " + wr + ", y = " + lr); 
            Debug.Log("The tiletype of this cell is floor: "+(solution[wr, lr][0] == TileType.FLOOR));

            while (true) // try until a valid position is sampled
            {
                wrBrownSpider = Random.Range(1, width - 1);
                lrBrownSpider = Random.Range(1, length - 1);

                do
                {

                    wrBrownSpider = Random.Range(1, width - 1);
                    lrBrownSpider = Random.Range(1, length - 1);

                } while ((wr == wrBrownSpider)&&(lr == lrBrownSpider));

                if (solution[wrBrownSpider, lrBrownSpider][0] == TileType.FLOOR)
                {
                    //Debug.Log("Brown Spider's spawn point are x =  "+wrBrownSpider+", y = "+lrBrownSpider);
                    
                    float x = bounds.min[0] + (float)wrBrownSpider * (bounds.size[0] / (float)width);
                    float z = bounds.min[2] + (float)lrBrownSpider * (bounds.size[2] / (float)length);    
                    brown_spider.transform.position = new Vector3(x + 0.5f, 0.037217f, z + 0.5f); 
                    break;          
                }
            }
  
            Debug.Log("Brown Spider's spawn point are x =  " + wrBrownSpider + ", y = " + lrBrownSpider); 
            Debug.Log("The tiletype of this cell is floor: " + (solution[wrBrownSpider, lrBrownSpider][0] == TileType.FLOOR));


            while (true) // try until a valid position is sampled
            {
                wrGreenSpider = Random.Range(1, width - 1);
                lrGreenSpider = Random.Range(1, length - 1);

                do
                {

                    wrGreenSpider = Random.Range(1, width - 1);
                    lrGreenSpider = Random.Range(1, length - 1);

                } while (((wr == wrGreenSpider)&&(lr == lrGreenSpider)) || ((wrBrownSpider == wrGreenSpider)&&(lrBrownSpider == lrGreenSpider)));

                if (solution[wrGreenSpider, lrGreenSpider][0] == TileType.FLOOR)
                {
                    //Debug.Log("Green Spider's spawn point are x =  "+wrGreenSpider+", y = "+lrGreenSpider);
                    
                    float x = bounds.min[0] + (float)wrGreenSpider * (bounds.size[0] / (float)width);
                    float z = bounds.min[2] + (float)lrGreenSpider * (bounds.size[2] / (float)length);    
                    green_spider.transform.position = new Vector3(x + 0.5f, 0.037217f, z + 0.5f); 
                    break;          
                }
            }

            Debug.Log("Green Spider's spawn point are x =  " + wrGreenSpider + ", y = " + lrGreenSpider);   
            Debug.Log("The tiletype of this cell is floor: " + (solution[wrGreenSpider, lrGreenSpider][0] == TileType.FLOOR));  


            first_pos = true;
        }
        




        

        maze_generation_v2(ref solution);
        Debug.Log("Maze is generated successfully!");

        // the rest of the code creates the scenery based on the grid state 
        // you don't need to modify this code (unless you want to replace the virus
        // or other prefabs with something else you like)
        int w = 0;
        for (float x = bounds.min[0]; x < bounds.max[0]; x += bounds.size[0] / (float)width - 1e-6f, w++)
        {
            int l = 0;
            for (float z = bounds.min[2]; z < bounds.max[2]; z += bounds.size[2] / (float)length - 1e-6f, l++)
            {
                if ((w >= width) || (l >= width))
                    continue;

                float y = bounds.min[1];
                if (solution[w, l][0] == TileType.WALL)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "WALL";
                    if (w != 0 && l != 0 && w != width - 1 && l != length - 1)
                    {
                        cube.tag = "Tile";
                    }
                    cube.transform.localScale = new Vector3(bounds.size[0] / (float)width, storey_height, bounds.size[2] / (float)length);
                    cube.transform.position = new Vector3(x + 0.5f, y + storey_height / 2.0f, z + 0.5f);
                    //cube.GetComponent<Renderer>().material.color = new Color(0.6f, 0.8f, 0.8f);
                    cube.GetComponent<Renderer>().material = wall_mat;
                }
                else if (solution[w, l][0] == TileType.FLOOR)
                {
                    int rand = Random.Range(0, 50); 
                    if (rand == 0)
                    {
                        GameObject time_bottle = Instantiate(time_prefab, new Vector3(0, 0, 0), Quaternion.identity); 
                        time_bottle.AddComponent<NavMeshModifier>();
                        time_bottle.GetComponent<NavMeshModifier>().ignoreFromBuild = true;
                        time_bottle.name = "TIME_BOT";
                        time_bottle.tag = "Tile";
                        time_bottle.transform.position = new Vector3(x + 0.5f, y + 0.1f, z + 0.5f);
                        time_bottle.transform.localScale = new Vector3(1f, 1f, 1f);

                        BoxCollider boxCollider = time_bottle.AddComponent<BoxCollider>();
                        time_bottle.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f);
                        time_bottle.GetComponent<BoxCollider>().center = new Vector3(0f, 1f, 0f);
                        time_bottle.GetComponent<BoxCollider>().isTrigger = true;
                        time_bottle.AddComponent<BotTime>();
                    }
                    else if (rand == 1)
                    {
                        GameObject coin_bottle = Instantiate(magn_prefab, new Vector3(0, 0, 0), Quaternion.identity); 
                        coin_bottle.AddComponent<NavMeshModifier>();
                        coin_bottle.GetComponent<NavMeshModifier>().ignoreFromBuild = true;
                        coin_bottle.name = "COIN_BOT";
                        coin_bottle.tag = "Tile";
                        coin_bottle.transform.position = new Vector3(x + 0.5f, y + 0.1f, z + 0.5f);
                        coin_bottle.transform.localScale = new Vector3(1f, 1f, 1f);

                        BoxCollider boxCollider = coin_bottle.AddComponent<BoxCollider>();
                        coin_bottle.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f);
                        coin_bottle.GetComponent<BoxCollider>().center = new Vector3(0f, 1f, 0f);
                        coin_bottle.GetComponent<BoxCollider>().isTrigger = true;
                        coin_bottle.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f) * 0.05f;
                        coin_bottle.GetComponent<BoxCollider>().isTrigger = true;
                        coin_bottle.AddComponent<BotCoin>();
                    }
                    else if (rand == 2)
                    {
                        GameObject shield_bottle = Instantiate(shield_prefab, new Vector3(0, 0, 0), Quaternion.identity); 
                        shield_bottle.AddComponent<NavMeshModifier>();
                        shield_bottle.GetComponent<NavMeshModifier>().ignoreFromBuild = true;
                        shield_bottle.name = "SHIELD_BOT";
                        shield_bottle.tag = "Tile";
                        shield_bottle.transform.position = new Vector3(x + 0.5f, y + 0.1f, z + 0.5f);
                        shield_bottle.transform.localScale = new Vector3(1f, 1f, 1f);

                        BoxCollider boxCollider = shield_bottle.AddComponent<BoxCollider>();
                        shield_bottle.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f);
                        shield_bottle.GetComponent<BoxCollider>().center = new Vector3(0f, 1f, 0f);
                        shield_bottle.GetComponent<BoxCollider>().isTrigger = true;
                        shield_bottle.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f) * 0.05f;
                        shield_bottle.GetComponent<BoxCollider>().isTrigger = true;
                        shield_bottle.AddComponent<BotShield>();
                    }
                    else if (rand == 3)
                    {
                        GameObject rand_box = Instantiate(rand_prefab, new Vector3(0, 0, 0), Quaternion.identity); 
                        rand_box.AddComponent<NavMeshModifier>();
                        rand_box.GetComponent<NavMeshModifier>().ignoreFromBuild = true;
                        rand_box.name = "RAND_BOX";
                        rand_box.tag = "Tile";
                        rand_box.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);
                        rand_box.transform.localScale = new Vector3(1f, 1f, 1f);

                        BoxCollider boxCollider = rand_box.AddComponent<BoxCollider>();
                        rand_box.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f);
                        rand_box.GetComponent<BoxCollider>().center = new Vector3(0f, 0.5f, 0f);
                        rand_box.GetComponent<BoxCollider>().isTrigger = true;
                        rand_box.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f) * 0.05f;
                        rand_box.GetComponent<BoxCollider>().isTrigger = true;
                        int select_bot = Random.Range(0, 3); 
                        if (select_bot == 0)
                            rand_box.AddComponent<BotTime>();
                        else if (select_bot == 1)
                            rand_box.AddComponent<BotCoin>();
                        else if (select_bot == 2)
                            rand_box.AddComponent<BotShield>();
                    }
                    else
                    {
                        GameObject coin = Instantiate(coin_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                        coin.AddComponent<NavMeshModifier>();
                        coin.GetComponent<NavMeshModifier>().ignoreFromBuild = true;
                        coin.name = "COIN";
                        coin.tag = "Tile";
                        coin.transform.position = new Vector3(x + 0.5f, y + storey_height / 2.0f, z + 0.5f);
                        coin.transform.localScale = new Vector3(10f, 10f, 10f);
                        coin.transform.Rotate(90,0,0);
                        
                        MeshRenderer coinRenderer = coin.GetComponentInChildren<MeshRenderer>();
                        coinRenderer.material = coin_mat;

                        BoxCollider boxCollider = coin.AddComponent<BoxCollider>();
                        coin.GetComponent<BoxCollider>().size = new Vector3(0.05f, 0.01f, 0.05f);
                        coin.GetComponent<BoxCollider>().isTrigger = true;
                        coin.AddComponent<Coin>();
                    }
                }
            }
        }

        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        if (remaining_time > 0 && player_health > 0)
        {

            // Update the time
            if (!ingame_menu_active){
                remaining_time -= Time.deltaTime;
                UpdateTime();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ingame_menu_active = !ingame_menu_active;
                if(ingame_menu_active)
                {
                    in_game_menu.gameObject.SetActive(true);
                    Cursor.visible = true; 
                    Cursor.lockState = CursorLockMode.None; 
                }
                else
                {
                    in_game_menu.gameObject.SetActive(false);
                    Cursor.visible = false; 
                    Cursor.lockState = CursorLockMode.Locked; 
                }
            }

            if (Time.time - timestamp_last_msg > 3.0f) // renew the msg by restating the initial goal
            {
                text_box.GetComponent<Text>().text = "";     
            }

            // coin picked by the player  (boolean variable is manipulated by Coin.cs)
            if (coin_landed_on_player_recently)
            {
                audio_com.clip = coin_collide_sound;
                audio_com.Play();

                score += single_score;
                score_text.text = "Score: " + score;  // update score text
                text_box.GetComponent<Text>().text = "You've got a coin!";

                if (score % 10 == 0)
                {
                    Cursor.visible = true; 
                    Cursor.lockState = CursorLockMode.None; 
                    GenerateMathProblem(); 
                    math_panel.SetActive(true); 
                }

                timestamp_last_msg = Time.time;
                coin_landed_on_player_recently = false;
            }

            if (time_bot_landed_on_player_recently)
            {
                remaining_time += time_token; 
                text_box.GetComponent<Text>().text = $"{time_token} seconds have been added to your time!";
                timestamp_last_msg = Time.time;
                time_bot_landed_on_player_recently = false;
                audio_com.clip = tool_sound;
                audio_com.time = 1.5f;
                audio_com.Play();
                audio_com.SetScheduledEndTime(AudioSettings.dspTime + 1.5f);  //set drug audio
            }
            // Tool 1:
            // (1) usage: press key 1
            // (2) result: coins within the radius of 10.0f of player are destroied, and update score by adding (num_of_attracted_coins*single_score)
            if (coin_bot_landed_on_player_recently)
            {
                int num_of_attracted_coins=0;
                
                Collider[] colliders = Physics.OverlapSphere(fps_player_obj.transform.position, 10.0f);
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.name == "COIN")
                    {
                        score += single_score;
                        score_text.text = "Score: " + score;  // update score text
                        num_of_attracted_coins += 1;
                        Destroy(collider.gameObject);
                    }
                }

                text_box.GetComponent<Text>().text = $"You've attracted {num_of_attracted_coins} coins!";
                timestamp_last_msg = Time.time;
                coin_bot_landed_on_player_recently = false;
                audio_com.clip = tool_sound;
                audio_com.time = 1.5f;
                audio_com.Play();
                audio_com.SetScheduledEndTime(AudioSettings.dspTime + 1.5f);  //set drug audio
            }
            if (shield_bot_landed_on_player_recently)
            { 
                player_health += health_token; 
                health_changed = true; 
                text_box.GetComponent<Text>().text = $"{health_token}hp have been added to your health!";
                timestamp_last_msg = Time.time;
                shield_bot_landed_on_player_recently = false;
                audio_com.clip = tool_sound;
                audio_com.time = 1.5f;
                audio_com.Play();
                audio_com.SetScheduledEndTime(AudioSettings.dspTime + 1.5f);  //set drug audio
            }
        }
        else
        {
            if (fps_player_obj != null)
            {
                Object.Destroy(fps_player_obj);
                GameObject camera = GameObject.Find("Main Camera");
                // camera.GetComponent<AudioSource>().clip = enter_house_sound;
                camera.GetComponent<AudioListener>().enabled = true;
                camera.GetComponent<AudioSource>().Play();
                // audio_com.SetScheduledEndTime(AudioSettings.dspTime + 2f);
                // win case: in_game_menu setting
                math_panel.gameObject.SetActive(false); 
                in_game_menu.gameObject.SetActive(true);
                ingame_menu_active = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                camera.GetComponent<AudioListener>().enabled = false;

                if (is_win(width))
                {
                    game_text.GetComponent<Text>().text = "CONGRATULATIONS\nYOU WIN!"; 
                } 
                else if (player_health <= 0)
                {
                    game_text.GetComponent<Text>().text = "Oops, you ran out of health.\nLet's try it again!"; 
                }
                else if (remaining_time <= 0)
                {
                    game_text.GetComponent<Text>().text = "Oops, you ran out of time.\nLet's try it again!"; 
                }
            }
        }

    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(2.0f);
    }

    private float heuristic(int w, int l, int[] goal)
    {
        //Euclidean distance
        float x = (float)Mathf.Pow(w - goal[0], 2) + (float)Mathf.Pow(l - goal[1], 2);
        return (float)Mathf.Sqrt(x);
    }

    // private Result A_star_search(List<TileType>[,] solution, int start_pos_w, int start_pos_l)
    // {
    //     // initial 
    //     int[] house_pos = { house_pos_w, house_pos_l };
    //     Node[,] a_star_map = new Node[width, length];
    //     for (int i = 0; i < width; i++)
    //     {
    //         for (int j = 0; j < length; j++)
    //         {
    //             int[] pp = { i, j };
    //             a_star_map[i, j] = new Node(pp);
    //         }
    //     }
    //     //
    //     PriorityQueue Q = new PriorityQueue();
    //     int[] start_pos = { start_pos_w, start_pos_l };
    //     Debug.Log("house_pos" + house_pos[0] + " " + house_pos[1]);
    //     //start position 
    //     a_star_map[start_pos_w, start_pos_l].cell = start_pos;
    //     a_star_map[start_pos_w, start_pos_l].gScore = 0;
    //     a_star_map[start_pos_w, start_pos_l].hScore = heuristic(start_pos[0], start_pos[1], house_pos);
    //     a_star_map[start_pos_w, start_pos_l].fScore =
    //         a_star_map[start_pos_w, start_pos_l].gScore + a_star_map[start_pos_w, start_pos_l].hScore;
    //     Q.Enqueue(a_star_map[start_pos_w, start_pos_l]);
    //     // collect close heuristic position
    //     float close_heuristic_pos = heuristic(start_pos[0], start_pos[1], house_pos);
    //     int close_w = start_pos_w;
    //     int close_l = start_pos_l;

    //     while (!Q.IsEmpty())
    //     {   
    //         Node current = Q.Peek();
    //         int[] current_pos = current.cell;

    //         if (current.cell[0] == house_pos[0] && current.cell[1] == house_pos[1])
    //         {
    //             return new Result { result = true, w = -1, l = -1};  // reach the house position
    //         }
    //         // update the closest position during the search process
    //         float current_h = heuristic(current_pos[0], current_pos[1], house_pos);
    //         if (current_h < close_heuristic_pos)
    //         {
    //             close_heuristic_pos = current_h;
    //             close_w = current_pos[0];
    //             close_l = current_pos[1];
    //         }
    //         Q.Dequeue();
    //         int[][] neighbor = neighbors(current_pos);
    //         for (int g = 0; g < 4; g++)
    //         {
    //             int i = neighbor[g][0]; // neighbor w position
    //             int j = neighbor[g][1]; // neighbor l position
    //             if (i == house_pos[0] && j == house_pos[1]) // special case for neighbor is house but is in wall tile
    //             {
    //                 float gScore = current.gScore + heuristic(i, j, current_pos);
    //                 if (gScore < a_star_map[i, j].gScore)
    //                 {  // found better path
    //                     a_star_map[i, j].Previous = current;
    //                     a_star_map[i, j].gScore = gScore; // update G and F
    //                     a_star_map[i, j].hScore = heuristic(i, j, house_pos);
    //                     a_star_map[i, j].fScore = gScore + heuristic(i, j, house_pos);
    //                     if (!Q.cell_in_queue(i, j))
    //                     {
    //                         Q.Enqueue(a_star_map[i, j]);
    //                     }
    //                 }
    //             }
    //             else if (!solution[i, j].Contains(TileType.WALL)) // general case: don't check wall
    //             {
    //                 float gScore = current.gScore + heuristic(i, j, current_pos);
    //                 if (gScore < a_star_map[i, j].gScore)
    //                 {  // found better path
    //                     a_star_map[i, j].gScore = gScore; // update G and F
    //                     a_star_map[i, j].fScore = gScore + heuristic(i, j, house_pos);
    //                     a_star_map[i, j].hScore = heuristic(i, j, house_pos);
    //                     if (!Q.cell_in_queue(i, j))
    //                     {
    //                         Q.Enqueue(a_star_map[i, j]);
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     return new Result {result = false, w = close_w, l = close_l };
    // }

    private List<Node> reconstruct_path(Node goal)
    {
        var path = new List<Node>();
        Node current = goal;

        while (current != null)
        {
            path.Insert(0, current);
            current = current.Previous;
        }

        return path;
    }

    private int[][] neighbors(int[] cur_position)
    {
        int[][] jaggedArray = new int[4][];
        jaggedArray[0] = new int[] { cur_position[0] - 1, cur_position[1] };
        jaggedArray[1] = new int[] { cur_position[0] + 1, cur_position[1] };
        jaggedArray[2] = new int[] { cur_position[0], cur_position[1] - 1 };
        jaggedArray[3] = new int[] { cur_position[0], cur_position[1] + 1};
        return jaggedArray;
    }

    public void try_same_map() // renew game status for same map, check comments for detail
    {
        GameObject[] existingTiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (var tile in existingTiles)
        {
            Destroy(tile); // destroy generated objects except out wall and house
        }
        // renew the map and player status
        timestamp_last_msg = 0.0f;
        function_calls = 0;
        timestamp_virus_landed = float.MaxValue;
        coin_landed_on_player_recently = false;
        player_health = 100; 
        health_changed = true; 

        float x = bounds.min[0] + (float)player_pos_w * (bounds.size[0] / (float)width);
        float z = bounds.min[2] + (float)player_pos_l * (bounds.size[2] / (float)length);
        fps_player_obj = Instantiate(fps_prefab);
        fps_player_obj.name = "PLAYER";
        fps_player_obj.tag = "Tile";

        // character is placed above the level so that in the beginning, he appears to fall down onto the maze
        fps_player_obj.transform.position = new Vector3(x + 0.5f, 2.0f * storey_height, z + 0.5f);
        //BoxCollider boxCollider = fps_player_obj.AddComponent<BoxCollider>();

        // fps_player_obj.AddComponent<AudioSource>();  //add audioSource component
        // fps_player_obj.GetComponent<AudioSource>().enabled = true;
        audio_com = fps_player_obj.GetComponent<AudioSource>();
        in_game_menu.gameObject.SetActive(false);
        math_panel.SetActive(false); 
        ingame_menu_active = false;

        DrawDungeon(old_grid);
    }

    public void set_health(float health)
    {
        player_health = health; 
    }

    public void set_health_changed()
    {
        health_changed = !health_changed; 
    }

    public float get_health()
    {
        return player_health; 
    }

    public bool is_health_changed()
    {
        return health_changed; 
    }

    private bool is_win(int difficulty)
    {
        if (score >= difficulty * 10)
            return true; 
        return false; 
    }

}

