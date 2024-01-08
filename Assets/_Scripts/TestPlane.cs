using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TestPlane : MonoBehaviour
{
    public Material cubeMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "WALL";
                cube.transform.localScale = new Vector3(1, 1, 1);
                cube.transform.position = new Vector3(i, 0, j);
                //cube.GetComponent<Renderer>().material = cubeMaterial;
            }
        }
    }
}
