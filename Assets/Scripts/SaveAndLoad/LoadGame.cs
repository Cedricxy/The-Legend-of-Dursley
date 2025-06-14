/* using UnityEngine;
using System.IO;

namespace SaveAndLoad
{
    [DefaultExecutionOrder(-1)]
    public class LoadGame : MonoBehaviour
    {
        public static LoadGame Instance { get; private set; }
        private SaveDataObject loadedData;

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
            Debug.Log("LoadGameData");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadGameData();
                UpdateGameData();
                Debug.Log("LoadGameData");
            }
        }

        private class SaveDataObject
        {
            public bool IsInitialGameStart = true;
            public int MaxStorageValue = 1000000;
            public int GoldStorageValue = 250000;
            public int ElixirStorageValue = 250000;
        }

        public void LoadGameData()
        {
            string filePath = Application.persistentDataPath + "/SaveGameData.json";
            Debug.Log($"<color=orange>[LoadGame]</color> Versuch, Spieldaten zu laden von: {filePath}");
            if (File.Exists(filePath))
            {
                string jsonLoad = File.ReadAllText(filePath);
                loadedData = JsonUtility.FromJson<SaveDataObject>(jsonLoad);
                Debug.Log("<color=orange>[LoadGame]</color> Spieldaten erfolgreich geladen.");
                Debug.Log($"<color=orange>[LoadGame]</color> Geladene Werte: IsInitialGameStart={loadedData.IsInitialGameStart}, LastScene='{loadedData.LastScene}', Hearts={loadedData.HeartValue}, Stars={loadedData.StarValue}");
            }
            else
            {
                Debug.LogWarning("<color=orange>[LoadGame]</color> Keine Speicherdatei gefunden. Erstelle neues SaveDataObject mit Standardwerten.");
                loadedData = new SaveDataObject(); // Erstellt ein Objekt mit Standardwerten (IsInitialGameStart=true, HeartValue=1, LastScene="MainMenu")
                Debug.Log($"<color=orange>[LoadGame]</color> Standardwerte verwendet: IsInitialGameStart={loadedData.IsInitialGameStart}, LastScene='{loadedData.LastScene}', Hearts={loadedData.HeartValue}, Stars={loadedData.StarValue}");
            }
        }

        public bool IsInitialGameStart => loadedData.IsInitialGameStart;

        public int MaxStorageValue => loadedData.MaxStorageValue;

        public int GoldStorageValue => loadedData.GoldStorageValue;

        public int ElixirStorageValue => loadedData.ElixirStorageValue;

        private void UpdateGameData()
        {
            Debug.Log("UpdateGameData");
            IsGameLoaded = true;
        }
    }
} */

using UnityEngine;
using System.IO;

namespace SaveAndLoad
{
    [DefaultExecutionOrder(-1)]
    public class LoadGame : MonoBehaviour
    {
        public static LoadGame Instance { get; private set; }
        private SaveDataObject loadedData;

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
            Debug.Log("LoadGameData");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadGameData();
                UpdateGameData();
                Debug.Log("LoadGameData");
            }
        }

        // Neue SaveDataObject-Struktur passend zu SaveGame und ResourceStorageManager
        private class SaveDataObject
        {
            public bool IsInitialGameStart = true;
            public int HeartValue = 1;
            public int StarValue = 0;
            public string LastScene = "MainMenu";
        }

        public void LoadGameData()
        {
            string filePath = Application.persistentDataPath + "/SaveGameData.json";
            Debug.Log($"<color=orange>[LoadGame]</color> Versuch, Spieldaten zu laden von: {filePath}");
            if (File.Exists(filePath))
            {
                string jsonLoad = File.ReadAllText(filePath);
                loadedData = JsonUtility.FromJson<SaveDataObject>(jsonLoad);
                Debug.Log("<color=orange>[LoadGame]</color> Spieldaten erfolgreich geladen.");
                Debug.Log($"<color=orange>[LoadGame]</color> Geladene Werte: IsInitialGameStart={loadedData.IsInitialGameStart}, LastScene='{loadedData.LastScene}', Hearts={loadedData.HeartValue}, Stars={loadedData.StarValue}");
            }
            else
            {
                Debug.LogWarning("<color=orange>[LoadGame]</color> Keine Speicherdatei gefunden. Erstelle neues SaveDataObject mit Standardwerten.");
                loadedData = new SaveDataObject(); // Erstellt ein Objekt mit Standardwerten (IsInitialGameStart=true, HeartValue=1, LastScene="MainMenu")
                Debug.Log($"<color=orange>[LoadGame]</color> Standardwerte verwendet: IsInitialGameStart={loadedData.IsInitialGameStart}, LastScene='{loadedData.LastScene}', Hearts={loadedData.HeartValue}, Stars={loadedData.StarValue}");
            }
        }

        public bool IsInitialGameStart => loadedData.IsInitialGameStart;
        public int HeartStorageValue => loadedData.HeartValue;
        public int StarStorageValue => loadedData.StarValue;
        public string LastScene => loadedData.LastScene;

        private void UpdateGameData()
        {
            Debug.Log("UpdateGameData");
            IsGameLoaded = true;
        }
    }
}