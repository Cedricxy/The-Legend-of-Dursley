using UnityEngine;
using ResourceSystem.Manager;

namespace Collectables
{
    public class Star : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Star.cs: OnTriggerEnter2D mit Player ausgelöst.");
                if (ResourceManager.Instance != null)
                {
                    Debug.Log($"Star.cs: ResourceManager.Instance ID: {ResourceManager.Instance.GetInstanceID()}. Rufe AddStar() auf.");
                    ResourceManager.Instance.AddStar();
                }
                else
                {
                    Debug.LogError("Star.cs: ResourceManager.Instance ist null!");
                }

                Destroy(gameObject);
            }
        }
    }
}