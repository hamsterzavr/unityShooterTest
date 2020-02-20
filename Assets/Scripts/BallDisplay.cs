using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{

    public class BallDisplay : MonoBehaviour
    {
        public Text textField;
        int cBalls = 0;
        // Start is called before the first frame update
        void Start()
        {

        }
        public void changeColor(ColorDiscriptor cDis,int ballCount)
        {
            Image rnd = GetComponent<Image>();
            if (rnd != null && cDis!=null)
            {
                rnd.color = cDis.color;
                cBalls = ballCount;
                if (textField != null)
                {
                    textField.text = "" + cBalls;
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}