using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MyNamespace;
//using UnityEngine.UIElements;

// enum TileType
// {
//     WALL = 0,
//     FLOOR = 1,
//     WATER = 2,
//     COIN = 3,
//     VIRUS = 4,
// }
// public class Result
// {
//     public bool result { get; set; }
//     public int w { get; set; }
//     public int l { get; set; }
// }

public class Level : MonoBehaviour
{
//     // fields/variables you may adjust from Unity's interface
//     public int width = 16;   // size of level (default 16 x 16 blocks)
//     public int length = 16;
//     public float storey_height = 2.5f;   // height of walls
//     public float virus_speed = 3.0f;     // virus velocity
//     public GameObject fps_prefab;        // these should be set to prefabs as provided in the starter scene
//     public GameObject virus_prefab;
//     public GameObject coin_prefab;
//     public GameObject water_prefab;
//     public GameObject house_prefab;
//     public GameObject text_box;
//     public GameObject scroll_bar;
//     public Material coin_mat; 

//     //recreate map stuff
//     private List<TileType>[,] old_grid;
//     private bool house_exist = false;
//     private int player_pos_w;
//     private int player_pos_l;
//     private bool first_pos = false;
//     private int house_pos_w = -1;
//     private int house_pos_l = -1;
//     //in game menu
//     public GameObject panel;
//     private bool ingame_menu_active = false;

//     //audio component
//     public AudioSource audio_com;      
//     public AudioClip virus_collide_sound;
//     public AudioClip water_collide_sound;
//     public AudioClip coin_collide_sound;
//     public AudioClip lose_health_sound;
//     public AudioClip enter_house_sound;
//     private bool hasPlayed_forHouse = false;

//     public Material cubeMaterial;

//     // fields/variables accessible from other scripts
//     internal GameObject fps_player_obj;   // instance of FPS template
//     internal float player_health = 1.0f;  // player health in range [0.0, 1.0]
//     internal int num_virus_hit_concurrently = 0;            // how many viruses hit the player before washing them off
//     internal bool virus_landed_on_player_recently = false;  // has virus hit the player? if yes, a timer of 5sec starts before infection
//     internal float timestamp_virus_landed = float.MaxValue; // timestamp to check how many sec passed since the virus landed on player
//     internal bool coin_landed_on_player_recently = false;   // has coin collided with player?
//     internal bool player_is_on_water = false;               // is player on water block
//     internal bool player_entered_house = false;             // has player arrived in house?

//     // fields/variables needed only from this script
//     private Bounds bounds;                   // size of ground plane in world space coordinates 
//     private float timestamp_last_msg = 0.0f; // timestamp used to record when last message on GUI happened (after 7 sec, default msg appears)
//     private int function_calls = 0;          // number of function calls during backtracking for solving the CSP
//     private int num_viruses = 0;             // number of viruses in the level
//     private List<int[]> pos_viruses;         // stores their location in the grid

//     // feel free to put more fields here, if you need them e.g, add AudioClips that you can also reference them from other scripts
//     // for sound, make also sure that you have ONE audio listener active (either the listener in the FPS or the main camera, switch accordingly)

//     // a helper function that randomly shuffles the elements of a list (useful to randomize the solution to the CSP)
//     private void Shuffle<T>(ref List<T> list)
//     {
//         int n = list.Count;
//         while (n > 1)
//         {
//             n--;
//             int k = Random.Range(0, n + 1);
//             T value = list[k];
//             list[k] = list[n];
//             list[n] = value;
//         }
//     }

//     // Use this for initialization
//     void Start()
//     {
//         // initialize internal/private variables
//         //Random.InitState(seed);
//         bounds = GetComponent<Collider>().bounds; 
//         timestamp_last_msg = 0.0f;
//         function_calls = 0;
//         num_viruses = 0;
//         player_health = 1.0f;
//         num_virus_hit_concurrently = 0;
//         virus_landed_on_player_recently = false;
//         timestamp_virus_landed = float.MaxValue;
//         coin_landed_on_player_recently = false;
//         player_is_on_water = false;
//         player_entered_house = false;    
//         panel.SetActive(false);     

//         // initialize 2D grid
//         List<TileType>[,] grid = new List<TileType>[width, length];
//         // useful to keep variables that are unassigned so far
//         List<int[]> unassigned = new List<int[]>();

//         // will place x viruses in the beginning (at least 1). x depends on the sise of the grid (the bigger, the more viruses)        
//         num_viruses = width * length / 25 + 1;  // at least one virus will be added

//         pos_viruses = new List<int[]>();
//         // create the wall perimeter of the level, and let the interior as unassigned
//         // then try to assign variables to satisfy all constraints
//         // *rarely* it might be impossible to satisfy all constraints due to initialization
//         // in this case of no success, we'll restart the random initialization and try to re-solve the CSP
//         bool success = false;        
//         while (!success)
//         {
//             for (int v = 0; v < num_viruses; v++)
//             {
//                 while (true) // try until virus placement is successful (unlikely that there will no places)
//                 {
//                     // try a random location in the grid
//                     int wr = Random.Range(1, width - 1);
//                     int lr = Random.Range(1, length - 1);

//                     // if grid location is empty/free, place it there
//                     if (grid[wr, lr] == null)
//                     {
//                         grid[wr, lr] = new List<TileType> { TileType.VIRUS };
//                         pos_viruses.Add(new int[2] { wr, lr });
//                         break;
//                     }
//                 }
//             }

//             for (int w = 0; w < width; w++)
//                 for (int l = 0; l < length; l++)
//                     if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
//                         grid[w, l] = new List<TileType> { TileType.WALL };
//                     else
//                     {
//                         if (grid[w, l] == null) // does not have virus already or some other assignment from previous run
//                         {
//                             // CSP will involve assigning variables to one of the following four values (VIRUS is predefined for some tiles)
//                             List<TileType> candidate_assignments = new List<TileType> { TileType.WALL, TileType.FLOOR,TileType.WATER, TileType.COIN };
//                             Shuffle<TileType>(ref candidate_assignments);

//                             grid[w, l] = candidate_assignments;
//                             unassigned.Add(new int[] { w, l });
//                         }
//                     }

//             // YOU MUST IMPLEMENT this function!!!
//             success = BackTrackingSearch(grid, unassigned);
//             if (!success)
//             {
//                 Debug.Log("Could not find valid solution - will try again");
//                 unassigned.Clear();
//                 grid = new List<TileType>[width, length];
//                 function_calls = 0; 
//             }
//         }
//         old_grid = grid;
//         DrawDungeon(grid);
//     }

//     // one type of constraint already implemented for you
//     bool DoWeHaveTooManyInteriorWallsORWaterORCoin(List<TileType>[,] grid)
//     {
//         int[] number_of_assigned_elements = new int[] { 0, 0, 0, 0, 0 };
//         for (int w = 0; w < width; w++)
//             for (int l = 0; l < length; l++)
//             {
//                 if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
//                     continue;
//                 if (grid[w, l].Count == 1)
//                     number_of_assigned_elements[(int)grid[w, l][0]]++;
//             }

//         if (// (number_of_assigned_elements[(int)TileType.WALL] > num_viruses * 7) || 
//              (number_of_assigned_elements[(int)TileType.WATER] > (width + length) / 4)) //||  // change here to increase #coins
//              //(number_of_assigned_elements[(int)TileType.COIN] >= num_viruses / 2))
//             return true;
//         else
//             return false;
//     }

//     // another type of constraint already implemented for you
//     bool DoWeHaveTooFewWallsORWaterORCoin(List<TileType>[,] grid)
//     {
//         int[] number_of_potential_assignments = new int[] { 0, 0, 0, 0, 0 };
//         for (int w = 0; w < width; w++)
//             for (int l = 0; l < length; l++)
//             {
//                 if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
//                     continue;
//                 for (int i = 0; i < grid[w, l].Count; i++)
//                     number_of_potential_assignments[(int)grid[w, l][i]]++;
//             }

//         if ((number_of_potential_assignments[(int)TileType.WALL] < (width * length) / 2) ||
//              (number_of_potential_assignments[(int)TileType.WATER] < num_viruses / 4) ||
//              (number_of_potential_assignments[(int)TileType.COIN] < num_viruses / 4))
//             return true;
//         else
//             return false;
//     }

//     // *** YOU NEED TO COMPLETE THIS FUNCTION  ***
//     // must return true if there are three (or more) interior consecutive wall blocks either horizontally or vertically
//     // by interior, we mean walls that do not belong to the perimeter of the grid
//     // e.g., a grid configuration: "FLOOR - WALL - WALL - WALL - FLOOR" is not valid
//     bool tooLongWallHelp(List<TileType>[,] grid, int w, int l, string s)
//     {
//         if(s == "left")
//         {
//             if (grid[w - 1, l].Count == 1 && grid[w - 2, l].Count == 1)
//             {
//                 if (grid[w - 1, l].Contains(TileType.WALL) && grid[w - 2, l].Contains(TileType.WALL))
//                 {
//                     return true;
//                 }
//             }
//         }
//         else if (s == "right")
//         {
//             if (grid[w + 1, l].Count == 1 && grid[w + 2, l].Count == 1)
//             {
//                 if (grid[w + 1, l].Contains(TileType.WALL) && grid[w + 2, l].Contains(TileType.WALL))
//                 {
//                     return true;
//                 }
//             }
//         }
//         else if (s == "up")
//         {
//             if (grid[w, l - 1].Count == 1 && grid[w, l - 2].Count == 1)
//             {
//                 if (grid[w, l - 1].Contains(TileType.WALL) && grid[w, l - 2].Contains(TileType.WALL))
//                 {
//                     return true;
//                 }
//             }
//         }
//         else if (s == "down")
//         {
//             if (grid[w, l + 1].Count == 1 && grid[w, l + 2].Count == 1)
//             {
//                 if (grid[w, l + 1].Contains(TileType.WALL) && grid[w, l + 2].Contains(TileType.WALL))
//                 {
//                     return true;
//                 }
//             }
//         }
//         return false;
//     }
//     bool TooLongWall(List<TileType>[,] grid)
//     {
//         /*** implement the rest ! */
//         for (int w = 1; w < width - 1; w++)
//         {
//             for(int l = 1; l < length - 1; l++)
//             {
//                 if ((grid[w, l].Count == 1) && grid[w, l].Contains(TileType.WALL))
//                 {
//                     // only check left
//                     if(w >= width - 3)
//                     {
//                         if(tooLongWallHelp(grid, w, l, "left")) return true;
//                     }
//                     // only check right
//                     else if (w <= 2)
//                     {
//                         if (tooLongWallHelp(grid, w, l, "right")) return true;
//                     }
//                     // check both left and right
//                     else
//                     {
//                         if (tooLongWallHelp(grid, w, l, "right") || tooLongWallHelp(grid, w, l, "left"))
//                             return true;
//                     }
//                     // check up
//                     if (l >= length - 3)
//                     {
//                         if (tooLongWallHelp(grid, w, l, "up")) return true;
//                     }
//                     // only check down
//                     else if (l <= 2)
//                     {
//                         if (tooLongWallHelp(grid, w, l, "down")) return true;
//                     }
//                     //check both up and down
//                     else
//                     {
//                         if (tooLongWallHelp(grid, w, l, "up") || tooLongWallHelp(grid, w, l, "down"))
//                             return true;
//                     }
//                 }
//             }
//         }
//         return false;         // assume don't have too long wall
//     }

//     // *** YOU NEED TO COMPLETE THIS FUNCTION  ***
//     // must return true if there is no WALL adjacent to a virus 
//     // adjacency means left, right, top, bottom, and *diagonal* blocks
//     bool NoWallsCloseToVirus(List<TileType>[,] grid)
//     {
//         /*** implement the rest ! */

//         bool NO_wall_near_virus = true; // assume there is no wall near virus
//         foreach (int[] virus in pos_viruses)
//         {
//             // edge case, we sure there is wall near virus
//             if ((virus[0] < 2 || virus[0] >= width - 2) && (virus[1] < 2 || virus[1] >= length - 2))
//             {
//                 NO_wall_near_virus = false;
//                 return NO_wall_near_virus;
//             }
//             else
//             {   //check near grid cells 
//                 for (int i = virus[0] - 1; i <= virus[0] + 1; i++) 
//                 {
//                     for (int j = virus[1] - 1; j <= virus[1] + 1; j++)
//                     {
//                         if ((grid[i, j].Count == 1) && grid[i, j].Contains(TileType.WALL))
//                         {
//                             NO_wall_near_virus = false;
//                             return NO_wall_near_virus;

//                         }
//                     }
//                 }
//             }
//         }
//         return NO_wall_near_virus;
//     }


//     // check if attempted assignment is consistent with the constraints or not
//     bool CheckConsistency(List<TileType>[,] grid, int[] cell_pos, TileType t)
//     {
//         int w = cell_pos[0];
//         int l = cell_pos[1];

//         List<TileType> old_assignment = new List<TileType>();
//         old_assignment.AddRange(grid[w, l]);
//         grid[w, l] = new List<TileType> { t };

// 		// note that we negate the functions here i.e., check if we are consistent with the constraints we want
//         bool areWeConsistent = !DoWeHaveTooFewWallsORWaterORCoin(grid) && !DoWeHaveTooManyInteriorWallsORWaterORCoin(grid); 
//                             // && !TooLongWall(grid) && !NoWallsCloseToVirus(grid);

//         grid[w, l] = new List<TileType>();
//         grid[w, l].AddRange(old_assignment);
//         return areWeConsistent;
//     }


//     // *** YOU NEED TO COMPLETE THIS FUNCTION  ***
//     // implement backtracking 
//     bool BackTrackingSearch(List<TileType>[,] grid, List<int[]> unassigned)
//     {
//         // if there are too many recursive function evaluations, then backtracking has become too slow (or constraints cannot be satisfied)
//         // to provide a reasonable amount of time to start the level, we put a limit on the total number of recursive calls
//         // if the number of calls exceed the limit, then it's better to try a different initialization
//         if (function_calls++ > 100000)       
//             return false;

//         // we are done!
//         if (unassigned.Count == 0)
//             return true;

//         /*** implement the rest ! */
//         int randomValue = Random.Range(0, unassigned.Count);  //choose random unassigned value
//         int[] assigned = unassigned[randomValue];
//         foreach (TileType t in grid[assigned[0], assigned[1]])
//         {
//             if (CheckConsistency(grid, assigned, t)){
//                 List<TileType> old_one = new List<TileType>();
//                 old_one.AddRange(grid[assigned[0], assigned[1]]);
//                 // head = cell which we assign successfully
//                 int[] head = unassigned[randomValue];
//                 grid[assigned[0], assigned[1]] = new List<TileType> { t };
//                 unassigned.RemoveAt(randomValue);
//                 bool result = BackTrackingSearch(grid, unassigned);
//                 if (result)
//                 {
//                     return true;
//                 }
//                 else
//                 {
//                     // go back
//                     grid[assigned[0], assigned[1]] = old_one;
//                     unassigned.Insert(randomValue, head);
//                 } 
//             }
//         }
//         return false;
//     }

//     // This function is to check whether the maze is valid
//     // if valid:
//     // 1. for internal cell: 
//     //    (1) if this cell is close to two edges: all its internal neighbors are not WALL
//     //    (2) if this cell is close to zero edges: there are at most two internal neighbors are WALL

//     // 2. for external cell: 
//     //    (1) not four corner cell: all its internal neighbors are not WALL
//     //    (2) for four corner cell: don't care
//     void maze_generation_v2(ref List<TileType>[,] grid)
//     {
//         List<int[]> neighbors = new List<int[]>();
//         neighbors.Add(new int[] {0,-1});
//         neighbors.Add(new int[] {0,1});
//         neighbors.Add(new int[] {-1,0});
//         neighbors.Add(new int[] {1,0});
//         for (int w = 0; w < width; w++)
//         {
//             for (int l = 0; l < length; l++)
//             {
//                 int num_of_walls=0;
//                 // 2. for internal cell
//                 if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
//                 {    
//                     // 2(2) for four corner cell: don't care
//                     if ((w == 0 && l == 0) || (w == 0 && l == length - 1) || (w == width - 1 && l == 0) || (w == width - 1 && l == length - 1))  
//                     {

//                         continue;
//                     }  

//                     // 2(1) not four corner cell: all its internal neighbors are not WALL
//                     else
//                     {
//                         for (int neiborghs_idx=0; neiborghs_idx<neighbors.Count; neiborghs_idx++)
//                         {
//                             // neiborgh_x,neiborgh_y: the location of current gird's neiborghs
//                             int neiborgh_x = w+neighbors[neiborghs_idx][0];
//                             int neiborgh_y = l+neighbors[neiborghs_idx][1];  

//                             if ((neiborgh_x>=1)&&(neiborgh_x<width-1)&&(neiborgh_y>=1)&&(neiborgh_y<length-1))
//                             {

//                                 if ((grid[neiborgh_x,neiborgh_y].Count==1)&&(grid[neiborgh_x,neiborgh_y].Contains(TileType.WALL)))
//                                 {
//                                     grid[neiborgh_x,neiborgh_y].Remove(TileType.WALL);
//                                     grid[neiborgh_x,neiborgh_y].Add(TileType.FLOOR);                                    
//                                 }

//                             }

//                         }  
//                     }           

//                 }
//                 // 1. for internal cell
//                 else
//                 {
//                     // 1(1) if this cell is close to two edges: all its internal neighbors are not WALL
//                     if ((w == 1 && l == 1) || (w == 1 && l == length - 2) || (w == width - 2 && l == 1) || (w == width - 2 && l == length - 2))  
//                     {
//                         for (int neiborghs_idx=0; neiborghs_idx<neighbors.Count; neiborghs_idx++)
//                         {
//                             // neiborgh_x,neiborgh_y: the location of current gird's neiborghs
//                             int neiborgh_x = w+neighbors[neiborghs_idx][0];
//                             int neiborgh_y = l+neighbors[neiborghs_idx][1];  

//                             if ((neiborgh_x>=1)&&(neiborgh_x<width-1)&&(neiborgh_y>=1)&&(neiborgh_y<length-1))
//                             {

//                                 if ((grid[neiborgh_x,neiborgh_y].Count==1)&&(grid[neiborgh_x,neiborgh_y].Contains(TileType.WALL)))
//                                 {
//                                     grid[neiborgh_x,neiborgh_y].Remove(TileType.WALL);
//                                     grid[neiborgh_x,neiborgh_y].Add(TileType.FLOOR);                                    
//                                 }

//                             }

//                         }   
//                     }

//                     // 1(2) if this cell is close to zero edges: there are at most two internal neighbors are WALL
//                     else
//                     {
//                         for (int neiborghs_idx=0; neiborghs_idx<neighbors.Count; neiborghs_idx++)
//                         {
//                             // neiborgh_x,neiborgh_y: the location of current gird's neiborghs
//                             int neiborgh_x = w+neighbors[neiborghs_idx][0];
//                             int neiborgh_y = l+neighbors[neiborghs_idx][1];  

//                             if ((neiborgh_x>=1)&&(neiborgh_x<width-1)&&(neiborgh_y>=1)&&(neiborgh_y<length-1))
//                             {
//                                 if ((grid[neiborgh_x,neiborgh_y].Count==1)&&(grid[neiborgh_x,neiborgh_y].Contains(TileType.WALL)))
//                                 {
//                                     num_of_walls += 1;
//                                 }

//                                 if (num_of_walls > 2)
//                                 {
//                                     grid[neiborgh_x,neiborgh_y].Remove(TileType.WALL);
//                                     grid[neiborgh_x,neiborgh_y].Add(TileType.FLOOR);
//                                     num_of_walls-=1;

//                                 }
//                             }

                            
//                         }                          
//                     }
                 
//                 }
                 
//             }
//         }
        
                  
//     }
//     // places the primitives/objects according to the grid assignents
//     // you will need to edit this function (see below)
//     void DrawDungeon(List<TileType>[,] solution)
//     {
//         GetComponent<Renderer>().material.color = Color.grey; // ground plane will be grey

//         // place character at random position (wr, lr) in terms of grid coordinates (integers)
//         // make sure that this random position is a FLOOR tile (not wall, coin, or virus)
//         int wr = 0;
//         int lr = 0;
//         if (!first_pos)  // try same map case, I will generate player position once, we don't need to generate again
//         {
//             while (true) // try until a valid position is sampled
//             {
//                 wr = Random.Range(1, width - 1);
//                 lr = Random.Range(1, length - 1);

//                 if (solution[wr, lr][0] == TileType.FLOOR)
//                 {
//                     float x = bounds.min[0] + (float)wr * (bounds.size[0] / (float)width);
//                     float z = bounds.min[2] + (float)lr * (bounds.size[2] / (float)length);
//                     fps_player_obj = Instantiate(fps_prefab);
//                     fps_player_obj.name = "PLAYER";
//                     fps_player_obj.tag = "Tile";
//                     // character is placed above the level so that in the beginning, he appears to fall down onto the maze
//                     fps_player_obj.transform.position = new Vector3(x + 0.5f, 2.0f * storey_height, z + 0.5f);

//                     // fps_player_obj.AddComponent<AudioSource>();  //add audioSource component
//                     audio_com = fps_player_obj.GetComponent<AudioSource>();
//                     player_pos_w = wr;
//                     player_pos_l = lr;
//                     first_pos = true;
//                     break;
//                 }
//             }
//         }

//         // place an exit from the maze at location (wee, lee) in terms of grid coordinates (integers)
//         // destroy the wall segment there - the grid will be used to place a house
//         // the exist will be placed as far as away from the character (yet, with some randomness, so that it's not always located at the corners)
//         // int max_dist = -1;
//         // int wee = -1;
//         // int lee = -1;
//         // if (!house_exist) // try same map case, I will generate house position once, we don't need to generate again
//         // {
//         //     while (true) // try until a valid position is sampled
//         //     {
//         //         if (wee != -1)
//         //             break;
//         //         for (int we = 0; we < width; we++)
//         //         {
//         //             for (int le = 0; le < length; le++)
//         //             {
//         //                 // skip corners
//         //                 if (we == 0 && le == 0)
//         //                     continue;
//         //                 if (we == 0 && le == length - 1)
//         //                     continue;
//         //                 if (we == width - 1 && le == 0)
//         //                     continue;
//         //                 if (we == width - 1 && le == length - 1)
//         //                     continue;

//         //                 if (we == 0 || le == 0 || wee == length - 1 || lee == length - 1)
//         //                 {
//         //                     // randomize selection
//         //                     if (Random.Range(0.0f, 1.0f) < 0.1f)
//         //                     {
//         //                         int dist = System.Math.Abs(wr - we) + System.Math.Abs(lr - le);
//         //                         if (dist > max_dist) // must be placed far away from the player
//         //                         {
//         //                             wee = we;
//         //                             lee = le;
//         //                             max_dist = dist;
//         //                             house_pos_l = le;
//         //                             house_pos_w = we;
//         //                         }
//         //                     }
//         //                 }
//         //             }
//         //         }
//         //     }
//         // }


//         // *** YOU NEED TO COMPLETE THIS PART OF THE FUNCTION  ***
//         // implement an algorithm that checks whether
//         // all paths between the player at (wr,lr) and the exit (wee, lee)
//         // are blocked by walls. i.e., there's no way to get to the exit!
//         // if this is the case, you must guarantee that there is at least 
//         // one accessible path (any path) from the initial player position to the exit
//         // by removing a few wall blocks (removing all of them is not acceptable!)
//         // this is done as a post-processing step after the CSP solution.
//         // It might be case that some constraints might be violated by this
//         // post-processing step - this is OK.

//         /*** implement what is described above ! */
//         maze_generation_v2(ref solution);
//         Debug.Log("Maze is generated successfully!");
//         // Result result = A_star_search(solution, player_pos_w, player_pos_l);
//         // if (result.result)  // find path case
//         // {
//         //     Debug.Log(result.result);
//         // }
//         // // no path finding case, basic idea: find closest grid position(searched floor which has lowest h score)
//         // // during A star search.
//         // // replace its neighbor (wall which has the lowest h score) to floor, then A star search again.
//         // else  
//         // {
//         //     Debug.Log(result.result + " case");
//         //     bool locker = true;
//         //     int new_start_w = result.w;  // closest position w during first search
//         //     int new_start_l = result.l;  // closest position l during first search
//         //     int[] house_pos = { house_pos_w, house_pos_l };
//         //     while (locker)
//         //     {
//         //         int[] cur_position = { new_start_w, new_start_l };
//         //         int[][] neighbor = neighbors(cur_position);
//         //         float h = float.PositiveInfinity;
//         //         for (int i = 0; i < 4; i++)  // check 4 direction neighbors
//         //         {
//         //             int ww = neighbor[i][0];
//         //             int ll = neighbor[i][1];
//         //             if (ww != 0 && ll != 0 && ww != width - 1 && ll != length - 1) // ignore outer wall
//         //             {
//         //                 if(solution[ww, ll].Contains(TileType.WALL)) // only care neighbor is wall
//         //                 {
//         //                     if(heuristic(ww, ll, house_pos) < h)  // find neighbor(wall) which is closest to the wall
//         //                     {
//         //                         h = heuristic(ww, ll, house_pos); // update h
//         //                         new_start_w = ww;                 // update closest_w
//         //                         new_start_l = ll;                 // update closest_l
//         //                     }
//         //                 }
//         //             }
//         //         }
//         //         // find wall neighbor position, replace it to Floor.
//         //         solution[new_start_w, new_start_l] = new List<TileType> { TileType.FLOOR }; 
//         //         // check whether we can find path in updated grid
//         //         Result new_tile_result = A_star_search(solution, new_start_w, new_start_l);
//         //         if (new_tile_result.result)
//         //         {
//         //             locker = false;  // if we can reach the house, update locker
//         //         }
//         //         else
//         //         {
//         //             // if we still can't find path
//         //             new_start_w = new_tile_result.w;  // update closest_w in this turn
//         //             new_start_l = new_tile_result.l;  // update closest_l in this turn, continue while loop
//         //         }

//         //     }
//         // }

//         // the rest of the code creates the scenery based on the grid state 
//         // you don't need to modify this code (unless you want to replace the virus
//         // or other prefabs with something else you like)
//         int w = 0;
//         for (float x = bounds.min[0]; x < bounds.max[0]; x += bounds.size[0] / (float)width - 1e-6f, w++)
//         {
//             int l = 0;
//             for (float z = bounds.min[2]; z < bounds.max[2]; z += bounds.size[2] / (float)length - 1e-6f, l++)
//             {
//                 if ((w >= width) || (l >= width))
//                     continue;

//                 float y = bounds.min[1];
//                 //Debug.Log(w + " " + l + " " + h);
//                 // if ((w == wee) && (l == lee)) // this is the exit
//                 // {
//                 //     // try same map case, I will generate house once, we don't need to generate again
//                 //     if (!house_exist) 
//                 //     {
//                 //         GameObject house = Instantiate(house_prefab, new Vector3(0, 0, 0), Quaternion.identity);
//                 //         house.name = "HOUSE";
//                 //         house.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);
//                 //         if (l == 0)
//                 //             house.transform.Rotate(0.0f, 270.0f, 0.0f);
//                 //         else if (w == 0)
//                 //             house.transform.Rotate(0.0f, 0.0f, 0.0f);
//                 //         else if (l == length - 1)
//                 //             house.transform.Rotate(0.0f, 90.0f, 0.0f);
//                 //         else if (w == width - 1)
//                 //             house.transform.Rotate(0.0f, 180.0f, 0.0f);

//                 //         house.AddComponent<BoxCollider>();
//                 //         house.GetComponent<BoxCollider>().isTrigger = true;
//                 //         house.GetComponent<BoxCollider>().size = new Vector3(3.0f, 3.0f, 3.0f);
//                 //         house.AddComponent<House>();
//                 //         house_exist = true;
//                 //     }
//                 //     else
//                 //     {
//                 //         GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 //         cube.name = "WALL";
//                 //         cube.transform.localScale = new Vector3(bounds.size[0] / (float)width, storey_height, bounds.size[2] / (float)length);
//                 //         cube.transform.position = new Vector3(x + 0.5f, y + storey_height / 2.0f, z + 0.5f);
//                 //         cube.GetComponent<Renderer>().material.color = new Color(0.6f, 0.8f, 0.8f);
//                 //     }
               
//                 // }
//                 if (solution[w, l][0] == TileType.WALL)
//                 {
//                     if (w == house_pos_w && l == house_pos_l)
//                     {
//                         // ignore current house position in try same map case
//                     }
//                     else
//                     {
//                         GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                         cube.name = "WALL";
//                         if (w != 0 && l != 0 && w != width - 1 && l != length - 1)
//                         {
//                             cube.tag = "Tile";
//                         }
//                         cube.transform.localScale = new Vector3(bounds.size[0] / (float)width, storey_height, bounds.size[2] / (float)length);
//                         cube.transform.position = new Vector3(x + 0.5f, y + storey_height / 2.0f, z + 0.5f);
//                         //cube.GetComponent<Renderer>().material.color = new Color(0.6f, 0.8f, 0.8f);
//                         cube.GetComponent<Renderer>().material = cubeMaterial;
//                     }
                    
//                 }
//                 else if (solution[w, l][0] == TileType.VIRUS)
//                 {
//                     GameObject virus = Instantiate(virus_prefab, new Vector3(0, 0, 0), Quaternion.identity);
//                     virus.name = "COVID";
//                     virus.tag = "Tile";
//                     virus.transform.position = new Vector3(x + 0.5f, y + Random.Range(1.0f, storey_height / 2.0f), z + 0.5f);

//                     //GameObject virus = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//                     //virus.GetComponent<Renderer>().material.color = new Color(0.5f, 0.0f, 0.0f);
//                     //virus.name = "ENEMY";
//                     //virus.transform.position = new Vector3(x + 0.5f, y + Random.Range(1.0f, storey_height / 2.0f), z + 0.5f);
//                     //virus.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
//                     //virus.AddComponent<BoxCollider>();
//                     //virus.GetComponent<BoxCollider>().size = new Vector3(1.2f, 1.2f, 1.2f);
//                     //virus.AddComponent<Rigidbody>();
//                     //virus.GetComponent<Rigidbody>().useGravity = false;

//                     virus.AddComponent<Virus>();
//                     virus.GetComponent<Rigidbody>().mass = 10000;
//                 }
//                 //==========================================================================================================
//                 //==========================================================================================================
//                 else if (solution[w, l][0] == TileType.COIN)
//                 {
//                     GameObject capsule = Instantiate(coin_prefab, new Vector3(0, 0, 0), Quaternion.identity);
//                     capsule.name = "COIN";
//                     capsule.tag = "Tile";
//                     capsule.transform.position = new Vector3(x + 0.5f, y - Random.Range(1.0f, storey_height / 2.0f), z + 0.5f);
//                     capsule.transform.localScale = new Vector3(3f, 3f, 3f);
//                     //capsule.transform.Rotate(90,0,0);
//                     //capsule.AddComponent<Renderer>();
//                     //capsule.GetComponent<Renderer>().material = coin_mat;

//                     /*                    if (capsuleRenderer != null)
//                                         {
//                                             capsuleRenderer.material = coin_mat;
//                                             // capsuleRenderer.material.color = new Color(1.0f, 1.0f, 0.0f); 
//                                         }
//                                         else
//                                         {
//                                             capsuleRenderer = capsule.AddComponent<MeshRenderer>();
//                                             //capsuleRenderer.material = coin_mat; 
//                                             capsuleRenderer.material.color = Color.blue;

//                                         }

//                                         capsule.AddComponent<Coin>();
//                                         //GameObject virus = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//                                         //virus.GetComponent<Renderer>().material.color = new Color(0.5f, 0.0f, 0.0f);
//                                         //virus.name = "ENEMY";
//                                         // virus.transform.position = new Vector3(x + 0.5f, y + Random.Range(1.0f, storey_height / 2.0f), z + 0.5f);
//                                         capsule.transform.localScale = new Vector3(10f, 10f, 10f);
//                                         //virus.AddComponent<BoxCollider>();
//                                         //virus.GetComponent<BoxCollider>().size = new Vector3(1.2f, 1.2f, 1.2f);
//                                         //virus.AddComponent<Rigidbody>();
//                                         //virus.GetComponent<Rigidbody>().useGravity = false;

//                                         capsule.AddComponent<Coin>();

//                                         //capsule.GetComponent<Rigidbody>().mass = 10000;*/
//                 }


//                 // else if (solution[w, l][0] == TileType.COIN)
//                 // {
//                 //     GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
//                 //     capsule.name = "COIN";
//                 //     capsule.tag = "Tile";
//                 //     capsule.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
//                 //     capsule.transform.position = new Vector3(x + 0.5f, y + Random.Range(1.0f, storey_height / 2.0f), z + 0.5f);
//                 //     capsule.GetComponent<Renderer>().material.color = Color.green;
//                 //     capsule.AddComponent<Coin>();
//                 // }
//                 //==========================================================================================================
//                 //==========================================================================================================
//                 else if (solution[w, l][0] == TileType.WATER)
//                 {
//                     GameObject water = Instantiate(water_prefab, new Vector3(0, 0, 0), Quaternion.identity);
//                     water.name = "WATER";
//                     water.tag = "Tile";
//                     water.transform.localScale = new Vector3(0.5f * bounds.size[0] / (float)width, 1.0f, 0.5f * bounds.size[2] / (float)length);
//                     water.transform.position = new Vector3(x + 0.5f, y + 0.1f, z + 0.5f);

//                     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                     cube.name = "WATER_BOX";
//                     cube.transform.localScale = new Vector3(bounds.size[0] / (float)width, storey_height / 20.0f, bounds.size[2] / (float)length);
//                     cube.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);
//                     cube.GetComponent<Renderer>().material.color = Color.grey;
//                     cube.GetComponent<BoxCollider>().size = new Vector3(1.1f, 20.0f * storey_height, 1.1f);
//                     cube.GetComponent<BoxCollider>().isTrigger = true;
//                     cube.AddComponent<Water>();
//                 }
//             }
//         }
//     }


//     // *** YOU NEED TO COMPLETE THIS PART OF THE FUNCTION JUST TO ADD SOUNDS ***
//     // YOU MAY CHOOSE ANY SHORT SOUNDS (<2 sec) YOU WANT FOR A VIRUS HIT, A VIRUS INFECTION,
//     // GETTING INTO THE WATER, AND REACHING THE EXIT
//     // note: you may also change other scripts/functions to add sound functionality,
//     // along with the functionality for the starting the level, or repeating it
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Escape))
//         {
//             ingame_menu_active = !ingame_menu_active;
//             if(ingame_menu_active)
//             {
//                 panel.gameObject.SetActive(true);
//             }
//             else
//             {
//                 panel.gameObject.SetActive(false);
//             }
//         }
//         if (player_health < 0.001f) // the player dies here
//         {
//             text_box.GetComponent<Text>().text = "Failed!";

