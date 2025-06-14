using UnityEngine;
using UnityEngine.SceneManagement;
using SaveAndLoad;
using GameManagement;

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
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoadedActions;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoadedActions;
        }

        public void Start()
        {
            if (LoadGame.Instance != null)
            {
                 OnSceneLoadedActions(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            }
        }

        // Wird nach dem Laden jeder Szene aufgerufen.
        void OnSceneLoadedActions(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MainMenu" || scene.name == "Endscreen")
            {
                if (LoadGame.Instance != null)
                {
                    LoadGame.Instance.LoadGameData();
                    LoadGameStorage();
                }
                return;
            }

            // Führt einen vollständigen Reset durch.
            if (GameController.ForceNewGameReset && scene.name == "Tutorial")
            {
                this.isInitialGameStart = false;
                this.heartStorageValue = 1;
                this.starStorageValue = 0;
                this.lastScene = "Tutorial";

                SaveGame.SaveDataObject newGameSaveData = new SaveGame.SaveDataObject
                {
                    IsInitialGameStart = this.isInitialGameStart,
                    HeartValue = this.heartStorageValue,
                    StarValue = this.starStorageValue,
                    LastScene = this.lastScene
                };
                SaveGame.SaveGameData(newGameSaveData);
                GameController.ForceNewGameReset = false;
                return;
            }

            if (LoadGame.Instance != null)
            {
                LoadGame.Instance.LoadGameData();
            }
            else
            {
                return;
            }

            LoadGameStorage();
            CheckInitialGameStart();

            // Lädt die letzte Szene, falls sie nicht bereits aktiv ist.
            if (SceneManager.GetActiveScene().name != this.lastScene)
            {
                LoadLastScene();
            }
        }

        // Lädt die gespeicherten Werte aus LoadGame in den ResourceStorageManagers.
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
        
        // Lädt die in 'lastScene' gespeicherte Szene.
        private void LoadLastScene()
        {
            if (!string.IsNullOrEmpty(lastScene) && SceneManager.GetActiveScene().name != lastScene)
            {
                SceneManager.LoadScene(lastScene);
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

        // Überprüft, ob ein initialer Spielstart vorliegt oder die Lebenspunkte aufgebraucht sind.
        private void CheckInitialGameStart()
        {
            bool needsResetOrInitialSetup = this.isInitialGameStart || this.heartStorageValue <= 0;

            if (needsResetOrInitialSetup)
            {
                SaveGame.SaveDataObject dataToSave = new SaveGame.SaveDataObject();
                dataToSave.IsInitialGameStart = false;
                dataToSave.HeartValue = 1;
                // StarValue wird auf 0 gesetzt, wenn es ein initialer Start war oder die Herzen ≤ 0 waren.
                dataToSave.StarValue = (this.isInitialGameStart || this.heartStorageValue <= 0) ? 0 : this.starStorageValue;
                dataToSave.LastScene = "Tutorial";

                SaveGame.SaveGameData(dataToSave);

                this.isInitialGameStart = dataToSave.IsInitialGameStart;
                this.heartStorageValue = dataToSave.HeartValue;
                this.starStorageValue = dataToSave.StarValue;
                this.lastScene = dataToSave.LastScene;
            }
        }
    }
}