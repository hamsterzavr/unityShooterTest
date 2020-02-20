using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public class ForceGen : MonoBehaviour
    {
        public float dt = 0.1F;
        // Start is called before the first frame update
        public Camera mainCamera;
        public TrajectoryRenderer rendMain;
        public TrajectoryRenderer rendLeft;
        public TrajectoryRenderer rendRight;
        public GameObject barrel;
        public float spread;
        public float maxForce;
        public float radius;
        public float timeOfForce=0.5f;
        bool isMaxForce;
        public BallManager manager;
        Vector3 endPos=Vector3.zero;
        public void setPos(Vector3 pos)
        {
            
            endPos = pos;

        }
        Vector3 genForce(double t)
        {

           
            Vector3 force = (barrel.transform.position - transform.position);
            
            
            force.z = 0;
            float forceValue = (endPos-transform.position).magnitude*50;
            isMaxForce = forceValue >= maxForce;
            forceValue = isMaxForce ? maxForce : forceValue;
            return force.normalized*forceValue;
        }
        Vector3 GenForceLeftSpred(double t)
        {
            
            Vector3 actForce = rotateVector(genForce(t), spread);
            float forceValue = actForce.magnitude;
            bool isMax = forceValue >= maxForce;
            forceValue = isMax ? maxForce : forceValue;
            return actForce.normalized * forceValue;
        }
        Vector3 rotateVector(Vector3 vector,float angle)
        {

            return Quaternion.AngleAxis(angle,Vector3.forward)*vector;
        }
        Vector3 GenForceRightSpred(double t)
        {

            Vector3 actForce = rotateVector(genForce(t), -spread);
            float forceValue = actForce.magnitude;
            bool isMax = forceValue >= maxForce;
            forceValue = isMax ? maxForce : forceValue;
            return actForce.normalized * forceValue;
        }
        public void Shoot()
        {
            float rnd = (Random.value * 2 - 1)*spread;

            Vector3 actForce = genForce(0);             
            float forceValue = actForce.magnitude;
            Debug.Log(forceValue);
            bool max = forceValue >= maxForce;
            if (max)
            {
                actForce = rotateVector(actForce, rnd);
            }
            Ball ballToShoot = manager.getObjToShot();
            if (ballToShoot != null)
            {
                GameObject g = ballToShoot.gameObject;
                if (!g.activeSelf)
                {

                    ballToShoot.transform.position = barrel != null ? barrel.transform.position : transform.position;
                    ballToShoot.reInit(manager.CurBall, manager);

                    if (isMaxForce)
                    {
                        ballToShoot.setHight();
                    }
                    ballToShoot.phis.ApplyForce(actForce, this, timeOfForce);
                }
            }

        }
        void Start()
        {
            isMaxForce = false;
            TrajectoryRenderer[] renderers = GetComponents<TrajectoryRenderer>();
            LineRenderer[] lines = GetComponents<LineRenderer>();
            if (rendMain == null)
            {
                rendMain = renderers.Length > 0 && lines.Length > 0 ? renderers[0] : null;
                if (rendMain != null)
                {
                    rendMain.dt =dt;
                    rendMain.time = timeOfForce;
                    rendMain.a = lines[0];
                    rendMain.radius = radius;
                }
                else
                {
                    rendMain = (TrajectoryRenderer)gameObject.AddComponent(typeof(TrajectoryRenderer));
                    rendMain.time = timeOfForce;
                    rendMain.dt =dt;
                    rendMain.a = (LineRenderer)gameObject.AddComponent(typeof(LineRenderer));

                    rendMain.radius = radius;
                }

            }
            if (rendLeft==null)
            {
                rendLeft = renderers.Length > 1 && lines.Length > 1 ? renderers[1] : null;
                if (rendLeft != null)
                {
                    rendLeft.dt =dt;
                    rendLeft.time = timeOfForce;
                    rendLeft.a = lines[1];
                    rendLeft.radius = radius;
                }
                else
                {
                    rendLeft = (TrajectoryRenderer)gameObject.AddComponent(typeof(TrajectoryRenderer));
                    rendLeft.time = timeOfForce;
                    rendLeft.dt =dt;
                    rendLeft.a = (LineRenderer)gameObject.AddComponent(typeof(LineRenderer));
                    rendLeft.radius = radius;
                }

            }
            if (rendRight==null)
            {
                rendRight = renderers.Length > 2 && lines.Length > 2 ? renderers[2] : null;
                if (rendRight != null)
                {
                    rendRight.dt =dt;
                    rendRight.time = timeOfForce;
                    rendRight.a = lines[2];
                    rendRight.radius = radius;
                }
                else
                {
                    rendRight = (TrajectoryRenderer)gameObject.AddComponent(typeof(TrajectoryRenderer));
                    rendRight.time = timeOfForce;
                    rendRight.dt =dt;
                    rendRight.a = (LineRenderer)gameObject.AddComponent(typeof(LineRenderer));
                    rendRight.radius = radius;
                }

            }
        }

        // Update is called once per frame
        public Vector3 f;
        void Update()
        {
            if (rendMain != null && rendLeft!=null && rendRight!=null)
            {
                rendMain.force = genForce;
                rendLeft.force = GenForceLeftSpred;
                rendRight.force = GenForceRightSpred;
                rendMain.dt =dt;
                rendMain.time = timeOfForce;
                rendLeft.dt =dt;
                rendLeft.time = timeOfForce;
                rendRight.dt =dt;
                rendRight.time = timeOfForce;
                rendRight.SetState(isMaxForce);
                rendLeft.SetState(isMaxForce);
                rendMain.SetState(!isMaxForce);
            }
            f = genForce(0);
        }
    }
}