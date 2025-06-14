using UnityEngine;
using ResourceSystem.Storage; // Zugriff auf ResourceStorageManager

namespace Enemies
{
    public class Slime : MonoBehaviour
    {
        [SerializeField] private int damageAmount = 1; // Schaden, den der Slime verursacht
        [SerializeField] private int maxHealth = 5;    // Maximale Lebenspunkte des Slimes
        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Überprüfen, ob das kollidierende Objekt der Spieler ist
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Slime hat Spieler berührt.");
                // Greife auf den ResourceStorageManager zu, um dem Spieler Schaden zuzufügen
                if (ResourceStorageManager.Instance != null)
                {
                    ResourceStorageManager.Instance.HeartStorageValue -= damageAmount;
                    if (ResourceStorageManager.Instance.HeartStorageValue < 0)
                    {
                        ResourceStorageManager.Instance.HeartStorageValue = 0;
                    }
                    Debug.Log($"Spieler hat {damageAmount} Schaden erhalten. Aktuelle Herzen: {ResourceStorageManager.Instance.HeartStorageValue}");
                }
                else
                {
                    Debug.LogError("ResourceStorageManager.Instance ist nicht gefunden worden, um Schaden zu verursachen.");
                }
            }
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            Debug.Log($"Slime hat {amount} Schaden erhalten. Aktuelle HP: {_currentHealth}/{maxHealth}");
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Slime besiegt!");
            // Hier könntest du noch eine Todesanimation, Soundeffekte oder Loot-Drops hinzufügen
            Destroy(gameObject);
        }
    }
}