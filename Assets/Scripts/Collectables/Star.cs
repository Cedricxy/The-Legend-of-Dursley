using UnityEngine;
using ResourceSystem.Manager;
using GameManagement;

namespace Collectables
{
    public class Star : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.AddStar();
                }
                
                if (ObjectCounter.Instance != null)
                {
                    ObjectCounter.Instance.ReportObjectCleared();
                }
                Destroy(gameObject);
            }
        }
    }
}