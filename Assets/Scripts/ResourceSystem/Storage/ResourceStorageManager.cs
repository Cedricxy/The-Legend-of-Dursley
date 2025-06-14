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
using UnityEngine.SceneManagement;
using SaveAndLoad; 

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
                Debug.Log($"<color=green>[RSM]</color> Awake: Instanz gesetzt, ID: {Instance.GetInstanceID()}");
            }
            else
            {
                Debug.LogWarning($"<color=green>[RSM]</color> Zerstöre zusätzliche ResourceStorageManager-Instanz, ID: {gameObject.GetInstanceID()}. Bestehende Instanz-ID: {Instance.GetInstanceID()}");
                Destroy(gameObject);
                return; // Wichtig, um weitere Initialisierung dieser doppelten Instanz zu verhindern
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoadedActions;
            Debug.Log("<color=green>[RSM]</color> OnEnable: sceneLoaded abonniert.");
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoadedActions;
            Debug.Log("<color=green>[RSM]</color> OnDisable: sceneLoaded deabonniert.");
        }

        public void Start()
        {
            // Diese Start-Methode wird nur einmal beim allerersten Erstellen des RSM-Objekts aufgerufen.
            // Die Hauptlogik für Szenen-Initialisierung passiert jetzt in OnSceneLoadedActions.
            Debug.Log($"<color=green>[RSM]</color> Initialer Start (einmalig pro RSM-Lebenszeit). Aktuelle Szene beim Erstellen: '{SceneManager.GetActiveScene().name}'");
            // Für den allerersten Start (z.B. direktes Starten in einer Test-Szene ohne MainMenu), 
            // rufen wir die Logik manuell auf.
            // Bei normalen Spielstarts über das Menü wird OnSceneLoadedActions durch den Szenenwechsel getriggert.
            if (LoadGame.Instance != null) // Sicherstellen, dass LoadGame bereit ist
            {
                 // Für den allerersten Start des Spiels (noch keine Szene explizit geladen)
                 // rufen wir die Logik hier auf, um den Zustand zu initialisieren.
                 OnSceneLoadedActions(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("<color=green>[RSM]</color> LoadGame.Instance ist null im RSM.Start(). Dies könnte zu Problemen führen, wenn das Spiel nicht über das MainMenu gestartet wird.");
            }
        }

        // Die Update-Methode wird nicht mehr für die Kernlogik des Ladens/Resets benötigt.
        // public void Update()
        // {
        // }

        void OnSceneLoadedActions(Scene scene, LoadSceneMode mode)
        {
            // NEU: Wenn die MainMenu- oder Endscreen-Szene geladen wird, soll der RSM keine weiteren Aktionen ausführen,
            // die einen Szenenwechsel oder eine Zustandsänderung erzwingen.
            if (scene.name == "MainMenu" || scene.name == "Endscreen")
            {
                Debug.Log($"<color=green>[RSM]</color> OnSceneLoadedActions: Spezielle Szene ('{scene.name}') geladen. Es werden keine automatischen Spielzustandsänderungen oder Szenenwechsel von hier aus durchgeführt.");
                // Optional: Lade die Daten, damit die Szene sie ggf. anzeigen kann, aber handle sie nicht weiter.
                if (LoadGame.Instance != null)
                {
                    LoadGame.Instance.LoadGameData();
                    LoadGameStorage(); // Aktualisiert die internen Felder des RSM
                     Debug.Log($"<color=green>[RSM]</color> OnSceneLoadedActions ('{scene.name}'): Daten in RSM geladen: isInitial={isInitialGameStart}, Hearts={heartStorageValue}, RSM.lastScene='{this.lastScene}'");
                }
                return; // WICHTIG: Verhindert weitere Verarbeitung für das MainMenu und den Endscreen
            }

            Debug.Log($"<color=green>[RSM]</color> OnSceneLoadedActions für Szene '{scene.name}'. RSM-Werte VOR Aktionen: isInitial={isInitialGameStart}, Hearts={heartStorageValue}, RSM.lastScene='{this.lastScene}'");

            if (LoadGame.Instance != null)
            {
                LoadGame.Instance.LoadGameData(); 
                Debug.Log($"<color=green>[RSM]</color> OnSceneLoadedActions: LoadGame.Instance.LoadGameData() aufgerufen. LoadGame hat jetzt: LastScene='{LoadGame.Instance.LastScene}', Hearts={LoadGame.Instance.HeartStorageValue}, IsInitial={LoadGame.Instance.IsInitialGameStart}");
            }
            else
            {
                Debug.LogError("<color=green>[RSM]</color> OnSceneLoadedActions: LoadGame.Instance ist null! Kann Daten nicht von Festplatte aktualisieren.");
                return; 
            }

            LoadGameStorage(); 
            Debug.Log($"<color=green>[RSM]</color> OnSceneLoadedActions: Nach LoadGameStorage(): RSM.isInitial={isInitialGameStart}, RSM.Hearts={heartStorageValue}, RSM.lastScene='{this.lastScene}'");

            CheckInitialGameStart(); 
            Debug.Log($"<color=green>[RSM]</color> OnSceneLoadedActions: Nach CheckInitialGameStart(): RSM.isInitial={isInitialGameStart}, RSM.Hearts={heartStorageValue}, RSM.lastScene='{this.lastScene}'");

            // WICHTIG: LoadLastScene nur ausführen, wenn wir NICHT bereits in der Zielszene sind,
            // die CheckInitialGameStart möglicherweise gerade festgelegt hat (z.B. Tutorial).
            if (SceneManager.GetActiveScene().name != this.lastScene)
            {
                LoadLastScene();
            }
            else
            {
                Debug.Log($"<color=green>[RSM]</color> OnSceneLoadedActions: Aktive Szene ('{SceneManager.GetActiveScene().name}') ist bereits die Ziel-Szene ('{this.lastScene}'). Kein erneutes Laden durch LoadLastScene nötig.");
            }
            Debug.Log($"<color=green>[RSM]</color> OnSceneLoadedActions für Szene '{scene.name}' abgeschlossen.");
        }

        private void LoadGameStorage()
        {
            if (LoadGame.Instance != null)
            {
                // Wichtig: Zuerst laden, was in der Datei steht.
                isInitialGameStart = LoadGame.Instance.IsInitialGameStart;
                heartStorageValue = LoadGame.Instance.HeartStorageValue;
                starStorageValue = LoadGame.Instance.StarStorageValue;
                lastScene = LoadGame.Instance.LastScene;
                Debug.Log($"<color=green>[RSM]</color> LoadGameStorage: Geladen aus Datei - isInitial={isInitialGameStart}, Hearts={heartStorageValue}, Scene='{lastScene}'");
            }
            else
            {
                Debug.LogError("<color=green>[RSM]</color> LoadGame.Instance ist null in LoadGameStorage. Standardwerte werden beibehalten.");
            }
        }
        
        private void LoadLastScene()
        {
            // Überprüfe, ob eine Szene geladen werden soll und es nicht bereits die aktive Szene ist.
            if (!string.IsNullOrEmpty(lastScene) && SceneManager.GetActiveScene().name != lastScene)
            {
                Debug.Log($"<color=green>[RSM]</color> LoadLastScene: Lade Szene '{lastScene}'. Aktuelle Szene: {SceneManager.GetActiveScene().name}");
                SceneManager.LoadScene(lastScene);
            }
            else
            {
                Debug.Log($"<color=green>[RSM]</color> LoadLastScene: Szene '{lastScene}' ist bereits aktiv oder nicht spezifiziert. Kein Szenenwechsel.");
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

        private void CheckInitialGameStart()
        {
            bool needsResetOrInitialSetup = false;
            string reason = "";

            if (this.isInitialGameStart)
            {
                needsResetOrInitialSetup = true;
                reason = "IsInitialGameStart aus Speicherdatei war true.";
            }
            else if (this.heartStorageValue <= 0)
            {
                needsResetOrInitialSetup = true;
                reason = "HeartStorageValue aus Speicherdatei war <= 0.";
            }

            if (needsResetOrInitialSetup)
            {
                Debug.LogWarning($"<color=green>[RSM]</color> CheckInitialGameStart: {reason} Setze Spiel auf Standardwerte für Tutorial-Start.");
                
                SaveGame.SaveDataObject dataToSave = new SaveGame.SaveDataObject();
                dataToSave.IsInitialGameStart = false; 
                dataToSave.HeartValue = 1;
                // StarValue auf 0 setzen, wenn es ein initialer Start war ODER wenn Herzen <= 0 waren (kompletter Reset bei Tod)
                dataToSave.StarValue = (this.isInitialGameStart || this.heartStorageValue <= 0) ? 0 : this.starStorageValue; 
                dataToSave.LastScene = "Tutorial";

                SaveGame.SaveGameData(dataToSave);
                Debug.Log("<color=green>[RSM]</color> CheckInitialGameStart: Neue Standardwerte gespeichert.");

                this.isInitialGameStart = dataToSave.IsInitialGameStart;
                this.heartStorageValue = dataToSave.HeartValue;
                this.starStorageValue = dataToSave.StarValue;
                this.lastScene = dataToSave.LastScene;
            }
            else
            {
                Debug.Log("<color=green>[RSM]</color> CheckInitialGameStart: Kein initialer Spielstart oder Reset nötig.");
            }
        }
    }
}