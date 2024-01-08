using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class fireball : MonoBehaviour
{
    public Vector3 move_dir;
    public float speed;
    public GameObject explosion_prefab;
    public float explosion_force;
    public float explosion_radius;
    public GameObject health_manager;
    GameObject player;
    public AudioClip soundClip;
    // light setting
    public Light alarm;
    public float minIntensity = 20f;
    public float maxIntensity = 100f;
    public float duration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        explosion_prefab = Resources.Load("explosion") as GameObject;
        health_manager = GameObject.Find("health_manager");
        player = GameObject.Find("PLAYER");
        speed = 9f;
        //StartCoroutine(fireball_dir(move_dir, speed));
        explosion_radius = 6f;
        explosion_force = 1000f;
        alarm.intensity = 100f;
        alarm.range = 8f;
        StartCoroutine(ChangeLightIntensity(alarm));

    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, move_dir, speed * Time.deltaTime);
        //StartCoroutine(fireball_dir(move_dir, speed));
        transform.position = transform.position + (move_dir) * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Spider") && player != null)
        {

            Vector3 explosion_pos = transform.position;
            GameObject explosion = Instantiate(explosion_prefab, transform.position, Quaternion.identity);

            Rigidbody player_rigid = player.GetComponent<Rigidbody>();
            
            Collider[] colliders = Physics.OverlapSphere(explosion_pos, explosion_radius);
            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (collider.name == "PLAYER")
                    {
                        Debug.Log("haha");
                        Vector3 force_dir = (rb.position - explosion_pos);
                        if(force_dir.y < 0)
                        {
                            force_dir.y = rb.position.y - 0.3f;
                        }
                        
                        rb.AddForce(force_dir.normalized * 35f, ForceMode.Impulse);
                        if (health_manager.GetComponent<HealthManager>().health > 0)
                        {
                            health_manager.GetComponent<HealthManager>().health -= 10f;
                        }
                    }

                }
            }
            soundClip = GetComponent<AudioSource>().clip;
            AudioSource.PlayClipAtPoint(soundClip, explosion_pos);
            Destroy(gameObject);
            Destroy(alarm);
        }

    }
    IEnumerator fireball_dir(Vector3 end_position, float speed)
    {

        //while (this.transform.position != end_position * 2)
        while (true) 
        {

            transform.position = Vector3.MoveTowards(transform.position, end_position*1.1f, speed * Time.deltaTime);
            //transform.position = transform.position + end_position * speed * Time.deltaTime;
            yield return null;
        }

    }
    IEnumerator ChangeLightIntensity(Light spotLight)
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime / duration;
            if (t >= 1f)
            {
                float temp = minIntensity;
                minIntensity = maxIntensity;
                maxIntensity = temp;
                t = 0f;
            }
            spotLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
            yield return null;
        }
    }


}
