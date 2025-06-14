using UnityEngine;
using System.IO;

namespace SaveAndLoad
{
    [DefaultExecutionOrder(-1)]
    public class LoadGame : MonoBehaviour
    {
        public static LoadGame Instance { get; private set; }
        private SaveGame.SaveDataObject loadedData;

        public static bool IsGameLoaded { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadGameData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadGameData();
                UpdateGameData();
            }
        }

        // Lädt Spieldaten aus einer JSON-Datei oder erstellt neue Standarddaten.
        public void LoadGameData()
        {
            string filePath = Application.persistentDataPath + "/SaveGameData.json";
            if (File.Exists(filePath))
            {
                string jsonLoad = File.ReadAllText(filePath);
                loadedData = JsonUtility.FromJson<SaveGame.SaveDataObject>(jsonLoad);
                if (loadedData == null)
                {
                    loadedData = new SaveGame.SaveDataObject();
                }
            }
            else
            {
                // Erstellt ein neues SaveDataObject mit Standardwerten.
                loadedData = new SaveGame.SaveDataObject();
            }
        }

        // Gibt die geladenen Spieldaten zurück.
        public bool IsInitialGameStart => loadedData != null ? loadedData.IsInitialGameStart : true;
        public int HeartStorageValue => loadedData != null ? loadedData.HeartValue : 1;
        public int StarStorageValue => loadedData != null ? loadedData.StarValue : 0;
        public string LastScene => loadedData != null ? loadedData.LastScene : "Tutorial";

        // Setzt den Status, dass die Spieldaten geladen werden können.
        private void UpdateGameData()
        {
            IsGameLoaded = true;
        }
    }
}