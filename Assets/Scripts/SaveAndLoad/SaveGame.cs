// Diese C#-Datei ist für das Speichern von den Spieldaten in Unity zuständig.
using UnityEngine;
using System.IO;
using ResourceSystem.Storage;
using UnityEngine.SceneManagement;

namespace SaveAndLoad
{
    public class SaveGame : MonoBehaviour
    {
        // Gibt an, ob das Spiel automatisch gespeichert werden soll.
        [SerializeField] private bool autosave;

        // Boolean, der festlegt, ob ein neues Spiel startet und ggf. vorhandene Werte überschrieben werden.
        [SerializeField] private bool isSetToInitialGameStart;

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
            SaveGameData();
        }

        private void Update()
        {
            // Speichert die Spieldaten, wenn die Taste F5 gedrückt wird.
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGameData();
            }
        }

        // Zugriff auf den initialen Spielstartzustand.
        public static bool IsSetToInitialGameStart
        {
            get => Instance.isSetToInitialGameStart;
            set => Instance.isSetToInitialGameStart = value;
        }

        // Überprüft, ob das Spiel im Startzustand ist.
        private static bool CheckIsInitialGameStart()
        {
            bool isInitialGameStart = true;

            if (ResourceStorageManager.Instance != null)
            {
                isInitialGameStart = ResourceStorageManager.Instance.IsInitialGameStart;
            }

            if (IsSetToInitialGameStart)
            {
                isInitialGameStart = true;
            }

            return isInitialGameStart;
        }

        // Die zuletzt gespeicherte Szene.
        private static string CheckLastScene()
        {
            return SceneManager.GetActiveScene().name;
        }
        
        // Überprüft den Wert der Herzen.
        private static int CheckHeartStorageValue()
        {
            int heartStorageValue = 0;

            if (ResourceStorageManager.Instance != null)
            {
                heartStorageValue = 	ResourceStorageManager.Instance.HeartStorageValue;
            }

            return heartStorageValue;
        }

        // Überprüft den Wert der Sterne.
        private static int CheckStarStorageValue()
        {
            int starStorageValue = 0;

            if (ResourceStorageManager.Instance != null)
            {
                starStorageValue = 	ResourceStorageManager.Instance.StarStorageValue;
            }

            return starStorageValue;
        }

        // Klasse, die die zu speichernden Spieldaten repräsentiert.
        private class SaveDataObject
        {
            public bool IsInitialGameStart = true; // Spielstartzustand
            public string LastScene = "MainMenu"; // Standard-Szene
            public int HeartValue = 1; // Standardwert für Herzen
            public int StarValue = 0; // Standardwert für Sterne
        }

        // Speichert die Spieldaten in einer JSON-Datei.
        public static void SaveGameData()
        {
            SaveDataObject saveDataObject = new SaveDataObject()
            {
                IsInitialGameStart = CheckIsInitialGameStart(),
                LastScene = CheckLastScene(),
                HeartValue = CheckHeartStorageValue(),
                StarValue = CheckStarStorageValue()
            };

            string jsonSave = JsonUtility.ToJson(saveDataObject); // Konvertiert die Daten in das JSON-Format.
            File.WriteAllText(Application.persistentDataPath + "/SaveGameData.json", jsonSave); // Speichert die JSON-Datei.
        }
    }
}