//             if (fps_player_obj != null)
//             {
//                 GameObject grave = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 grave.name = "GRAVE";
//                 grave.tag = "Tile";
//                 grave.transform.localScale = new Vector3(bounds.size[0] / (float)width, 2.0f * storey_height, bounds.size[2] / (float)length);
//                 grave.transform.position = fps_player_obj.transform.position;
//                 grave.GetComponent<Renderer>().material.color = Color.black;
//                 Object.Destroy(fps_player_obj);

//                 // sound setting when player die
//                 GameObject camera = GameObject.Find("Main Camera");
//                 camera.GetComponent<AudioSource>().clip = lose_health_sound;
//                 camera.GetComponent<AudioListener>().enabled = true;
//                 camera.GetComponent<AudioSource>().Play();
//                 // lose case: panel setting
//                 panel.gameObject.SetActive(true);
//                 ingame_menu_active = true;
//                 Cursor.visible = true;
//                 Cursor.lockState = CursorLockMode.None;
//                 camera.GetComponent<AudioListener>().enabled = false;
//             }

//             return;
//         }
//         if (player_entered_house) // the player suceeds here, variable manipulated by House.cs
//         {
//             if (!hasPlayed_forHouse)
//             {
//                 GameObject camera = GameObject.Find("Main Camera");
//                 camera.GetComponent<AudioSource>().clip = enter_house_sound;
//                 camera.GetComponent<AudioListener>().enabled = true;
//                 camera.GetComponent<AudioSource>().Play();
//                 audio_com.SetScheduledEndTime(AudioSettings.dspTime + 2f);
//                 hasPlayed_forHouse = true;
//                 // win case: panel setting
//                 panel.gameObject.SetActive(true);
//                 ingame_menu_active = true;
//                 Cursor.visible = true;
//                 Cursor.lockState = CursorLockMode.None;
//                 camera.GetComponent<AudioListener>().enabled = false;
//             }
//             if (virus_landed_on_player_recently)
//                 text_box.GetComponent<Text>().text = "Washed it off at home! Success!!!";
//             else
//                 text_box.GetComponent<Text>().text = "Success!!!";

