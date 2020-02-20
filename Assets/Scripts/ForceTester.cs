using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public class ForceTester : MonoBehaviour
    {
        public PhysObj obj;
        public float g = 9.8F;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            obj.ApllyForceToVelocity(Vector3.down * g * obj.mass);
        }
    }
}