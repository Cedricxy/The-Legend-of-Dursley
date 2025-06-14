using UnityEngine;
using ResourceSystem.Storage;

namespace Enemies
{
    public class Slime : MonoBehaviour
    {
        [SerializeField] private int damageAmount = 1; // Maximaler Schaden
        [SerializeField] private int maxHealth = 5;    // Maximale Herzen
        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Kollisionsprüfung mit dem Spieler.
            if (other.gameObject.CompareTag("Player"))
            {
                // Greift auf den ResourceStorageManager zu, um dem Spieler Schaden zuzufügen.
                if (ResourceStorageManager.Instance != null)
                {
                    ResourceStorageManager.Instance.HeartStorageValue -= damageAmount;
                    if (ResourceStorageManager.Instance.HeartStorageValue < 0)
                    {
                        ResourceStorageManager.Instance.HeartStorageValue = 0;
                    }
                }
            }
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (GameManagement.ObjectCounter.Instance != null)
            {
                GameManagement.ObjectCounter.Instance.ReportObjectCleared();
            }
            Destroy(gameObject);
        }
    }
}