//             Object.Destroy(fps_player_obj);
//             return;
//         }

//         if (Time.time - timestamp_last_msg > 7.0f) // renew the msg by restating the initial goal
//         {
//             text_box.GetComponent<Text>().text = "Find your home!";            
//         }

//         // virus hits the players (boolean variable is manipulated by Virus.cs)
//         if (virus_landed_on_player_recently)
//         {
//             float time_since_virus_landed = Time.time - timestamp_virus_landed;
//             if (time_since_virus_landed > 5.0f)
//             {
//                 audio_com.clip = lose_health_sound;  // lose health audio
//                 audio_com.Play();
//                 player_health -= Random.Range(0.25f, 0.5f) * (float)num_virus_hit_concurrently;
//                 player_health = Mathf.Max(player_health, 0.0f);
//                 if (num_virus_hit_concurrently > 1)
//                     text_box.GetComponent<Text>().text = "Ouch! Infected by " + num_virus_hit_concurrently + " viruses";
//                 else
//                     text_box.GetComponent<Text>().text = "Ouch! Infected by a virus";
//                 timestamp_last_msg = Time.time;
//                 timestamp_virus_landed = float.MaxValue;
//                 virus_landed_on_player_recently = false;
//                 num_virus_hit_concurrently = 0;
//             }
//             else
//             {
//                 if (num_virus_hit_concurrently == 1)
//                     text_box.GetComponent<Text>().text = "A virus landed on you. Infection in " + (5.0f - time_since_virus_landed).ToString("0.0") + " seconds. Find water or coin!";
//                 else
//                     text_box.GetComponent<Text>().text = num_virus_hit_concurrently + " viruses landed on you. Infection in " + (5.0f - time_since_virus_landed).ToString("0.0") + " seconds. Find water or coin!";
//             }
//         }

