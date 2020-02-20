using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{

    public class Ball : MonoBehaviour
    {
        public bool NoGroup
        {
            get
            {
                return group == null;
            }
        }
        ColorDiscriptor discriptor;
        HashSet<Ball> group;
        HashSet<Ball> links;
        public GameObject exp;
        public HashSet<Ball> recreateGroup()
        {
            group = new HashSet<Ball>();
            group.Add(this);
            return group;
        }
        public PhysObj phis;
        public bool HightEnergy { get => hightEnergy; }
        public ColorDiscriptor Discriptor { get => discriptor; set => discriptor = discriptor==null?value:discriptor; }
        public BallManager manager;
        public void setHight()
        {
            hightEnergy = true;
        }
        public void setLow()
        {
            hightEnergy = false;
        }
        bool hightEnergy;
        public void link(Ball another)
        {
            Joint[] joints = GetComponents<Joint>();
            bool cantBeLinked = false;
            for (int i = 0; i < joints.Length; i++)
            {
                if (joints[i].connectedBody == null)
                {
                    cantBeLinked = true;
                }
                if (joints[i].connectedBody== another.GetComponent<Rigidbody>())
                {
                    cantBeLinked = false;
                }
            }
           
                if (links == null)
                {
                    links = new HashSet<Ball>();
                }
                if (!links.Contains(another))
                {
                    links.Add(another);
                if (!cantBeLinked)
                    {
                    Joint j = gameObject.AddComponent<FixedJoint>();
                    j.connectedBody = another.gameObject.GetComponent<Rigidbody>();
                    manager.Regroup();
                }
            }
        }

        public void groupObjs(Ball another)
        {
            if (group==null && another.group != null)
            {
                group = another.group;
                group.Add(this);
            }
            if (group != null)
            {
                if (another.group == null)
                {
                    another.group = group;
                    group.Add(another);
                }
                else
                {
                    
                    
                    foreach (var b in another.group)
                    {
                        if (!group.Add(b))
                        group.Add(b);
                    }
                    foreach (var b in another.group)
                    {
                        b.group = group;
                    }
                    another.group = group;
                }
            }
        }
        public int groupSize
        {
            get
            {
                return group.Count;
            }
        }
        public float Radius
        {
            get
            {
                return collider != null ? collider.bounds.size.x / 1.8f : 0.5f;
            }
        }

        public HashSet<Ball> Group { get => group;  }

        public Ball(ColorDiscriptor discriptor)
        {
            this.discriptor = discriptor;
            if (phis == null)
            {
                phis = new PhysObj();
            }
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            
        }
        public void reInit(ColorDiscriptor discriptor,BallManager manager)
        {
            gameObject.SetActive(true);
            this.discriptor = discriptor;
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = discriptor.color;
            }
            this.manager = manager;
        }
        new Collider collider;
        public void deactivate()
        {
            gameObject.SetActive(false);
            if (links != null)
            {
                links.Clear();
            }
        }
        public int getValue()
        {
            deactivate();
           // GameObject a =  GameObject.Instantiate(exp,transform);
            
            Renderer renderer = exp.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (renderer.material!=null)
                renderer.material.color = discriptor.color;
            }
            exp.SetActive(true);
            exp.transform.position = transform.position;
            return discriptor==null?discriptor.pointValue:0;
        }
        // Start is called before the first frame update
        void Start()
        {
            collider = gameObject.GetComponent<Collider>();
            if (phis == null)
            {
                phis = new PhysObj();
            }
            if (discriptor == null)
            {
                deactivate();

            }
            else
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material material = renderer.material;
                    if (material == null)
                    {
                        renderer.material = manager.defaultMat;
                    }
                    if (material != null)
                    {
                      
                            renderer.material.color = discriptor.color;
                    }
                    
                }
            }
        }
        
        // Update is called once per frame
        void FixedUpdate()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position,Radius, 1);
            for (int i = 0; i < colliders.Length; i++)
            {
                Ball ball = colliders[i].GetComponent<Ball>();
                if (ball != null && colliders[i]!=collider)
                {
                    if (HightEnergy)
                    {
                        ball.manager.DisposeBall(ball);
                        setLow();
                        phis.ApplyForce( -phis.Velocity/2,ball,Time.deltaTime);
                        

                    }
                    else
                    {
                        ball.link(this);
                    }
                }
            }
        }
    }
}