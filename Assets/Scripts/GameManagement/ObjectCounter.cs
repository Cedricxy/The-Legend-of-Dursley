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
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
                    _targetClearCount = 16; // So viele Objekte müssen im Tutorial zerstört werden.
                    break;
                case "Kapitel1":
                    _targetClearCount = 32;
                    break;
                case "Kapitel2":
                    _targetClearCount = 16;
                    break;
                default:
                    _targetClearCount = 0;
                    return;
            }
        }

        public void ReportObjectCleared()
        {
            if (_targetClearCount == 0)
            {
                return;
            }

            _currentClearedCount++;

            if (_currentClearedCount >= _targetClearCount)
            {
                OnSceneObjectivesCompleted?.Invoke();
                
                // Setzt den Zähler zurück.
                _currentClearedCount = 0; 
                _targetClearCount = 0; 
            }
        }
    }
}