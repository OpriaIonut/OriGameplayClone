using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OriProject
{
    public class SceneSaver : MonoBehaviour
    {
        public PlayerLogic playerLogic;

        private bool checkedSavedData = false;

        private void Update()
        {
            if(!checkedSavedData)
            {
                checkedSavedData = true;
                CheckSaveData();
            }
        }

        private void CheckSaveData()
        {
            if(PlayerPrefs.HasKey("Player"))
            {
                string data = PlayerPrefs.GetString("Player");
                string[] items = data.Split('_');

                float damage = playerLogic.maxHealth - float.Parse(items[0]);
                playerLogic.TakeDamage(damage, transform, false);

                playerLogic.transform.position = new Vector3(float.Parse(items[1]), float.Parse(items[2]), float.Parse(items[3]));
                playerLogic.transform.rotation = new Quaternion(float.Parse(items[4]), float.Parse(items[5]), float.Parse(items[6]), float.Parse(items[7]));

                EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
                for(int index = 0; index < enemies.Length; index++)
                {
                    if (!PlayerPrefs.HasKey(enemies[index].gameObject.name))
                    {
                        if (enemies[index].GetType() == typeof(EnemyColony))
                        {
                            EnemyColony script = (EnemyColony)enemies[index];
                            script.KillCompletely();
                        }
                        else
                            enemies[index].TakeDamage(1000);
                    }
                    else
                    {
                        data = PlayerPrefs.GetString(enemies[index].gameObject.name);
                        items = data.Split('_');

                        damage = enemies[index].maxHealth - float.Parse(items[0]);
                        enemies[index].TakeDamage(damage);

                        enemies[index].transform.position = new Vector3(float.Parse(items[1]), float.Parse(items[2]), float.Parse(items[3]));
                        enemies[index].transform.rotation = new Quaternion(float.Parse(items[4]), float.Parse(items[5]), float.Parse(items[6]), float.Parse(items[7]));
                    }
                }

                Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
                for (int index = 0; index < checkpoints.Length; index++)
                {
                    if (PlayerPrefs.HasKey(checkpoints[index].gameObject.name))
                    {
                        string value = PlayerPrefs.GetString(checkpoints[index].gameObject.name);
                        if (value == "True")
                            checkpoints[index].ChangeParticleDisplay();
                    }
                }
            }
        }

        public static void SaveGameData()
        {
            PlayerPrefs.DeleteAll();

            EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
            for(int index = 0; index < enemies.Length; index++)
            {
                string data = "" + enemies[index].GetCurrentHealth() + "_" + enemies[index].transform.position.x + "_" + enemies[index].transform.position.y + "_" + enemies[index].transform.position.z + "_" + enemies[index].transform.rotation.x + "_" + enemies[index].transform.rotation.y + "_" + enemies[index].transform.rotation.z + "_" + enemies[index].transform.rotation.w;

                PlayerPrefs.SetString(enemies[index].gameObject.name, data);
            }

            PlayerLogic playerScript = FindObjectOfType<PlayerLogic>();
            string playerData = "" + playerScript.GetCurrentHealth() + "_" + playerScript.transform.position.x + "_" + playerScript.transform.position.y + "_" + playerScript.transform.position.z + "_" + playerScript.transform.rotation.x + "_" + playerScript.transform.rotation.y + "_" + playerScript.transform.rotation.z + "_" + playerScript.transform.rotation.w;

            PlayerPrefs.SetString("Player", playerData);

            Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
            for(int index = 0; index < checkpoints.Length; index++)
            {
                PlayerPrefs.SetString(checkpoints[index].gameObject.name, "" + checkpoints[index].wasUsed);
            }

            PlayerPrefs.Save();
        }

        public static void ReloadScene()
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}