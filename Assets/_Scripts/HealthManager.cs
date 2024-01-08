using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HealthManager : MonoBehaviour
{
    public Image health_bar;
    public float health;
    public TextMeshProUGUI text;
    private float maxHealth;
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
        health = 100f;
        maze.set_health(health); 
        maxHealth = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.Find("PLAYER");
        text.text = "Health: " + health + "/" + maxHealth;
        BarFiller();
        if(player != null && player.transform.position.y < -10)
        {
            health = 0;  // player below the map
            BarFiller();
        }
        if (maze.is_health_changed())
        {

            maze.set_health_changed(); 
            health = maze.get_health(); 
        }
        else
            maze.set_health(health); 

        if (health < 0)
        {
            health = 0;
        } 
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    void BarFiller()
    {
        health_bar.fillAmount = Mathf.Lerp(health_bar.fillAmount, health/maxHealth, 2f * Time.deltaTime);
    }
}
