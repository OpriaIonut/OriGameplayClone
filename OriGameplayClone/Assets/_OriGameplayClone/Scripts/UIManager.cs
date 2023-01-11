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

        public void ShowPopup(bool gameWon)
        {
            popupTitle.text = gameWon ? "Game Won!" : "Game Lost";
            gameOverPopup.SetActive(true);
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