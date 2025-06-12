/* using Gui;
using UnityEngine;
using SaveAndLoad;

namespace ResourceSystem.Storage
{
    [DefaultExecutionOrder(0)]
    public class ResourceStorageManager : MonoBehaviour
    {
        public static ResourceStorageManager Instance { get; private set; }

        [SerializeField] private bool isInitialGameStart;
        [SerializeField] private int maxStorageValue;
        [SerializeField] private int goldStorageValue;
        [SerializeField] private int elixirStorageValue;

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

        public void Start()
        {
            Debug.Log("ResourceStorageManager Start");
            LoadGameStorage();
            CheckInitialGameStart();
        }

        public void Update()
        {
            CheckLoadGame();
            CheckInitialGameStart();
        }

        private void OnValidate()
        {
            goldStorageValue = Mathf.Clamp(goldStorageValue, 0, maxStorageValue);
            elixirStorageValue = Mathf.Clamp(elixirStorageValue, 0, maxStorageValue);
        }

        private void LoadGameStorage()
        {
            if (LoadGame.Instance != null)
            {
                isInitialGameStart = LoadGame.Instance.IsInitialGameStart;
                maxStorageValue = LoadGame.Instance.MaxStorageValue;
                goldStorageValue = LoadGame.Instance.GoldStorageValue;
                elixirStorageValue = LoadGame.Instance.ElixirStorageValue;
            }
        }

        public bool IsInitialGameStart
        {
            get => isInitialGameStart;
            set => isInitialGameStart = value;
        }

        public int MaxStorageValue
        {
            get => maxStorageValue;
            set => maxStorageValue = value;
        }

        public int GoldStorageValue
        {
            get => goldStorageValue;
            set => goldStorageValue = value;
        }

        public int ElixirStorageValue
        {
            get => elixirStorageValue;
            set => elixirStorageValue = value;
        }

        private void CheckLoadGame()
        {
            if (LoadGame.IsGameLoaded)
            {
                LoadGameStorage();
                LoadGame.IsGameLoaded = false;
            } else if (VillageGuiManager.ProfileLoadGame)
            {
                LoadGameStorage();
                VillageGuiManager.ProfileLoadGame = false;
            }
        }

        private void CheckInitialGameStart()
        {
            if (isInitialGameStart)
            {
                maxStorageValue = 1000000;
                goldStorageValue = Mathf.Clamp(250000, 0, maxStorageValue);
                elixirStorageValue = Mathf.Clamp(250000, 0, maxStorageValue);
                isInitialGameStart = false;
                Debug.Log("Initial Game Start");
            }
        }
    }
} */

using Gui;
using UnityEngine;
using SaveAndLoad;
using UnityEngine.SceneManagement;

namespace ResourceSystem.Storage
{
    [DefaultExecutionOrder(0)]
    public class ResourceStorageManager : MonoBehaviour
    {
        public static ResourceStorageManager Instance { get; private set; }

        [SerializeField] private bool isInitialGameStart;
        [SerializeField] private int heartStorageValue = 1;
        [SerializeField] private int starStorageValue = 0;
        [SerializeField] private string lastScene = "Tutorial";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
                Debug.Log($"ResourceStorageManager Awake: Instanz gesetzt, ID: {Instance.GetInstanceID()}");
            }
            else
            {
                Debug.LogWarning($"Zerstöre zusätzliche ResourceStorageManager-Instanz, ID: {gameObject.GetInstanceID()}. Bestehende Instanz-ID: {Instance.GetInstanceID()}");
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            LoadGameStorage(); // Lädt Werte, inklusive isInitialGameStart und lastScene aus der Datei
            CheckInitialGameStart(); // Setzt Standardwerte, falls nötig, und aktualisiert isInitialGameStart und lastScene
            LoadLastScene(); // Lädt die Szene basierend auf dem (potenziell aktualisierten) lastScene Wert
        }

        public void Update()
        {
            //CheckLoadGame();
            //CheckInitialGameStart();
        }

        private void LoadGameStorage()
        {
            if (LoadGame.Instance != null)
            {
                isInitialGameStart = LoadGame.Instance.IsInitialGameStart;
                heartStorageValue = LoadGame.Instance.HeartStorageValue;
                starStorageValue = LoadGame.Instance.StarStorageValue;
                lastScene = LoadGame.Instance.LastScene;
            }
        }
        
        private void LoadLastScene()
        {
            // Überprüfe, ob eine Szene geladen werden soll und es nicht bereits die aktive Szene ist.
            if (!string.IsNullOrEmpty(lastScene) && SceneManager.GetActiveScene().name != lastScene)
            {
                Debug.Log($"ResourceStorageManager.LoadLastScene: Lade Szene '{lastScene}'.");
                SceneManager.LoadScene(lastScene);
            }
            else
            {
                Debug.Log($"ResourceStorageManager.LoadLastScene: Szene '{lastScene}' ist bereits aktiv oder nicht spezifiziert. Kein Szenenwechsel.");
            }
        }

        public bool IsInitialGameStart
        {
            get => isInitialGameStart;
            set => isInitialGameStart = value;
        }

        public int HeartStorageValue
        {
            get => heartStorageValue;
            set => heartStorageValue = value;
        }

        public int StarStorageValue
        {
            get => starStorageValue;
            set => starStorageValue = value;
        }

        public string LastScene
        {
            get => lastScene;
            set => lastScene = value;
        }

        private void CheckLoadGame()
        {
            if (LoadGame.IsGameLoaded)
            {
                LoadGameStorage();
                LoadGame.IsGameLoaded = false;
            }
        }

        private void CheckInitialGameStart()
        {
            // Diese Methode wird nach LoadGameStorage aufgerufen.
            // Wenn isInitialGameStart (entweder aus der Datei geladen oder der Standardwert) true ist,
            // werden die Werte für einen neuen Spielstand gesetzt.
            if (this.isInitialGameStart) 
            {
                Debug.Log("ResourceStorageManager.CheckInitialGameStart: Setze Spiel auf Standardwerte.");
                heartStorageValue = 1;
                starStorageValue = 0;
                lastScene = "MainMenu"; // Definiere hier deine absolute Start-Szene für einen neuen Spielstand
                // Wichtig: Nachdem die initialen Werte gesetzt wurden, ist es kein "initialer Spielstart" mehr für den nächsten Speicherzyklus.
                this.isInitialGameStart = false; 
            }
        }
    }
}