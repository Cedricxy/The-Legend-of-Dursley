using UnityEngine;

namespace Enemies
{
    public class DestroyableTree : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 2;
        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth -= amount;

            if (_currentHealth <= 0)
            {
                DestroyObject();
            }
        }

        private void DestroyObject()
        {
            if (GameManagement.ObjectCounter.Instance != null)
            {
                GameManagement.ObjectCounter.Instance.ReportObjectCleared();
            }
            gameObject.SetActive(false); // Deaktiviert den Baum.
        }
    }
}