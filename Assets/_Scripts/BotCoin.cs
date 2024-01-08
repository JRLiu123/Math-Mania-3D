using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotCoin : MonoBehaviour
{
    private GameObject fps_player_obj;
    private Maze maze; 

    // Start is called before the first frame update
    void Start()
    {
        GameObject maze_obj = GameObject.FindGameObjectWithTag("Level");
        maze = maze_obj.GetComponent<Maze>();
        if (maze == null)
        {
            Debug.LogError("Internal error: could not find the Level object - did you remove its 'Level' tag?");
            return;
        }
        fps_player_obj = maze.fps_player_obj;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with " + other.gameObject.name);
        if (other.gameObject.name == "PLAYER")
        {
            Debug.Log("Collider with botcoin.");
            maze.coin_bot_landed_on_player_recently = true;
            Destroy(gameObject);
        }
    }
}
