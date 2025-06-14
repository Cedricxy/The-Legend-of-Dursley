using UnityEngine;
using ResourceSystem.Manager; // Für ResourceManager
using GameManagement; // Für ObjectCounter

namespace Collectables
{
    public class Star : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("[Star.cs] OnTriggerEnter2D mit Player ausgelöst.");
                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.AddStar(); // Methode im ResourceManager zum Hinzufügen von Sternen
                }
                else
                {
                    Debug.LogError("[Star.cs] ResourceManager.Instance ist null!");
                }

                // ObjectCounter informieren
                if (ObjectCounter.Instance != null)
                {
                    ObjectCounter.Instance.ReportObjectCleared();
                }
                else
                {
                    Debug.LogWarning("[Star.cs] ObjectCounter.Instance ist null. Ziel konnte nicht gemeldet werden.");
                }

                Destroy(gameObject);
            }
        }
    }
}