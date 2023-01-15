using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace OriProject
{
    public class UIManager : MonoBehaviour
    {
        public GameObject gameOverPopup;
        public TextMeshProUGUI popupTitle;

        private bool gamePaused = false;
        public bool GamePaused { get { return gamePaused; } }

        #region Singleton

        private static UIManager instance;
        public static UIManager Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("Multiple instances of UIManager found. Deleting from " + gameObject.name);
                Destroy(this);
            }
            else
                instance = this;
        }
        #endregion

        public void ShowPopup(bool gameWon)
        {
            popupTitle.text = gameWon ? "Game Won!" : "Game Lost";
            gameOverPopup.SetActive(true);
            gamePaused = true;
        }

        public void LastCheckpointClick()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void RestartClick()
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("GameScene");
        }
    }
}