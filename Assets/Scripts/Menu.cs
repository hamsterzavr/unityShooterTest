using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Assets.Scripts
{
    public class Menu : MonoBehaviour
    {
        public UnityEngine.UI.Button continueBut;
        public GameObject mPannel;
        public GameObject aboutPannel;
        // Start is called before the first frame update
        void Start()
        {
            if (continueBut != null)
            {
 
                    continueBut.enabled = BallManager.checkStateFile();
                
            }
            if (mPannel != null)
            {
                mPannel.SetActive(true);
            }
            if (aboutPannel != null)
            {
                aboutPannel.SetActive(false);
            }
        }
        public void Continue()
        {
            BallManager.NewGameState = false;
            SceneManager.LoadScene(1);
        }
        // Update is called once per frame
        void Update()
        {

        }
        public void StartNewGame()
        {
            SceneManager.LoadScene(1,LoadSceneMode.Single);
        }
        public void Exit()
        {
            Application.Quit();
        }
        public void About()
        {
            if (mPannel != null)
            {
                mPannel.SetActive(false);
            }
            if (aboutPannel != null)
            {
                aboutPannel.SetActive(true);
            }
        }
        public void Back()
        {
            if (mPannel != null)
            {
                mPannel.SetActive(true);
            }
            if (aboutPannel != null)
            {
                aboutPannel.SetActive(false);
            }
        }
    }

}
