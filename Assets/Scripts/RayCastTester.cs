using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit rHit;
        bool hited = Physics.Raycast(transform.position, Vector3.down, 1);
     
        Renderer rd = GetComponent<Renderer>();
        if (rd != null)
        {
            if (hited)
            {
                rd.material.SetColor("_Color", Color.red);
            }
            else
            {
                rd.material.SetColor("_Color", Color.green);
            }
        }
        
    }
}