//         // coin picked by the player  (boolean variable is manipulated by Coin.cs)
//         if (coin_landed_on_player_recently)
//         {
//             audio_com.clip = coin_collide_sound;
//             audio_com.time = 1.5f;
//             audio_com.Play();
//             audio_com.SetScheduledEndTime(AudioSettings.dspTime + 1.5f);  //set coin audio
//             if (player_health < 0.999f || virus_landed_on_player_recently)
//                 text_box.GetComponent<Text>().text = "Phew! New coin helped!";
//             else
//                 text_box.GetComponent<Text>().text = "No coin was needed!";
//             timestamp_last_msg = Time.time;
//             player_health += Random.Range(0.25f, 0.75f);
//             player_health = Mathf.Min(player_health, 1.0f);
//             coin_landed_on_player_recently = false;
//             timestamp_virus_landed = float.MaxValue;
//             virus_landed_on_player_recently = false;
//             num_virus_hit_concurrently = 0;
//         }

//         // splashed on water  (boolean variable is manipulated by Water.cs)
//         if (player_is_on_water)
//         {
//             if (virus_landed_on_player_recently)
//                 text_box.GetComponent<Text>().text = "Phew! Washed it off!";
//             timestamp_last_msg = Time.time;
//             timestamp_virus_landed = float.MaxValue;
//             virus_landed_on_player_recently = false;
//             num_virus_hit_concurrently = 0;
//         }

