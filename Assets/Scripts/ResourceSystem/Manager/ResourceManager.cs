using UnityEngine;
using ResourceSystem.Storage;

namespace ResourceSystem.Manager
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

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

        // Fügt dem ResourceStorageManager eine bestimmte Anzahl an Herzen hinzu.
        public void AddHeart(int amount = 1)
        {
            if (ResourceStorageManager.Instance != null)
            {
                ResourceStorageManager.Instance.HeartStorageValue += amount;
                SaveAndLoad.SaveGame.SaveGameData();
            }
        }

        // Fügt dem ResourceStorageManager eine bestimmte Anzahl an Sternen hinzu.
        public void AddStar(int amount = 1)
        {
            if (ResourceStorageManager.Instance != null)
            {
                ResourceStorageManager.Instance.StarStorageValue += amount;
                SaveAndLoad.SaveGame.SaveGameData();
            }
        }
        
        public void Start()
        {
            CheckUsableHearts();
            CheckUsableStars();
        }

        public void Update()
        {
            CheckUsableHearts();
            CheckUsableStars();
        }

        // Gibt die aktuell verfügbare Anzahl an Herzen aus dem ResourceStorageManager zurück.
        private static int CheckUsableHearts()
        {
            if (ResourceStorageManager.Instance == null)
                return 0;
            return ResourceStorageManager.Instance.HeartStorageValue;
        }

        // Gibt die aktuell verfügbare Anzahl an Sternen aus dem ResourceStorageManager zurück.
        private static int CheckUsableStars()
        {
            if (ResourceStorageManager.Instance == null)
                return 0;
            return ResourceStorageManager.Instance.StarStorageValue;
        }
    }
}