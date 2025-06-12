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
            if (File.Exists(filePath))
            {
                string jsonLoad = File.ReadAllText(filePath);
                loadedData = JsonUtility.FromJson<SaveDataObject>(jsonLoad);
                Debug.Log("loadedData: " + loadedData);
            }
            else
            {
                loadedData = new SaveDataObject();
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
            if (File.Exists(filePath))
            {
                string jsonLoad = File.ReadAllText(filePath);
                loadedData = JsonUtility.FromJson<SaveDataObject>(jsonLoad);
                Debug.Log("loadedData: " + loadedData);
            }
            else
            {
                loadedData = new SaveDataObject();
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