//         // update scroll bar (not a very conventional manner to create a health bar, but whatever)
//         scroll_bar.GetComponent<Scrollbar>().size = player_health;
//         if (player_health < 0.5f)
//         {
//             ColorBlock cb = scroll_bar.GetComponent<Scrollbar>().colors;
//             cb.disabledColor = new Color(1.0f, 0.0f, 0.0f);
//             scroll_bar.GetComponent<Scrollbar>().colors = cb;
//         }
//         else
//         {
//             ColorBlock cb = scroll_bar.GetComponent<Scrollbar>().colors;
//             cb.disabledColor = new Color(0.0f, 1.0f, 0.25f);
//             scroll_bar.GetComponent<Scrollbar>().colors = cb;
//         }

//         /*** implement the rest ! */
//     }
//     IEnumerator DelayedAction()
//     {
//         yield return new WaitForSeconds(2.0f);
//     }

//     private float heuristic(int w, int l, int[] goal)
//     {
//         //Euclidean distance
//         float x = (float)Mathf.Pow(w - goal[0], 2) + (float)Mathf.Pow(l - goal[1], 2);
//         return (float)Mathf.Sqrt(x);
//     }
//     private Result A_star_search(List<TileType>[,] solution, int start_pos_w, int start_pos_l)
//     {
//         // initial 
//         int[] house_pos = { house_pos_w, house_pos_l };
//         Node[,] a_star_map = new Node[width, length];
//         for (int i = 0; i < width; i++)
//         {
//             for (int j = 0; j < length; j++)
//             {
//                 int[] pp = { i, j };
//                 a_star_map[i, j] = new Node(pp);
//             }
//         }
//         //
//         PriorityQueue Q = new PriorityQueue();
//         int[] start_pos = { start_pos_w, start_pos_l };
//         Debug.Log("house_pos" + house_pos[0] + " " + house_pos[1]);
//         //start position 
//         a_star_map[start_pos_w, start_pos_l].cell = start_pos;
//         a_star_map[start_pos_w, start_pos_l].gScore = 0;
//         a_star_map[start_pos_w, start_pos_l].hScore = heuristic(start_pos[0], start_pos[1], house_pos);
//         a_star_map[start_pos_w, start_pos_l].fScore =
//             a_star_map[start_pos_w, start_pos_l].gScore + a_star_map[start_pos_w, start_pos_l].hScore;
//         Q.Enqueue(a_star_map[start_pos_w, start_pos_l]);
//         // collect close heuristic position
//         float close_heuristic_pos = heuristic(start_pos[0], start_pos[1], house_pos);
//         int close_w = start_pos_w;
//         int close_l = start_pos_l;

