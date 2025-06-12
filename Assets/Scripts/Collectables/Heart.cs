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
                Debug.Log("Heart.cs: OnTriggerEnter2D mit Player ausgelöst.");
                if (ResourceManager.Instance != null)
                {
                    Debug.Log($"Heart.cs: ResourceManager.Instance ID: {ResourceManager.Instance.GetInstanceID()}. Rufe AddHeart() auf.");
                    ResourceManager.Instance.AddHeart();
                }
                else
                {
                    Debug.LogError("Heart.cs: ResourceManager.Instance ist null!");
                }
                Destroy(gameObject);
            }
        }
    }
}