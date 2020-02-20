using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public class PhysObj : MonoBehaviour
    {
        public float mass = 1;
        Vector3 velocity = Vector3.zero;
        Vector3 prevPos = Vector3.zero;
        Vector3 force = Vector3.zero;
        public bool useGravity;
        public float radius = 0.5f;
        public float tForce = 0f;
        public int cycles = 0;
        public List<float> timesU = new List<float>();
        public List<Vector3> pos = new List<Vector3>();
        Ball parentball;
        void OnCollisionEnter(Collision collision)
        {
        }
        void setVelocity(Vector3 velocity)
        {
            this.velocity = velocity;
        }
        Dictionary<object, Vector3> forces;
        Dictionary<object, float> times;
        public List<Vector3> forcesV = new List<Vector3>();

        public void ApplyForce(Vector3 force, object sender,float time)
        {
            if (forces == null)
            {
                forces = new Dictionary<object, Vector3>();
                times = new Dictionary<object, float>();
            }
            if (!forces.ContainsKey(sender))
            {
                forces.Add(sender, force);
                times.Add(sender, time);
            }
            forces[sender] = force;
            times[sender] = time;
           // Debug.Log(forces.Count);
        }
        public void ApllyForceToVelocity(Vector3 force)
        {
           
           // Debug.Log(force);
           // Debug.Log(force/mass);
            velocity += (force / mass) *Time.deltaTime;
        }
        public void ApllyForceToVelocity(Vector3 force,float dt)
        {

            // Debug.Log(force);
            // Debug.Log(force/mass);
            velocity += (force / mass) * dt;
        }
        // Start is called before the first frame update
        void Start()
        {
            forces = forces==null?new Dictionary<object, Vector3>():forces;
            toDelete = toDelete == null ? new HashSet<object>():toDelete;
            joint = GetComponent<Joint>();
            parentball = GetComponent<Ball>();
        }
        new Collider collider;
        HashSet<object> toDelete;
        Joint joint;
        public float dt = 0.1f;
        public List<Vector3> vels = new List<Vector3>();
        GameObject[] gameObjects;
        public Vector3 Velocity { get => velocity; }
        List<GameObject> tList = new List<GameObject>();
        bool lastHited = false;
        void moveObj() {
            if (collider == null)
            {
                collider = GetComponent<Collider>();
            }
            bool hited = false;
            int iters = (1 + Mathf.FloorToInt(Time.deltaTime / dt));
            Vector3 cPos = transform.position;
            Vector3 lPos = cPos;
            if (tList == null)
            {
                tList = new List<GameObject>();
            }
            tList.Clear();
            
            for (int i = 0; !hited && i < iters; i++)
            {


                var cols = Physics.OverlapSphere(transform.position, radius);
                int colsCount = 0;
                for (int j = 0; j < cols.Length; j++)
                {
                    if (!cols[j].isTrigger)
                    {
                        colsCount++;
                    }
                }
                hited = colsCount > 1;
                
                bool onlyReflects = true;
                if (hited && !lastHited)
                {
                    
                    for (int j = 0; j < cols.Length; j++)
                    {
                        BallReflector br = cols[i].GetComponent<BallReflector>();
                        if (br != null)
                        {
                            
                            Vector3 nDir = br.getDir();
                            float angle = Vector3.SignedAngle(nDir, velocity, Vector3.forward);
                            
                            velocity = Quaternion.AngleAxis(-angle, Vector3.forward) * velocity* br.coef;
                        }
                        else
                        {
                            
                            onlyReflects = cols[i]==collider;
                        }
                        
                    }
                }
                lastHited = hited;
                hited = !onlyReflects;
                
                Vector3 xforce = Vector3.zero;
                if (forces != null && gameObject.activeSelf)
                {
                    if (forces.Count > 0)
                    {
                        foreach (var force in forces)
                        {
                            if ((times[force.Key]-i*dt)>=0)
                            {
                                xforce += force.Value;
                                timesU.Add(dt);
                            }
                        }                       
                    }
                }
                if (useGravity)
                { 
                    xforce += mass * Vector3.down * BallManager.G;
                }
                forcesV.Add(xforce);
                if (!hited)
                {
                    ApllyForceToVelocity(xforce, dt);
                    cPos += velocity * dt;
                   lPos = cPos;
                }
            }
            if (forces!=null && times!=null)
            foreach (var force in forces)
                {
                    times[force.Key]-= iters * dt;
                    if (times[force.Key] < 0)
                    {
                        toDelete.Add(force.Key);
                    }
                    
                }
                foreach (var key in toDelete)
                {
                    times.Remove(key);
                    forces.Remove(key);
                }
            toDelete.Clear();
            if (!hited)
            {
                transform.position = cPos;
                
            }
            else
            {
                transform.position = lPos;
            }
            
        }
        // Update is called once per frame
        void checkForce()
        {
            force = Vector3.zero;
            if (forces.Count > 0)
            {
                //       Debug.Log("Test");
                foreach (var forceDis in forces)
                {
                    if ((times[forceDis.Key] - dt) >= 0)
                    {
                        force += forceDis.Value;
                       

                    }
                }
                if (useGravity)
                {

                    force += mass * Vector3.down * BallManager.G;
                }
    }
}
        void Update()
        {
            prevPos = transform.position;
            
            if (joint == null)
            {
                joint = GetComponent<Joint>();
            }
            if (joint == null || joint.breakForce<force.magnitude)
            {
                moveObj();
                //transform.Translate(velocity * Time.deltaTime);
            }
            else
            {
                velocity = Vector3.zero;
            }
            
            
        }
    }
}