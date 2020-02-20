using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public class Bin : MonoBehaviour
    {
        public int pointValue;
        public BallManager manager;
        // Start is called before the first frame update
        void Start()
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            Ball ball = other.GetComponent<Ball>();
            if (ball != null && manager!=null)
            {
                manager.addPoints(pointValue);
                ball.deactivate();
                manager.Regroup();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}