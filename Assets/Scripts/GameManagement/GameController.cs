using UnityEngine;
using UnityEngine.SceneManagement;
using ResourceSystem.Storage;
using SaveAndLoad;

namespace GameManagement
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        public static bool ForceNewGameReset = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
                ObjectCounter.OnSceneObjectivesCompleted += HandleSceneObjectivesCompleted;
                Player.PlayerAnimation.OnPlayerDied += HandlePlayerDied;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                ObjectCounter.OnSceneObjectivesCompleted -= HandleSceneObjectivesCompleted;
                Player.PlayerAnimation.OnPlayerDied -= HandlePlayerDied;
            }
        }

        private void HandleSceneObjectivesCompleted()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            string nextSceneToLoad = "";
            if (currentSceneName == "Tutorial")
            {
                nextSceneToLoad = "Kapitel1";
            }
            else if (currentSceneName == "Kapitel1")
            {
                nextSceneToLoad = "Kapitel2";
            }
            else if (currentSceneName == "Kapitel2")
            {
                nextSceneToLoad = "Endscreen";
            }

            if (!string.IsNullOrEmpty(nextSceneToLoad))
            {
                if (ResourceStorageManager.Instance != null)
                {
                    ResourceStorageManager.Instance.LastScene = nextSceneToLoad;
                }

                // Speichert den Spielstand, nachdem LastScene im ResourceStorageManager aktualisiert wurde.
                if (SaveGame.Instance != null) 
                {
                    SaveGame.SaveGameData(); 
                }
                
                SceneManager.LoadScene(nextSceneToLoad);
            }
        }

        // Methode für den Spieltod.
        private void HandlePlayerDied()
        {
            if (ResourceStorageManager.Instance != null && SaveGame.Instance != null)
            {
                ResourceStorageManager.Instance.IsInitialGameStart = true;
                ResourceStorageManager.Instance.HeartStorageValue = 0;
                ResourceStorageManager.Instance.StarStorageValue = 0;
                ResourceStorageManager.Instance.LastScene = "Tutorial";
                
                SaveGame.SaveDataObject deathSaveData = new SaveGame.SaveDataObject
                {
                    IsInitialGameStart = ResourceStorageManager.Instance.IsInitialGameStart,
                    HeartValue = ResourceStorageManager.Instance.HeartStorageValue,
                    StarValue = ResourceStorageManager.Instance.StarStorageValue,
                    LastScene = ResourceStorageManager.Instance.LastScene
                };
                SaveGame.SaveGameData(deathSaveData); 
            }
            
            LoadEndScreen();
        }

        public void LoadNextLevel()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            string nextScene = "";

            if (currentScene == "Tutorial")
            {
                nextScene = "Kapitel1";
            }
            else if (currentScene == "Kapitel1")
            {
                nextScene = "Kapitel2";
            }
            else if (currentScene == "Kapitel2")
            {
                nextScene = "Endscreen";
            }

            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
        }

        public void LoadEndScreen()
        {
            SceneManager.LoadScene("Endscreen"); 
        }
    }
}