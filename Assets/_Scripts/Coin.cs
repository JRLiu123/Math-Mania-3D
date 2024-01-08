using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour

{
    private GameObject fps_player_obj;
    private Maze maze; 
    public float rotation_speed = 35f; 

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

    void Update()
    {
        // Color greenness = new Color
        // {
        //     g = Mathf.Max(1.0f, 0.1f + Mathf.Abs(Mathf.Sin(Time.time)))
        // };
        // GetComponent<MeshRenderer>().material.color = greenness;
        transform.Rotate(new Vector3(0,0,1), rotation_speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PLAYER")
        {
            //Debug.Log("Collider with player.");
            maze.coin_landed_on_player_recently = true;
            Destroy(gameObject);
        }
    }

}
