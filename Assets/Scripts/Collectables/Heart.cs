using UnityEngine;
using ResourceSystem.Manager;

namespace Collectables
{
    public class Heart : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.AddHeart();
                }
                
                if (GameManagement.ObjectCounter.Instance != null)
                {
                    GameManagement.ObjectCounter.Instance.ReportObjectCleared();
                }
                Destroy(gameObject);
            }
        }
    }
}