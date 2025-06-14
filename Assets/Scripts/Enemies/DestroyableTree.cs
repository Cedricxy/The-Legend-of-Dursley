using UnityEngine;

namespace Enemies
{
    public class DestroyableTree : MonoBehaviour
    {
        [Header("Tree Stats")]
        [SerializeField] private int maxHealth = 2; // Benötigt 2 Treffer zum Zerstören
        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = maxHealth;
            // Stelle sicher, dass der Baum einen Collider hat, um den Weg zu blockieren.
            // Dieser sollte NICHT als Trigger konfiguriert sein.
            if (GetComponent<Collider2D>() == null)
            {
                Debug.LogWarning($"[DestroyableTree] {gameObject.name} hat keinen Collider2D. Er wird den Spieler nicht blockieren.", this);
            }
        }

        public void TakeDamage(int amount)
        {
            if (_currentHealth <= 0) return; // Bereits zerstört

            _currentHealth -= amount;
            Debug.Log($"<color=#654321>[DestroyableTree]</color> Baum hat {amount} Schaden erhalten. Aktuelle HP: {_currentHealth}/{maxHealth}", this);

            if (_currentHealth <= 0)
            {
                DestroyObject();
            }
        }

        private void DestroyObject()
        {
            Debug.Log("<color=#654321>[DestroyableTree]</color> Baum wurde zerstört!", this);
            // Hier könnte eine Zerstörungsanimation oder Partikeleffekte hinzugefügt werden.

            if (GameManagement.ObjectCounter.Instance != null)
            {
                GameManagement.ObjectCounter.Instance.ReportObjectCleared();
            }
            else
            {
                Debug.LogWarning("[DestroyableTree.cs] ObjectCounter.Instance ist null. Ziel konnte nicht gemeldet werden.");
            }

            gameObject.SetActive(false); // Lässt den Baum verschwinden
        }
    }
}