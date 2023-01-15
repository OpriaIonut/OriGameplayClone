using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OriProject
{
    public class MainMenu : MonoBehaviour
    {
        private void Start()
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("GameScene");
        }

        public void StartGame()
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}