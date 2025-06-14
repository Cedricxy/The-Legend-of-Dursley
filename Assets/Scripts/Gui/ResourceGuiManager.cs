using UnityEngine;
using TMPro;
using ResourceSystem.Storage;

namespace Gui
{
    public class ResourceGuiManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI starCounter;
        [SerializeField] private TextMeshProUGUI heartCounter;

        public void Start()
        {
            UpdateGui();
        }

        public void Update()
        {
            UpdateGui();
        }

        private void UpdateGui()
        {
            if (ResourceStorageManager.Instance == null)
                return;

            int displayableHearts = ResourceStorageManager.Instance.HeartStorageValue;
            int displayableStars = ResourceStorageManager.Instance.StarStorageValue;

            heartCounter.text = displayableHearts.ToString();
            starCounter.text = displayableStars.ToString();
        }
    }
}