using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class brownSpider : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    private Animator animator;
    GameObject player;
    private bool locker;
    private float agent_speed;
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        locker = false;
        agent_speed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!locker)
        {
            if (!agent.enabled)
            {
                agent.enabled = true;
            }
            player = GameObject.Find("PLAYER");
            if (player != null)
            {
                this.agent.SetDestination(player.transform.position);
            }
            if (agent.speed != 0)
            {
                animator.SetInteger("status", 1);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "PLAYER")
        {
            agent_speed = 0;
            animator.SetInteger("status", 4);
            GameObject health_manager = GameObject.Find("health_manager");
            if (health_manager.GetComponent<HealthManager>().health > 0)
            {
                health_manager.GetComponent<HealthManager>().health -= 10f;
            }
            StartCoroutine(ChangeRestAfterDelay(5f));
        }
    }
    IEnumerator ChangeRestAfterDelay(float delay)
    {
        // Wait for the specified delay
        locker = true;
        agent.enabled = false;
        yield return new WaitForSeconds(1f);
        AudioClip soundClip = GetComponent<AudioSource>().clip;
        AudioSource.PlayClipAtPoint(soundClip, agent.gameObject.transform.position);
        yield return new WaitForSeconds(delay-1f);
        animator.SetInteger("status", 0);
        agent.enabled = true;
        // Change the value of the 'test' variable after the delay
        locker = false;
    }

}
