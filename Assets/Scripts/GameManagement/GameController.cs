using UnityEngine;
using UnityEngine.SceneManagement; // Hinzugefügt für SceneManager
using ResourceSystem.Storage; // Für ResourceStorageManager
using SaveAndLoad; // Für SaveGame

namespace GameManagement
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        public static bool ForceNewGameReset = false; // NEU: Statisches Flag für Reset

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.parent = null; // Sicherstellen, dass es ein Root-Objekt ist
                DontDestroyOnLoad(gameObject); // Behält den GameController über Szenenwechsel hinweg
                ObjectCounter.OnSceneObjectivesCompleted += HandleSceneObjectivesCompleted; // Event abonnieren
                Debug.Log("[GameController] Instance created and subscribed to OnSceneObjectivesCompleted.");
            }
            else
            {
                Debug.LogWarning("[GameController] Duplicate instance detected. Destroying new one.");
                Destroy(gameObject);
            }
        }

        private void OnDestroy() // Wichtig, um Memory Leaks zu vermeiden
        {
            if (Instance == this)
            {
                ObjectCounter.OnSceneObjectivesCompleted -= HandleSceneObjectivesCompleted; // Event deabonnieren
                Debug.Log("[GameController] Instance destroyed and unsubscribed from OnSceneObjectivesCompleted.");
            }
        }

        private void HandleSceneObjectivesCompleted()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"[GameController] HandleSceneObjectivesCompleted: Ziele in Szene '{currentSceneName}' erreicht.");

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
                Debug.Log($"[GameController] Ziele für '{currentSceneName}' erreicht. Bereite Übergang zu '{nextSceneToLoad}' vor.");
                if (ResourceStorageManager.Instance != null)
                {
                    ResourceStorageManager.Instance.LastScene = nextSceneToLoad;
                    Debug.Log($"[GameController] ResourceStorageManager.Instance.LastScene auf '{nextSceneToLoad}' gesetzt.");
                }
                else
                {
                    Debug.LogError("[GameController] ResourceStorageManager.Instance ist null. Kann LastScene nicht für Speicherung aktualisieren.");
                }

                // Speichere den Spielstand, NACHDEM LastScene im RSM aktualisiert wurde.
                if (SaveGame.Instance != null) 
                {
                    SaveGame.SaveGameData(); 
                    Debug.Log($"[GameController] Spielstand gespeichert. LastScene in Speicherdatei sollte jetzt '{nextSceneToLoad}' sein.");
                }
                else
                {
                    Debug.LogError("[GameController] SaveGame.Instance ist null. Spielstand konnte nicht vor Szenenwechsel gespeichert werden.");
                }
                
                Debug.Log($"[GameController] Lade Szene '{nextSceneToLoad}'.");
                SceneManager.LoadScene(nextSceneToLoad);
            }
            else
            {
                Debug.LogWarning($"[GameController] Ziele in Szene '{currentSceneName}' erreicht, aber keine Logik für Szenenwechsel definiert.");
            }
        }

        public void LoadNextLevel() // Diese Methode wird jetzt nicht mehr direkt von HandleSceneObjectivesCompleted benötigt
        {
            string currentScene = SceneManager.GetActiveScene().name;
            string nextScene = "";

            if (currentScene == "Tutorial")
            {
                nextScene = "Kapitel1"; // Stelle sicher, dass diese Szene existiert
            }
            else if (currentScene == "Kapitel1")
            {
                nextScene = "Kapitel2"; // Stelle sicher, dass diese Szene existiert
            }
            else if (currentScene == "Kapitel2")
            {
                nextScene = "Endscreen"; // Geändert von "MainMenu" zu "Endscreen"
            }

            if (!string.IsNullOrEmpty(nextScene))
            {
                Debug.Log($"[GameController] LoadNextLevel: Lade Szene '{nextScene}'.");
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                Debug.LogError($"[GameController] LoadNextLevel: Keine nächste Szene für '{currentScene}' definiert.");
            }
        }

        public void LoadEndScreen()
        {
            // Stelle sicher, dass die Szene "Endscreen" zu den Build Settings hinzugefügt wurde
            SceneManager.LoadScene("Endscreen"); 
        }

        // Du könntest hier weitere Methoden für das Spielmanagement hinzufügen,
        // z.B. Spiel starten, Spiel pausieren, Level laden etc.
    }
}