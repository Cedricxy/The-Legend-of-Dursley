using UnityEngine;
using UnityEngine.SceneManagement;
using SaveAndLoad;

namespace GameManagement
{
    public class GameMenuManager : MonoBehaviour
    {
        public GameObject mainMenu;
        
        public void OnMenuEnable()
        {
            Time.timeScale = 0f;
            if(mainMenu != null)
                mainMenu.SetActive(true);
        }

        public void OnMenuDisable()
        {
            Time.timeScale = 1f;
            if(mainMenu != null)
                mainMenu.SetActive(false);

        }
        
        public void OnSaveGame()
        {
            if (SaveGame.Instance != null)
            {
                SaveGame.SaveGameData();
            }
        }

        public void OnMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        // Öffnet den Endscreen bzw. den "Game Over" Screen.
        public void OnEndscreenNewGame()
        {
            Time.timeScale = 1f;

            // Erstellt einen neuen Spielstand.
            SaveGame.SaveDataObject newGameData = new SaveGame.SaveDataObject
            {
                IsInitialGameStart = true, // Initialer Spielstart
                HeartValue = 1,
                StarValue = 0,
                LastScene = "Tutorial"
            };
            SaveGame.SaveGameData(newGameData);
            SceneManager.LoadScene("Tutorial");
        }

        public void OnEndscreenLoadGame()
        {
            Time.timeScale = 1f;

            if (LoadGame.Instance == null)
            {
                SceneManager.LoadScene("Tutorial");
                return;
            }

            // Lädt die aktuellsten Daten aus der Datei.
            LoadGame.Instance.LoadGameData();

            string sceneToLoad;
            bool isInitialFromFile = LoadGame.Instance.IsInitialGameStart;
            int heartsFromFile = LoadGame.Instance.HeartStorageValue;
            string lastSceneFromFile = LoadGame.Instance.LastScene;

            if (isInitialFromFile || heartsFromFile <= 0)
            {
                sceneToLoad = "Tutorial";
                
                SaveGame.SaveDataObject preppedSave = new SaveGame.SaveDataObject
                {
                    IsInitialGameStart = true,
                    HeartValue = 1,
                    StarValue = 0,
                    LastScene = "Tutorial"
                };
                SaveGame.SaveGameData(preppedSave);
            }
            else
            {
                sceneToLoad = string.IsNullOrEmpty(lastSceneFromFile) ? "Tutorial" : lastSceneFromFile;
            }
            
            SceneManager.LoadScene(sceneToLoad);
        }

        public void OnEndscreenMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}