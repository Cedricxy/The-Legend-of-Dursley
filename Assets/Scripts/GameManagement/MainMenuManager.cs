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

            if (ResourceStorageManager.Instance != null && SaveGame.Instance != null)
            {
                Debug.Log("<color=blue>[MainMenuManager]</color> Setze Werte für neuen Spielstand im ResourceStorageManager.");
                ResourceStorageManager.Instance.IsInitialGameStart = true; // Signal für RSM, Initialwerte zu setzen
                ResourceStorageManager.Instance.HeartStorageValue = 1;    
                ResourceStorageManager.Instance.StarStorageValue = 0;     
                ResourceStorageManager.Instance.LastScene = "Tutorial"; 

                Debug.Log("<color=blue>[MainMenuManager]</color> Speichere den neuen initialen Spielstand.");
                SaveGame.SaveGameData(); // Speichert diesen "frischen" Zustand sofort.
                                         // RSM.CheckIsInitialGameStart wird in der Tutorial-Szene diesen Zustand dann finalisieren.
            }
            else
            {
                Debug.LogError("<color=blue>[MainMenuManager]</color> ResourceStorageManager oder SaveGame Instanz nicht gefunden! Neuer Spielstand kann nicht korrekt vorbereitet werden.");
                // Wenn Instanzen fehlen, wird RSM in der Tutorial-Szene versuchen, aus einer (möglicherweise alten) Datei zu laden
                // oder Standardwerte zu verwenden, was zu unerwartetem Verhalten führen kann.
            }
            
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