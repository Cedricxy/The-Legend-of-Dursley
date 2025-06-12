/* using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ResourceSystem.Storage;

namespace Gui
{
    public class ResourceGuiManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resourceBarGoldValue;
        [SerializeField] private TextMeshProUGUI resourceBarElixirValue;
        [SerializeField] private Image resourceBarGold;
        [SerializeField] private Image resourceBarElixir;
        [SerializeField] private Sprite[] goldFrame;
        [SerializeField] private Sprite[] elixirFrame;

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

            int displayableGold = CheckDisplayableGold();
            int displayableElixir = CheckDisplayableElixir();

            resourceBarGoldValue.text = displayableGold.ToString();
            resourceBarElixirValue.text = displayableElixir.ToString();

            int maxStorageValue = UpdateMaxStorageValue();

            if (maxStorageValue > 0)
            {
                int goldFrameIndex = Mathf.Clamp(displayableGold * (goldFrame.Length - 1) / maxStorageValue, 0, goldFrame.Length - 1);
                resourceBarGold.sprite = goldFrame[goldFrameIndex];

                int elixirFrameIndex = Mathf.Clamp(displayableElixir * (elixirFrame.Length - 1) / maxStorageValue, 0, elixirFrame.Length - 1);
                resourceBarElixir.sprite = elixirFrame[elixirFrameIndex];
            }
        }

        private int UpdateMaxStorageValue()
        {
            return ResourceStorageManager.Instance.MaxStorageValue;
        }

        private static int CheckDisplayableGold()
        {
            return ResourceStorageManager.Instance.GoldStorageValue;
        }

        private static int CheckDisplayableElixir()
        {
            return ResourceStorageManager.Instance.ElixirStorageValue;
        }
    }
} */

using UnityEngine;
using UnityEngine.UI;
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
            
            Debug.Log($"ResourceGuiManager - Heart Value: {ResourceStorageManager.Instance.HeartStorageValue}, Star Value: {ResourceStorageManager.Instance.StarStorageValue}");
        }
    }
}