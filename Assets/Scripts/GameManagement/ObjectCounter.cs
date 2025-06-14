using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public class ObjectCounter : MonoBehaviour
    {
        public static ObjectCounter Instance { get; private set; }

        private int _targetClearCount;
        private int _currentClearedCount;

        public static event System.Action OnSceneObjectivesCompleted;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Wenn ObjectCounter auf einem GameObject ist, das DontDestroyOnLoad verwendet (z.B. GameController),
                // dann wird es auch persistent. Ansonsten hier DontDestroyOnLoad(gameObject); hinzufügen.
                Debug.Log("[ObjectCounter] Instance created.");
            }
            else
            {
                Debug.LogWarning("[ObjectCounter] Duplicate instance detected. Destroying new one.");
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("[ObjectCounter] Subscribed to sceneLoaded event.");
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("[ObjectCounter] Unsubscribed from sceneLoaded event.");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeForScene(scene.name);
        }

        private void InitializeForScene(string sceneName)
        {
            _currentClearedCount = 0;
            switch (sceneName)
            {
                case "Tutorial":
                    _targetClearCount = 16;
                    break;
                case "Kapitel1": // Stelle sicher, dass deine Szene genau so heißt
                    _targetClearCount = 32;
                    break;
                case "Kapitel2": // Stelle sicher, dass deine Szene genau so heißt
                    _targetClearCount = 16;
                    break;
                default:
                    _targetClearCount = 0; // Keine Ziele für diese Szene definiert oder Szene nicht relevant
                    Debug.Log($"[ObjectCounter] No objective target count defined for scene '{sceneName}'.");
                    return;
            }
            Debug.Log($"[ObjectCounter] Initialized for scene '{sceneName}'. Target objectives: {_targetClearCount}. Current: {_currentClearedCount}");
        }

        public void ReportObjectCleared()
        {
            if (_targetClearCount == 0)
            {
                // Nicht in einer Szene mit Zielen oder Ziele bereits erreicht und zurückgesetzt.
                return;
            }

            _currentClearedCount++;
            Debug.Log($"[ObjectCounter] Object cleared. Progress: {_currentClearedCount}/{_targetClearCount}");

            if (_currentClearedCount >= _targetClearCount)
            {
                Debug.Log($"[ObjectCounter] All {_targetClearCount} objectives completed for scene '{SceneManager.GetActiveScene().name}'!");
                OnSceneObjectivesCompleted?.Invoke();
                // Zähler zurücksetzen, um erneutes Auslösen in derselben Szene zu verhindern, falls das Objekt bestehen bleibt.
                _currentClearedCount = 0; 
                _targetClearCount = 0; 
            }
        }
    }
}