/* using UnityEngine;
using ResourceSystem.Storage;

namespace ResourceSystem.Manager
{
    public class ResourceManager : MonoBehaviour
    {
        public void Start()
        {
            CheckUsableGold();
            CheckUsableElixir();
        }

        public void Update()
        {
            CheckUsableGold();
            CheckUsableElixir();
        }

        private static int CheckUsableGold()
        {
            if (ResourceStorageManager.Instance == null)
                return 0;
            return ResourceStorageManager.Instance.GoldStorageValue;
        }

        private static int CheckUsableElixir()
        {
            if (ResourceStorageManager.Instance == null)
                return 0;
            return ResourceStorageManager.Instance.ElixirStorageValue;
        }
    }
} */

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
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddHeart(int amount = 1)
        {
            Debug.Log($"ResourceManager.AddHeart: Methode aufgerufen. ResourceManager Instanz-ID: {this.GetInstanceID()}");
            if (ResourceStorageManager.Instance != null)
            {
                Debug.Log($"ResourceManager.AddHeart: Vor Inkrement: RSM.HeartStorageValue = {ResourceStorageManager.Instance.HeartStorageValue}, RSM Instanz-ID: {ResourceStorageManager.Instance.GetInstanceID()}");
                ResourceStorageManager.Instance.HeartStorageValue += amount;
                Debug.Log($"ResourceManager.AddHeart: Nach Inkrement: RSM.HeartStorageValue = {ResourceStorageManager.Instance.HeartStorageValue}");
                SaveAndLoad.SaveGame.SaveGameData(); 
            }
            else
            {
                Debug.LogError("ResourceManager.AddHeart: ResourceStorageManager.Instance ist null!");
            }
        }

        public void AddStar(int amount = 1)
        {
            Debug.Log($"ResourceManager.AddStar: Methode aufgerufen. ResourceManager Instanz-ID: {this.GetInstanceID()}");
            if (ResourceStorageManager.Instance != null)
            {
                Debug.Log($"ResourceManager.AddStar: Vor Inkrement: RSM.StarStorageValue = {ResourceStorageManager.Instance.StarStorageValue}, RSM Instanz-ID: {ResourceStorageManager.Instance.GetInstanceID()}");
                ResourceStorageManager.Instance.StarStorageValue += amount;
                Debug.Log($"ResourceManager.AddStar: Nach Inkrement: RSM.StarStorageValue = {ResourceStorageManager.Instance.StarStorageValue}");
                SaveAndLoad.SaveGame.SaveGameData();
            }
            else
            {
                Debug.LogError("ResourceManager.AddStar: ResourceStorageManager.Instance ist null!");
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

        private static int CheckUsableHearts()
        {
            if (ResourceStorageManager.Instance == null)
                return 0;
            return ResourceStorageManager.Instance.HeartStorageValue;
        }

        private static int CheckUsableStars()
        {
            if (ResourceStorageManager.Instance == null)
                return 0;
            return ResourceStorageManager.Instance.StarStorageValue;
        }
    }
}