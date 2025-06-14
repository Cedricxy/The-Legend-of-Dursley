using UnityEngine;
using UnityEngine.SceneManagement; // Hinzugefügt für SceneManager


namespace GameManagement
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Behält den GameController über Szenenwechsel hinweg
            }
            else
            {
                Destroy(gameObject);
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