using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts
{
    class Turret:MonoBehaviour
    {
        ForceGen forceGenMain;
        public BallManager manager;
        public float maxForce = 5;
        public int coolDown=10;
        int frames = 0;
        void FixedUpdate()
        {
            Vector3 endPos = Vector3.zero;
            bool shoot;
            if (Input.touchCount < 1)
            {
                endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                shoot = Input.GetMouseButtonDown(0);
                //Debug.Log(shoot);
            }
            else
            {
                Touch touch = Input.GetTouch(0);
                shoot = touch.phase == TouchPhase.Ended;
            }
            endPos.z = gameObject.transform.position.z;
            forceGenMain.setPos(endPos);
            gameObject.transform.LookAt(endPos);
            if (shoot && frames==0)
            {
                frames = 1;
                forceGenMain.Shoot();
            }
            if (frames > coolDown)
            {
                frames = 0;
            }
            if (frames > 0)
            {
                frames++;
            }
        }
        private void Start()
        {
            forceGenMain = GetComponent<ForceGen>();
            if (forceGenMain == null)
            {
                forceGenMain = (ForceGen) gameObject.AddComponent(typeof(ForceGen));
            }
            forceGenMain.manager = manager;
        }
    }
}
