using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public delegate Vector3 Force(double t);
    
    public class TrajectoryRenderer : MonoBehaviour
    {
        public Material mat;
        public Color color = Color.white;
        public void SetState(bool state)
        {

            if (!state)
            {
                a.positionCount=0;
               
            }
        }
        GameObject[] gameObjects;
        // Start is called before the first frame update
        void Start()
        {
            gameObjects = FindObjectsOfType<GameObject>();
            if (startObj == null)
            {
                startObj = gameObject;
            }
            if (a == null)
            {
                a = GetComponent<LineRenderer>();
                if (a == null)
                {
                    a = gameObject.AddComponent<LineRenderer>();
                    a.startWidth = 0.1f;
                    a.endWidth = 0.1f;
                    a.startColor = color;
                    a.endColor = color;
                    if (mat != null)
                    {
                        a.material = mat;
                    }
                }
            }
        }
        public LineRenderer a = new LineRenderer();
        Vector3 startPos = Vector3.zero;
        public GameObject startObj;
        public GameObject traj;
        public Force force;
        public int cycles;
        public double tForce=0;
        public float g=9.8f;
        public List<Vector3> forcesV = new List<Vector3>();
        public float mass;
        public List<Vector3> vels = new List<Vector3>();
        public void setConstForce(Vector3 force)
        {
            this.force = (double t)=>{ return force; };
        }
        public float time = 0;
        public void SetTime(float newTime)
        {
            time = newTime;
        }
        public float radius=0.5f;
        public float dt = 0.1f;
        List<Collider> tList = new List<Collider>();
        List<Vector3> generateTraj(float dt)
        {
            
            List<Vector3> result = new List<Vector3>();
            Vector3 vel = Vector3.zero;
            Vector3 curPos = startPos;
            float t = 0;
            cycles = 0;
            bool hited=false;
            if (tList == null)
            {
                tList = new List<Collider>();
            }
            tList.Clear();
            if (gameObjects == null)
            {
                gameObjects = FindObjectsOfType<GameObject>();
            }
            for (int i = 0;i<gameObjects.Length;i++)
            {
                if (gameObjects[i].layer != 8)
                {
                    Collider col = gameObjects[i].GetComponent<Collider>();
                    if (col != null)
                    {
                        tList.Add(col);
                    } 
                }
            }
            do
            {
                for (int i = 0;!hited && i < tList.Count; i++)
                {
                    // hited = tList[i].bounds.Contains(curPos) && !tList[i].isTrigger;
                    hited = (tList[i].bounds.ClosestPoint(curPos)-curPos).magnitude<=radius && !tList[i].isTrigger;
                    if (hited)
                    {
                        BallReflector br = tList[i].GetComponent<BallReflector>();
                        if (br != null)
                        {
                            hited = false;
                            Vector3 nDir = br.getDir();
                            float angle = Vector3.SignedAngle(nDir, vel, Vector3.forward);
                            vel = Quaternion.AngleAxis(-angle, Vector3.forward) * vel*br.coef;
                        }
                    }
                }
                Vector3 curForce = Vector3.zero;
                t = dt * cycles;
                if (t <= time && force != null)
                {
                    cycles++;
                    curForce = force(t) + Vector3.down * g * mass;
                }
                else
                {
                    curForce = Vector3.down * g * mass;

                }
                vel += (curForce / mass) * dt;;
                result.Add(curPos);
                curPos += vel*dt;
               
            }
            while ((curPos.y>-100 && !hited && time<1));
            tForce = dt * cycles;
            return result;
        }
        // Update is called once per frame
        Vector3 oldForce = Vector3.zero;
        List<Vector3> position;
        //int i = 0;
        void Update()
        {
            if (a != null)
            {
                startPos = startObj.transform.position;



                if (oldForce==null ||force==null|| (oldForce != force(0)))
                {
                    
                    position = generateTraj(Mathf.Min(Time.deltaTime,dt));
                }
                oldForce = force!=null?force(0):Vector3.zero;
                if (position != null)
                {
                    a.positionCount = position.Count;
                    a.SetPositions(position.ToArray()); 
                }
            }
        }
    }
}
