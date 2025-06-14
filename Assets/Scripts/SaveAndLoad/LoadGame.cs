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
                loadedData = JsonUtility.FromJson<SaveGame.SaveDataObject>(jsonLoad);
                Debug.Log("<color=orange>[LoadGame]</color> Spieldaten erfolgreich geladen.");
                if (loadedData == null) // Zusätzliche Prüfung, falls FromJson fehlschlägt und null zurückgibt
                {
                    Debug.LogError("<color=orange>[LoadGame]</color> JsonUtility.FromJson hat null zurückgegeben. Erstelle Standarddaten.");
                    loadedData = new SaveGame.SaveDataObject(); // Fallback
                }
                else
                {
                    Debug.Log($"<color=orange>[LoadGame]</color> Geladene Werte: IsInitialGameStart={loadedData.IsInitialGameStart}, LastScene='{loadedData.LastScene}', Hearts={loadedData.HeartValue}, Stars={loadedData.StarValue}");
                }
            }
            else
            {
                Debug.LogWarning("<color=orange>[LoadGame]</color> Keine Speicherdatei gefunden. Erstelle neues SaveGame.SaveDataObject mit Standardwerten.");
                loadedData = new SaveGame.SaveDataObject(); // NEU: Erstelle eine Instanz von SaveGame.SaveDataObject
                Debug.Log($"<color=orange>[LoadGame]</color> Standardwerte verwendet: IsInitialGameStart={loadedData.IsInitialGameStart}, LastScene='{loadedData.LastScene}', Hearts={loadedData.HeartValue}, Stars={loadedData.StarValue}");
            }
        }

        public bool IsInitialGameStart => loadedData != null ? loadedData.IsInitialGameStart : true; // Null-Check hinzugefügt
        public int HeartStorageValue => loadedData != null ? loadedData.HeartValue : 1; // Null-Check hinzugefügt
        public int StarStorageValue => loadedData != null ? loadedData.StarValue : 0; // Null-Check hinzugefügt
        public string LastScene => loadedData != null ? loadedData.LastScene : "Tutorial"; // Null-Check und sinnvoller Fallback

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
        // private SaveDataObject loadedData; // Entferne alte private Klassendefinition
        private SaveGame.SaveDataObject loadedData; // NEU: Verwende die öffentliche Klasse von SaveGame

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

        public void LoadGameData()
        {
            string filePath = Application.persistentDataPath + "/SaveGameData.json";
            Debug.Log($"<color=orange>[LoadGame]</color> Versuch, Spieldaten zu laden von: {filePath}");
            if (File.Exists(filePath))
            {
                string jsonLoad = File.ReadAllText(filePath);
                // NEU: Deserialisiere in SaveGame.SaveDataObject
                loadedData = JsonUtility.FromJson<SaveGame.SaveDataObject>(jsonLoad); 
                Debug.Log("<color=orange>[LoadGame]</color> Spieldaten erfolgreich geladen.");
                if (loadedData == null) // Zusätzliche Prüfung, falls FromJson fehlschlägt und null zurückgibt
                {
                    Debug.LogError("<color=orange>[LoadGame]</color> JsonUtility.FromJson hat null zurückgegeben. Erstelle Standarddaten.");
                    loadedData = new SaveGame.SaveDataObject(); // Fallback
                }
                else
                {
                    Debug.Log($"<color=orange>[LoadGame]</color> Geladene Werte: IsInitialGameStart={loadedData.IsInitialGameStart}, LastScene='{loadedData.LastScene}', Hearts={loadedData.HeartValue}, Stars={loadedData.StarValue}");
                }
            }
            else
            {
                Debug.LogWarning("<color=orange>[LoadGame]</color> Keine Speicherdatei gefunden. Erstelle neues SaveGame.SaveDataObject mit Standardwerten.");
                // NEU: Erstelle eine Instanz von SaveGame.SaveDataObject
                loadedData = new SaveGame.SaveDataObject(); 
                Debug.Log($"<color=orange>[LoadGame]</color> Standardwerte verwendet: IsInitialGameStart={loadedData.IsInitialGameStart}, LastScene='{loadedData.LastScene}', Hearts={loadedData.HeartValue}, Stars={loadedData.StarValue}");
            }
        }

        public bool IsInitialGameStart => loadedData != null ? loadedData.IsInitialGameStart : true; // Null-Check hinzugefügt
        public int HeartStorageValue => loadedData != null ? loadedData.HeartValue : 1; // Null-Check hinzugefügt
        public int StarStorageValue => loadedData != null ? loadedData.StarValue : 0; // Null-Check hinzugefügt
        public string LastScene => loadedData != null ? loadedData.LastScene : "Tutorial"; // Null-Check und sinnvoller Fallback

        private void UpdateGameData()
        {
            Debug.Log("UpdateGameData");
            IsGameLoaded = true;
        }
    }
}