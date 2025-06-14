using UnityEngine;
using UnityEngine.SceneManagement;
using ResourceSystem.Storage;
using SaveAndLoad;

namespace GameManagement
{
    public class MainMenuManager : MonoBehaviour
    {
        public GameObject mainMenuCanvas; // Umbenannt für Klarheit, falls es ein spezifisches Canvas ist

        public void OnPlayGame()
        {
            Time.timeScale = 1f;
            string sceneToLoad = "Tutorial"; // Fallback-Szene

            if (LoadGame.Instance != null)
            {
                // Stelle sicher, dass die neuesten Daten geladen sind, bevor die Szene gelesen wird
                LoadGame.Instance.LoadGameData(); 
                if (!string.IsNullOrEmpty(LoadGame.Instance.LastScene))
                {
                    sceneToLoad = LoadGame.Instance.LastScene;
                }
                Debug.Log($"<color=blue>[MainMenuManager]</color> OnPlayGame: Lade Szene '{sceneToLoad}' basierend auf LoadGame.Instance.LastScene.");
            }
            else if (ResourceStorageManager.Instance != null && !string.IsNullOrEmpty(ResourceStorageManager.Instance.LastScene))
            {
                // Fallback, falls LoadGame.Instance noch nicht bereit ist, aber RSM schon existiert (weniger wahrscheinlich)
                sceneToLoad = ResourceStorageManager.Instance.LastScene;
                Debug.Log($"<color=blue>[MainMenuManager]</color> OnPlayGame: Lade Szene '{sceneToLoad}' basierend auf ResourceStorageManager.Instance.LastScene (Fallback).");
            }
            else
            {
                Debug.LogWarning($"<color=blue>[MainMenuManager]</color> OnPlayGame: Weder LoadGame noch ResourceStorageManager Instanz mit LastScene gefunden. Lade Fallback-Szene '{sceneToLoad}'.");
            }
            
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(false);
            SceneManager.LoadScene(sceneToLoad);      
        }
        
        public void OnQuitGame()
        {
            Debug.Log("<color=blue>[MainMenuManager]</color> OnQuitGame: Beende Anwendung.");
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stoppt das Spiel im Editor
            #endif
        }

        public void OnNewGame()
        {
            Time.timeScale = 1f;
            Debug.Log("<color=blue>[MainMenuManager]</color> OnNewGame aufgerufen.");

            // NEU: Signalisiere dem GameController/RSM, dass ein vollständiger Reset für ein neues Spiel erfolgen soll.
            GameController.ForceNewGameReset = true;
            Debug.Log("<color=blue>[MainMenuManager]</color> GameController.ForceNewGameReset auf true gesetzt.");

            // Die eigentliche Logik zum Setzen der Werte im RSM und Speichern
            // wird jetzt idealerweise vom RSM selbst in OnSceneLoaded/CheckInitialGameStart gehandhabt,
            // wenn ForceNewGameReset true ist.
            // Wir laden hier nur noch die Tutorial-Szene.
            // Die vorherige explizite Speicherung hier kann potenziell zu Konflikten führen,
            // wenn der RSM die Werte beim Laden der Tutorial-Szene sofort wieder überschreibt.

            // if (ResourceStorageManager.Instance != null && SaveGame.Instance != null)
            // {
            //     Debug.Log("<color=blue>[MainMenuManager]</color> Setze Werte für neuen Spielstand im ResourceStorageManager und erstelle SaveDataObject.");
            //     ResourceStorageManager.Instance.IsInitialGameStart = true; 
            //     ResourceStorageManager.Instance.HeartStorageValue = 1;    
            //     ResourceStorageManager.Instance.StarStorageValue = 0;     
            //     ResourceStorageManager.Instance.LastScene = "Tutorial"; 
            //     SaveGame.SaveDataObject newGameData = new SaveGame.SaveDataObject
            //     {
            //         IsInitialGameStart = true, 
            //         HeartValue = 1,
            //         StarValue = 0,
            //         LastScene = "Tutorial"
            //     };
            //     Debug.Log("<color=blue>[MainMenuManager]</color> Speichere den neuen initialen Spielstand explizit.");
            //     SaveGame.SaveGameData(newGameData); 
            // }
            // else
            // {
            //     Debug.LogError("<color=blue>[MainMenuManager]</color> ResourceStorageManager oder SaveGame Instanz nicht gefunden! Neuer Spielstand kann nicht korrekt vorbereitet werden.");
            // }
            
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(false);
            Debug.Log("<color=blue>[MainMenuManager]</color> Lade Tutorial-Szene für neuen Spielstand.");
            SceneManager.LoadScene("Tutorial");
        }

        public void OnLoadGame()
        {
            Debug.Log("<color=blue>[MainMenuManager]</color> OnLoadGame aufgerufen. Verhalten ist identisch zu OnPlayGame.");
            // Ruft OnPlayGame auf, da die Logik dieselbe sein soll: Lade den letzten Spielstand.
            // Die Logik im RSM kümmert sich um den Fall mit 0 Herzen.
            OnPlayGame(); 
        }
    }
}