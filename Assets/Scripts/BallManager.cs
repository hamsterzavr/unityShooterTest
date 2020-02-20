using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml.Serialization;
using System;
namespace Assets.Scripts
{
    [Serializable]
    public class GameState
    {
        public long points;
        public int bufSize;
        public string curLevel;
        public float g;
        public GameState()
        {
            bufSize = 1000;
            points = 0;
            curLevel = "Level1.xml";
            g = 9.8f;
        }

        public GameState(long points, string curLevel,int bufSize)
        {
            this.bufSize = bufSize;
            this.points = points;
            this.curLevel = curLevel;
        }
    }
    [Serializable]
    public class ColorDiscriptor
    {
        public Color color;
        public int pointValue;

        public ColorDiscriptor()
        {
            color = Color.white;
            pointValue = 1;
        }

        public ColorDiscriptor(Color color, int pointValue)
        {
            this.color = color;
            this.pointValue = pointValue;
        }
    }
    
    [Serializable]
    public class MapDescriptor
    {
        public int shots;
        public int height, width;
        public List<ColorDiscriptor> discriptors;
        public List<int> defMap;
        public string nextLevel;
        public float sx = 10;
        public float sy = 6;
        public MapDescriptor()
        {
            defMap = new List<int>();
            for (int i = 0; i < height * width; i++)
            {
                defMap.Add(-1);
            }
        }
        public int this[int x, int y]
        {
            get
            {
                int result = -1;
                if (x>-1 && y>-1 && x<width && y < height)
                {
                    result = defMap[x + y * width];
                }
                return result;
            }
            set
            {
                if (x > -1 && y > -1 && x < width && y < height)
                {
                    defMap[x + y * width]=value>discriptors.Count?-1:value;
                }
            }
        }
        public MapDescriptor(int height, int width, List<ColorDiscriptor> discriptors)
        {
            this.height = height;
            this.width = width;
            this.discriptors = discriptors;
        }
    }
    public class BallManager : MonoBehaviour
    {
        public Text points;
        public GameObject panel;
        public GameObject turret;
        public GameObject ground;
        public Material defaultMat;
        static GameState state;
        public GameObject ballObject;
        public MapDescriptor curentMap;
     
        ColorDiscriptor curBall;
        public BallDisplay display;
        List<Ball> firstLayer;
        public static float G
        {
            get
            {
                return state!=null?state.g:9.8f;
            }
        }
        int defCount;
        public ColorDiscriptor CurBall { get
            {
                ColorDiscriptor res = curBall;
                changeColor();
                return res;
            }
        }

        static public bool NewGameState { get => newGameState; set => newGameState = value||!checkStateFile(); }

        private List<Ball> ballPool;

