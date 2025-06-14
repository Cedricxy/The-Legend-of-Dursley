// Diese C#-Datei ist für das Speichern von den Spieldaten in Unity zuständig.
using UnityEngine;
using System.IO;
using ResourceSystem.Storage;
using UnityEngine.SceneManagement;

namespace SaveAndLoad
{
    public class SaveGame : MonoBehaviour
    {
        [SerializeField] private bool autosave;

        // Singleton der SaveGame-Klasse
        public static SaveGame Instance { get; private set; }

        private void Awake()
        {
            // Stellt sicher, dass nur eine Instanz der SaveGame-Klasse existiert.
            if (Instance == null)
            {
                Instance = this;
                transform.parent = null; // Entfernt das Objekt aus der Hierarchie in Unity.
                DontDestroyOnLoad(gameObject); // Verhindert, dass das Objekt beim Szenenwechsel zerstört wird.
            }
            else
            {
                Destroy(gameObject); // Löscht zusätzliche Instanzen.
            }
        }

        private void Start()
        {
            // Aktiviert das automatische Speichern, wenn autosave aktiviert ist.
            if (autosave)
            {
                InvokeRepeating(nameof(AutoSave), 5f, 5f); // Ruft AutoSave alle 5 Sekunden auf.
            }
        }

        private void AutoSave()
        {
            // Speichert die Spieldaten automatisch.
            SaveGameData(); // Ruft ohne Parameter auf, versucht RSM zu verwenden
        }

        private void Update()
        {
            // Speichert die Spieldaten, wenn die Taste F5 gedrückt wird.
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGameData(); // Ruft ohne Parameter auf, versucht RSM zu verwenden
            }
        }

        // Klasse, die die zu speichernden Spieldaten repräsentiert.
        public class SaveDataObject
        {
            public bool IsInitialGameStart = true; // Spielstartzustand
            public string LastScene = "MainMenu"; // Standard-Szene
            public int HeartValue = 1; // Standardwert für Herzen
            public int StarValue = 0; // Standardwert für Sterne
        }

        // Speichert die Spieldaten in einer JSON-Datei.
        public static void SaveGameData(SaveDataObject dataToSave = null)
        {
            SaveDataObject finalData;

            if (dataToSave != null)
            {
                finalData = dataToSave;
                Debug.Log("<color=cyan>[SaveGame]</color> Speichere explizit übergebenes SaveDataObject.");
            }
            else if (ResourceStorageManager.Instance != null)
            {
                finalData = new SaveDataObject()
                {
                    IsInitialGameStart = ResourceStorageManager.Instance.IsInitialGameStart,
                    HeartValue = ResourceStorageManager.Instance.HeartStorageValue,
                    StarValue = ResourceStorageManager.Instance.StarStorageValue,
                    LastScene = ResourceStorageManager.Instance.LastScene // GEÄNDERT: Nutze LastScene vom RSM
                };
                Debug.Log("<color=cyan>[SaveGame]</color> Speichere Daten von ResourceStorageManager.Instance. LastScene aus RSM: " + finalData.LastScene);
            }
            else
            {
                Debug.LogError("<color=cyan>[SaveGame]</color> Weder SaveDataObject übergeben noch ResourceStorageManager.Instance verfügbar. Spiel kann nicht gespeichert werden.");
                return;
            }
            
            string jsonSave = JsonUtility.ToJson(finalData, true);
            File.WriteAllText(Application.persistentDataPath + "/SaveGameData.json", jsonSave);
            
            Debug.Log($"<color=cyan>[SaveGame]</color> Spiel gespeichert! Pfad: {Application.persistentDataPath}/SaveGameData.json");
            Debug.Log($"<color=cyan>[SaveGame]</color> Gespeicherte Werte: IsInitialGameStart={finalData.IsInitialGameStart}, LastScene='{finalData.LastScene}', Hearts={finalData.HeartValue}, Stars={finalData.StarValue}");
        }
    }
}