//         while (!Q.IsEmpty())
//         {
// /*            if(aa == 5000)
//             {
//                 Debug.Log("max"); // avoid infinity loop
//                 break;
//             }
//             aa++;*/
            
//             Node current = Q.Peek();
//             int[] current_pos = current.cell;

//             if (current.cell[0] == house_pos[0] && current.cell[1] == house_pos[1])
//             {
//                 return new Result { result = true, w = -1, l = -1};  // reach the house position
//             }
//             // update the closest position during the search process
//             float current_h = heuristic(current_pos[0], current_pos[1], house_pos);
//             if (current_h < close_heuristic_pos)
//             {
//                 close_heuristic_pos = current_h;
//                 close_w = current_pos[0];
//                 close_l = current_pos[1];
//             }
//             Q.Dequeue();
//             int[][] neighbor = neighbors(current_pos);
//             for (int g = 0; g < 4; g++)
//             {
//                 int i = neighbor[g][0]; // neighbor w position
//                 int j = neighbor[g][1]; // neighbor l position
//                 if (i == house_pos[0] && j == house_pos[1]) // special case for neighbor is house but is in wall tile
//                 {
//                     float gScore = current.gScore + heuristic(i, j, current_pos);
//                     if (gScore < a_star_map[i, j].gScore)
//                     {  // found better path
//                         a_star_map[i, j].Previous = current;
//                         a_star_map[i, j].gScore = gScore; // update G and F
//                         a_star_map[i, j].hScore = heuristic(i, j, house_pos);
//                         a_star_map[i, j].fScore = gScore + heuristic(i, j, house_pos);
//                         if (!Q.cell_in_queue(i, j))
//                         {
//                             Q.Enqueue(a_star_map[i, j]);
//                         }
//                     }
//                 }
//                 else if (!solution[i, j].Contains(TileType.WALL)) // general case: don't check wall
//                 {
//                     float gScore = current.gScore + heuristic(i, j, current_pos);
//                     if (gScore < a_star_map[i, j].gScore)
//                     {  // found better path
//                         a_star_map[i, j].gScore = gScore; // update G and F
//                         a_star_map[i, j].fScore = gScore + heuristic(i, j, house_pos);
//                         a_star_map[i, j].hScore = heuristic(i, j, house_pos);
//                         if (!Q.cell_in_queue(i, j))
//                         {
//                             Q.Enqueue(a_star_map[i, j]);
//                         }
//                     }
//                 }
//             }
//         }

