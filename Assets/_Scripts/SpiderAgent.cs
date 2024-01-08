using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


public class SpiderAgent : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    private Animator animator;
    float agent_speed;
    public GameObject fireball_prefab;
    private bool rest; // check rest status 
    public float rest_time;
    public GameObject explosion_prefab;
    public Light spotLightPrefab;
    // deflection shooting 
    private Vector3 player_last_position;
    GameObject player;
    void Start()
    {
        player = GameObject.Find("PLAYER");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent_speed = agent.speed;
        rest = false;
        //rest_time = 5f;
        player_last_position = player.transform.position;
        StartCoroutine(RecordPositionCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find("PLAYER");
        if (player != null)
        {
            this.agent.SetDestination(new Vector3(player.transform.position.x, 20, player.transform.position.z));
        }

        //animator
        if (animator != null && player != null)
        {
            // attack
            Vector3 player_pos = player.transform.position;
            Vector3 offset = player_pos - agent.transform.position;
            offset.Normalize();
            float range = offset.magnitude; // cur distance
            RaycastHit hit;
            if (Physics.Raycast(agent.transform.position, offset, out hit, Mathf.Infinity))
            {
                // find suitable position, then attack: shoot fireball
                if (hit.collider.gameObject == player && range <= 35f)
                { 
                    agent.speed = 0;
                    if (!rest)  
                    {
                        rest = true;
                        animator.SetInteger("status", 2); // attack animator
                        float xdirection = Mathf.Sin(Mathf.Deg2Rad * player.transform.rotation.eulerAngles.y);
                        float zdirection = Mathf.Cos(Mathf.Deg2Rad * player.transform.rotation.eulerAngles.y);
                        Vector3 movement_direction = new Vector3(xdirection, 0.0f, zdirection);
                        float time = range / 10f;
                        Vector3 prediction = player_pos + (movement_direction) *
                                             player.GetComponent<FirstPersonController>().walkSpeed * 2 * time;
                        Debug.Log(prediction);
                        Vector3 shooting_dir = prediction - agent.transform.position;
                        shooting_dir.Normalize();

                        GameObject fireball = Instantiate(fireball_prefab, agent.transform.position,
                                                Quaternion.identity);
                        fireball.tag = "Spider";
                        fireball.transform.rotation = Quaternion.LookRotation(-shooting_dir);  // steer fireball dir
                        //GameObject fireball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        fireball.GetComponent<fireball>().speed = 9f;
                        fireball.GetComponent<fireball>().move_dir = shooting_dir;
                        Debug.Log("catch you");
                        RaycastHit hits;
                        if (Physics.Raycast(agent.transform.position, shooting_dir, out hits, Mathf.Infinity, ~3))
                        {
                            Light spotLight = Instantiate(spotLightPrefab, hits.point, Quaternion.identity);
                            fireball.GetComponent<fireball>().alarm = spotLight; // assign alarm

                        }
                            // rest spider
                        StartCoroutine(ChangeRestAfterDelay(rest_time));
                    }
                    else
                    {
                        agent.speed = 0;
                        animator.SetInteger("status", 0); // idle 
                    }

                }
                else  // walking to find suitable shooting position
                {
                    animator.SetInteger("status", 1); // walk animator
                    agent.speed = agent_speed;
                }

            }
        }
    }
    IEnumerator ChangeRestAfterDelay(float delay)
    {
        // Wait for the specified delay
        
        yield return new WaitForSeconds(delay+0.1f);

        // Change the value of the 'test' variable after the delay
        rest = false;
    }

    private IEnumerator RecordPositionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            // Get the player's current position
            //GameObject player = GameObject.Find("PLAYER");
            player_last_position = player.transform.position;
        }
    }


}