        static private bool newGameState = true;
        int curObj = 0;
        float radius = 0.5f;
        public void changeColor()
        {
            int color = Mathf.RoundToInt(UnityEngine.Random.value * (curentMap.discriptors.Count - 1));
            curBall = curentMap.discriptors[color];
        }
        public Ball getObjToShot()
        {
            Ball result = null;
            if (curentMap.shots > 0)
            {
                curentMap.shots--;
                while (curObj < ballPool.Count && ballPool[curObj].gameObject.activeSelf)
                {
                    curObj++;
                }
                if (curObj >= ballPool.Count)
                {
                    curObj = 0;
                }

                result = ballPool[curObj];
            }
            else
            {
                GameOver();
            }
            return result;
        }
        void loadState()
        {
            try
            {
                if (!newGameState)
                {
                    StreamReader reader = new StreamReader("State.xml");
                    XmlSerializer serializer = new XmlSerializer(typeof(GameState));
                    state = (GameState)serializer.Deserialize(reader);
                    reader.Close();
                }
                else
                {
                    state = new GameState();
                }
            }
            catch (Exception e)
            {
                state = new GameState();
            }
        }
        // Start is called before the first frame update
        void resetPool()
        {
            curObj = 0;
            for (int i = 0; i < ballPool.Count; i++)
            {
                if (ballPool[i] != null)
                {
                    ballPool[i].deactivate();
                }
            }
        }
        GameObject[] objects;
        void loadLevel(string level)
        {
            if (level != null && File.Exists(level))
            {
                StreamReader lvl = new StreamReader(level);
                XmlSerializer serializer = new XmlSerializer(typeof(MapDescriptor));
                curentMap = (MapDescriptor)serializer.Deserialize(lvl);
                lvl.Close();
                resetPool();
                firstLayer = new List<Ball>();

                for (int y = 0; y < curentMap.height; y++)
                {
                    for (int x = 0; x < curentMap.width; x++)
                    {
                        if ((curentMap[x, y] > -1) && (curentMap[x, y] < curentMap.discriptors.Count))
                        {
                            ballPool[curObj].reInit(curentMap.discriptors[curentMap[x, y]], this);
                            ballPool[curObj].gameObject.SetActive(true);
                            if (y == 0)
                            {
                                firstLayer.Add(ballPool[curObj]);
                            }
                            ballPool[curObj].gameObject.transform.position = (transform.position + Vector3.left * (curentMap.sx - x) * radius * 2 + Vector3.up * (curentMap.sy - y) * radius * 2);


                            curObj++;

                        }
                    }
                }

                for (int y = 0; y < curentMap.height; y++)
                {
                    for (int x = 0; x < curentMap.width; x++)
                    {
                        Ball obj = this[x, y];
                        if (obj != null)
                        {

                            {
                                if (y == 0)
                                {
                                    obj.gameObject.AddComponent<FixedJoint>().connectedBody = ground == null ? ground.GetComponent<Rigidbody>() : null;
                                }
                                else
                                {
                                    Ball link = this[x + 1, y];
                                    Rigidbody thisrb = obj.GetComponent<Rigidbody>();
                                    Joint lj = link != null ? link.GetComponent<Joint>() : null;
                                    if (link == null || (lj != null && lj.connectedBody == thisrb))
                                    {

                                        link = this[x - 1, y];
                                        lj = link != null ? link.GetComponent<Joint>() : null;
                                        if (link == null || (lj != null && lj.connectedBody == thisrb))
                                        {
                                            link = this[x, y - 1];

                                        }

                                    }
                                    lj = link != null ? link.GetComponent<Joint>() : null;
                                    Joint j = obj.GetComponent<Joint>();
                                    if (j == null && link != null && (lj == null || lj.connectedBody != thisrb))
                                    {
                                        link.link(obj);
                                    }
                                    else
                                    {
                                        obj.gameObject.AddComponent<FixedJoint>().connectedBody = ground == null ? ground.GetComponent<Rigidbody>() : null;

                                    }
                                }
                            }
                        }
                    }
                }
                changeColor();
                defCount = firstLayer.Count;
                Regroup();
            }
        }
        Ball this[int x, int y]
        {
            get
            {
                objects = FindObjectsOfType<GameObject>();
                Vector3 pos = (transform.position + Vector3.left * (curentMap.sx - x) * radius * 2 + Vector3.up * (curentMap.sy - y) * radius * 2);
                Ball obj = null;
                for (int i = 0; i < objects.Length; i++)
                {
                    Ball ball = objects[i].GetComponent<Ball>();
                    if (ball != null)
                    {
                        if ((objects[i].transform.position - pos).sqrMagnitude < radius*radius)
                        {
                            obj = ball;
                        }
                    }
                }
                return obj;
            }
        }
        void initPool()
        {
            int startCount = ballPool.Count;
            for (int i = 0; i < state.bufSize; i++)
            {
                if (startCount == 0)
                {
                    GameObject createdPrefab = GameObject.Instantiate(ballObject);
                    Ball ball = (Ball)(createdPrefab.GetComponent(typeof(Ball)));
                    if (ball != null && startCount == 0)
                    {
                        ballPool.Add(ball);
                        radius = ball.Radius;
                    }

                    else
                    {
                        // ball = new Ball(null);
                        BallHolder bh = createdPrefab.GetComponent<BallHolder>();
                        ball = bh.ball;
                        if (ball != null)
                        {
                            ballPool.Add(ball);

                            radius = ball.Radius;
                        }
                    }
                }
                else
                {
                    ballPool[i].deactivate();
                }
            }
        }
        public void DisposeBall(Ball ball)
        {
            if (state == null)
            {
                loadState();
            }
            if (state != null)
            {
                state.points += ball.getValue();
                Debug.Log(ball.gameObject.activeSelf);
                foreach (var j in ball.GetComponents<Joint>())
                {
                    Destroy(j);
                }
                FreeBalls();
            }
        }
        public void addPoints(int points)
        {
            state.points += points;
        }
        public void FreeBalls()
        {
            //Debug.Log(new KeyValuePair<DateTime, string>(DateTime.Now,"Free"));
           List<Rigidbody> deleteList = new List<Rigidbody>();
           for (int i = 0; i < ballPool.Count; i++)
            {
                bool act = ballPool[i].gameObject.activeSelf;
                
                bool isFixed = false;
                Joint[] joint = ballPool[i].GetComponents<Joint>();
                Stack<Rigidbody> rigidbodies = new Stack<Rigidbody>();
                for (int j = 0; j < joint.Length; j++)
                {
                    if (joint[j].gameObject.activeSelf)
                    rigidbodies.Push(joint[j].connectedBody);
                }
                HashSet<Rigidbody> visited = new HashSet<Rigidbody>();
                while (rigidbodies.Count > 0 && !isFixed)
                {
                    Rigidbody rb = rigidbodies.Pop();
                    visited.Add(rb);
                    isFixed = rb == null;
                   
                    if (!isFixed)
                    {
                        joint = rb.GetComponents<Joint>();
                        for (int j = 0; j < joint.Length; j++)
                        {
                            Rigidbody rb2 = joint[j].connectedBody;
                            bool t = rb2!=null?rb2.gameObject.activeSelf:false;
                            if (!visited.Contains(joint[j].connectedBody) && t)
                            {
                                rigidbodies.Push(joint[j].connectedBody);
                            }
                        }
                    }
                }
                if (!isFixed)
                {
                    joint = ballPool[i].GetComponents<Joint>();
                    Rigidbody rb = ballPool[i].GetComponent<Rigidbody>();
                    if (rb != null) {
                        deleteList.Add(rb);
                            }
                    for (int j = 0; j < joint.Length; j++)
                    {
                        if (joint[j] != null)
                        {
                            Destroy(joint[j]);
                        }
                    }
                }
                
            }
           for (int i = 0; i < ballPool.Count; i++)
            {
                Joint[] joint = ballPool[i].GetComponents<Joint>();
                for (int j = 0; j < joint.Length; j++)
                {
                    if (joint[j]!=null && deleteList.Contains(joint[j].connectedBody))
                    {
                        Destroy(joint[j]);
                    }
                }
            }
        }
        List<HashSet<Ball>> groups;
        List<int> oldCounts;
        void Disassemble()
        {
            for (int i = 0; i < ballPool.Count; i++)
            {
                foreach (var joint in ballPool[i].gameObject.GetComponents<Joint>())
                { 
                Rigidbody rg = ballPool[i].GetComponent<Rigidbody>();
                if (joint != null)
                {
                    Destroy(joint);
                }

            } 
            }
        }
        public void Regroup()
        {

            if (groups == null)
            {
                groups = new List<HashSet<Ball>>();
            }
            for (int i = 0; i < ballPool.Count; i++)
            {
                if (ballPool[i].gameObject.activeSelf)
                {
                    if (ballPool[i].NoGroup)
                    {
                        groups.Add(ballPool[i].recreateGroup());
                    }
                    else
                    {
                        if (!groups.Contains(ballPool[i].Group))
                        {
                            groups.Add(ballPool[i].Group);
                        }
                    }
                    Collider[] cols = Physics.OverlapSphere(ballPool[i].transform.position,ballPool[i].Radius*2f);
                    for (int j = 0; j < cols.Length; j++)
                    {
                        
                        Ball ball = cols[j].GetComponent<Ball>();
                        if (ball != null)
                        {
                            if (ball.Discriptor == ballPool[i].Discriptor)
                            {
                                ballPool[i].groupObjs(ball);
                            }
                        }
                    }
                }
            }
            for (int i = 0; oldCounts!=null && i < groups.Count; i++)
            {
                if (i<oldCounts.Count &&  groups[i].Count != oldCounts[i])
                {
                    if (groups[i].Count > 2)
                    {
                        foreach (var ball in groups[i])
                        {
                            DisposeBall(ball);
                        }
                    }
                }
            }
            oldCounts = new List<int>();
            groups.RemoveAll((HashSet<Ball> a) => { return a.Count == 0; });
            for (int i = 0; i < groups.Count; i++)
            {
                oldCounts.Add(groups[i].Count);
            }
            firstLayer.RemoveAll((Ball a) => { return !a.gameObject.activeSelf; });
            if (firstLayer.Count * 1.0f / defCount < 0.34f)
            {
                Disassemble();
            }

        }
        public void GameOver()
        {
            resetPool();
            if (points != null)
            {
                points.gameObject.SetActive (false);
            }
            if (display != null)
            {
                display.gameObject.SetActive(false);
            }
            if (panel != null)
            {
                panel.SetActive(true);
            }
            if (display != null)
            {
                display.gameObject.SetActive(false);
            }
        }
        public void Retry()
        {

            SceneManager.LoadScene(1);
        }
        public void ToMenu()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        void Start()
        {
              
            ballPool = new List<Ball>();
            loadState();
            initPool();
            loadLevel(state.curLevel);
            if (panel != null)
            {
                panel.SetActive(false);
            }
            if (display != null)
            {
                display.gameObject.SetActive(true);
            }

        }
        public void Win()
        {
            state.curLevel = curentMap.nextLevel;
            XmlSerializer serializer = new XmlSerializer(typeof(GameState));
            StreamWriter sw = new StreamWriter("State.xml");
            serializer.Serialize(sw, state);
            sw.Close();
            initPool();
            loadLevel(state.curLevel);
            if (panel != null)
            {
                panel.SetActive(false);
            }
            if (display != null)
            {
                display.gameObject.SetActive(true);
            }
        }
   
        static public bool checkStateFile()
        {
            return File.Exists("State.xml");
        }
        // Update is called once per frame
        bool NoBallsLeft()
        {
            bool noObjs = true;
            for (int i = 0;noObjs && i < ballPool.Count; i++)
            {
                noObjs = !ballPool[i].gameObject.activeSelf;
            }
            return noObjs;
        }
        void Update()
        {
            if (points != null && state!=null)
            {
                points.text = "" + state.points;
            }
            if (display != null && curentMap != null)
            {
                display.changeColor(curBall, curentMap.shots);
            }
            if (NoBallsLeft() && !panel.activeSelf)
            {
                Win();
            }
        }
    }
}