//         return new Result {result = false, w = close_w, l = close_l };
//     }
//     private List<Node> reconstruct_path(Node goal)
//     {
//         var path = new List<Node>();
//         Node current = goal;

//         while (current != null)
//         {
//             path.Insert(0, current);
//             current = current.Previous;
//         }

//         return path;
//     }
//     private int[][] neighbors(int[] cur_position)
//     {
//         int[][] jaggedArray = new int[4][];
//         jaggedArray[0] = new int[] { cur_position[0] - 1, cur_position[1] };
//         jaggedArray[1] = new int[] { cur_position[0] + 1, cur_position[1] };
//         jaggedArray[2] = new int[] { cur_position[0], cur_position[1] - 1 };
//         jaggedArray[3] = new int[] { cur_position[0], cur_position[1] + 1};
//         return jaggedArray;
//     }
//     public void try_same_map() // renew game status for same map, check comments for detail
//     {
//         GameObject[] existingTiles = GameObject.FindGameObjectsWithTag("Tile");

//         foreach (var tile in existingTiles)
//         {
//             Destroy(tile); // destroy generated objects except out wall and house
//         }
//         // renew the map and player status
//         timestamp_last_msg = 0.0f;
//         function_calls = 0;
//         num_viruses = 0;
//         player_health = 1.0f;
//         num_virus_hit_concurrently = 0;
//         virus_landed_on_player_recently = false;
//         timestamp_virus_landed = float.MaxValue;
//         coin_landed_on_player_recently = false;
//         player_is_on_water = false;
//         player_entered_house = false;
//         hasPlayed_forHouse = false; 

//         float x = bounds.min[0] + (float)player_pos_w * (bounds.size[0] / (float)width);
//         float z = bounds.min[2] + (float)player_pos_l * (bounds.size[2] / (float)length);
//         fps_player_obj = Instantiate(fps_prefab);
//         fps_player_obj.name = "PLAYER";
//         fps_player_obj.tag = "Tile";

//         // character is placed above the level so that in the beginning, he appears to fall down onto the maze
//         fps_player_obj.transform.position = new Vector3(x + 0.5f, 2.0f * storey_height, z + 0.5f);

//         // fps_player_obj.AddComponent<AudioSource>();  //add audioSource component
//         audio_com = fps_player_obj.GetComponent<AudioSource>();
//         panel.gameObject.SetActive(false);
//         ingame_menu_active = false;

//         DrawDungeon(old_grid);
//     }
}

