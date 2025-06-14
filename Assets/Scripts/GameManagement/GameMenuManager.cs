using UnityEngine;
using UnityEngine.SceneManagement;
using ResourceSystem.Storage;
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
                Debug.Log("<color=yellow>[GameMenuManager]</color> Spiel gespeichert über Menü.");
            }
            else
            {
                Debug.LogError("<color=yellow>[GameMenuManager]</color> SaveGame Instanz nicht gefunden! Spiel konnte nicht gespeichert werden.");
            }
        }

        public void OnMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        // Neue Methoden für den Endscreen

        public void OnEndscreenNewGame()
        {
            Time.timeScale = 1f;
            Debug.Log("<color=yellow>[GameMenuManager (Endscreen)]</color> OnEndscreenNewGame aufgerufen.");

            // Erstelle einen komplett neuen, sauberen Spielstand für das Tutorial
            SaveGame.SaveDataObject newGameData = new SaveGame.SaveDataObject
            {
                IsInitialGameStart = true, // Signalisiert dem RSM, dass es ein initialer Setup ist
                HeartValue = 1,
                StarValue = 0,
                LastScene = "Tutorial"
            };
            SaveGame.SaveGameData(newGameData);
            Debug.Log("<color=yellow>[GameMenuManager (Endscreen)]</color> Neuer initialer Spielstand für Tutorial gespeichert.");
            
            SceneManager.LoadScene("Tutorial");
        }

        public void OnEndscreenLoadGame()
        {
            Time.timeScale = 1f;
            Debug.Log("<color=yellow>[GameMenuManager (Endscreen)]</color> OnEndscreenLoadGame aufgerufen.");

            if (LoadGame.Instance == null)
            {
                Debug.LogError("<color=yellow>[GameMenuManager (Endscreen)]</color> OnEndscreenLoadGame: LoadGame.Instance ist null! Lade Fallback 'Tutorial'.");
                SceneManager.LoadScene("Tutorial");
                return;
            }

            // Lade die aktuellsten Daten aus der Datei
            LoadGame.Instance.LoadGameData();

            string sceneToLoad;
            bool isInitialFromFile = LoadGame.Instance.IsInitialGameStart;
            int heartsFromFile = LoadGame.Instance.HeartStorageValue;
            string lastSceneFromFile = LoadGame.Instance.LastScene;

            if (isInitialFromFile || heartsFromFile <= 0)
            {
                Debug.Log($"<color=yellow>[GameMenuManager (Endscreen)]</color> OnEndscreenLoadGame: Neustart-Bedingung erkannt (Initial: {isInitialFromFile}, Herzen: {heartsFromFile}). Lade 'Tutorial'.");
                sceneToLoad = "Tutorial";
                // Bereite den Speicherstand für einen sauberen Start im Tutorial vor
                SaveGame.SaveDataObject preppedSave = new SaveGame.SaveDataObject
                {
                    IsInitialGameStart = true, // Signal für RSM, dass es ein 'neuer' Start ist
                    HeartValue = 1,          // Start mit 1 Herz
                    StarValue = 0,           // Sterne auf 0 bei Tod/Neustart
                    LastScene = "Tutorial"   // Ziel ist Tutorial
                };
                SaveGame.SaveGameData(preppedSave);
            }
            else
            {
                sceneToLoad = string.IsNullOrEmpty(lastSceneFromFile) ? "Tutorial" : lastSceneFromFile;
                Debug.Log($"<color=yellow>[GameMenuManager (Endscreen)]</color> OnEndscreenLoadGame: Lade letzte Szene '{sceneToLoad}'.");
            }
            
            SceneManager.LoadScene(sceneToLoad);
        }

        public void OnEndscreenMainMenu()
        {
            Time.timeScale = 1f;
            Debug.Log("<color=yellow>[GameMenuManager (Endscreen)]</color> OnEndscreenMainMenu aufgerufen. Lade MainMenu.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}