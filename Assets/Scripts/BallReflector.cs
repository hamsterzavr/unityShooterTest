using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReflector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float coef = 0.9f;
    public Vector3 getDir()
    {
        return transform.up;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
