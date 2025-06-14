// Diese C#-Datei ist für das Speichern von den Spieldaten in Unity zuständig.
using UnityEngine;
using System.IO;
using ResourceSystem.Storage;

namespace SaveAndLoad
{
    public class SaveGame : MonoBehaviour
    {
        [SerializeField] private bool autosave;

        // Singleton der SaveGame-Klasse.
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
            SaveGameData(); // Ruft ohne Parameter auf, versucht RSM zu verwenden.
        }

        private void Update()
        {
            // Speichert die Spieldaten, wenn die Taste F5 gedrückt wird.
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGameData(); // Ruft ohne Parameter auf, versucht RSM zu verwenden.
            }
        }

        // Klasse, die die zu speichernden Spieldaten repräsentiert.
        public class SaveDataObject
        {
            public bool IsInitialGameStart = true; // Spielstartzustand
            public string LastScene = "MainMenu"; // Standard-Szene
            public int HeartValue = 1; // Standardwert für Herzen
            public int StarValue; // Standardwert für Sterne
        }

        // Speichert die Spieldaten in einer JSON-Datei.
        public static void SaveGameData(SaveDataObject dataToSave = null)
        {
            SaveDataObject finalData;

            if (dataToSave != null)
            {
                finalData = dataToSave;
            }
            else if (ResourceStorageManager.Instance != null)
            {
                finalData = new SaveDataObject()
                {
                    IsInitialGameStart = ResourceStorageManager.Instance.IsInitialGameStart,
                    HeartValue = ResourceStorageManager.Instance.HeartStorageValue,
                    StarValue = ResourceStorageManager.Instance.StarStorageValue,
                    LastScene = ResourceStorageManager.Instance.LastScene
                };
            }
            else
            {
                return;
            }
            
            string jsonSave = JsonUtility.ToJson(finalData, true);
            File.WriteAllText(Application.persistentDataPath + "/SaveGameData.json", jsonSave);
        }
    }
}