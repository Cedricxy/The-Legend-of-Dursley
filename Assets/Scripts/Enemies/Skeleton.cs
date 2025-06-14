using UnityEngine;
using ResourceSystem.Storage; // Für Zugriff auf ResourceStorageManager

namespace Enemies
{
    public class Skeleton : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int damageAmount = 2;
        [SerializeField] private int maxHealth = 10;
        private int _currentHealth;

        [Header("Patrol")]
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float waitTimeAtPoints = 1f; // Optionale Wartezeit an den Punkten
        private Transform _currentTarget;
        private bool _isWaiting = false;
        private float _waitTimer = 0f;
        private SpriteRenderer _spriteRenderer; // Um die Blickrichtung anzupassen

        private Rigidbody2D _rb;
        private Animator _animator;
        private Vector2 _currentFacingDirection = Vector2.right; // Standardmäßig nach rechts schauen
        private string _lastPlayedAnimationName = "";

        // private bool _lastFlipXState = false; // _spriteRenderer.flipX wird direkt verwendet

        private void Awake()
        {
            _currentHealth = maxHealth;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            if (_spriteRenderer == null)
            {
                Debug.LogError("Skeleton benötigt eine SpriteRenderer Komponente für die Blickrichtung.", this);
            }
            if (_rb == null)
            {
                Debug.LogError("Skeleton benötigt eine Rigidbody2D Komponente.", this);
            }
            if (_animator == null)
            {
                Debug.LogError("Skeleton benötigt eine Animator Komponente.", this);
            }
        }

        void Start()
        {
            if (pointA == null || pointB == null)
            {
                Debug.LogError("Bitte weise Punkt A und Punkt B für die Patrouille des Skeletts im Inspektor zu.", this);
                enabled = false; 
                return;
            }
            // Starte mit Punkt A als erstem Ziel
            _currentTarget = pointA;
            // Optional: Skelett direkt an Punkt A positionieren, wenn es nicht schon dort ist.
            // transform.position = new Vector2(pointA.position.x, transform.position.y); // Nur X-Position anpassen, Y beibehalten
            FlipSpriteToTarget(_currentTarget.position); 
        }

        void Update()
        {
            if (pointA == null || pointB == null || _currentTarget == null) return;

            string animationPrefix;
            Vector2 animationDirectionForLogic; // Richtung für die Logik (wohin es schaut/gehen will)

            if (_isWaiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= waitTimeAtPoints)
                {
                    _isWaiting = false;
                    _waitTimer = 0f;
                    SwitchTarget(); 
                    _currentFacingDirection = (_currentTarget.position.x > transform.position.x) ? Vector2.right : Vector2.left;
                }
                
                if (_rb != null && _rb.bodyType == RigidbodyType2D.Dynamic)
                {
                    _rb.linearVelocity = Vector2.zero;
                }
                animationPrefix = "Stand";
                animationDirectionForLogic = _currentFacingDirection;
            }
            else 
            {
                MoveTowardsTarget();
                animationPrefix = "Walk";
                animationDirectionForLogic = (_currentTarget.position.x > transform.position.x) ? Vector2.right : Vector2.left;
                _currentFacingDirection = animationDirectionForLogic;
            }
            PlaySkeletalAnimation(animationPrefix, animationDirectionForLogic);
        }

        private void MoveTowardsTarget()
        {
            Vector2 directionToTarget = (_currentTarget.position - transform.position).normalized;
            
            if (_rb.bodyType == RigidbodyType2D.Kinematic)
            {
                transform.position = Vector2.MoveTowards(transform.position, 
                                                         new Vector2(_currentTarget.position.x, transform.position.y), // Nur auf X-Achse bewegen
                                                         speed * Time.deltaTime);
            }
            else // Dynamic Rigidbody
            {
                 _rb.linearVelocity = new Vector2(directionToTarget.x * speed, _rb.linearVelocity.y);
            }

            // Überprüfe, ob das Ziel (nur X-Position) erreicht wurde
            if (Mathf.Abs(transform.position.x - _currentTarget.position.x) < 0.1f)
            {
                transform.position = new Vector2(_currentTarget.position.x, transform.position.y); // Exakt positionieren
                if (_rb.bodyType == RigidbodyType2D.Dynamic) _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y); // Stoppe X-Bewegung
                _isWaiting = true; 
            }
        }


        private void SwitchTarget()
        {
            if (_currentTarget == pointA)
            {
                _currentTarget = pointB;
            }
            else
            {
                _currentTarget = pointA;
            }
            FlipSpriteToTarget(_currentTarget.position);
        }

        private void FlipSpriteToTarget(Vector3 targetPosition)
        {
            if (_spriteRenderer != null)
            {
                if (targetPosition.x < transform.position.x)
                {
                    _spriteRenderer.flipX = true; // Schaut nach links
                }
                else if (targetPosition.x > transform.position.x)
                {
                    _spriteRenderer.flipX = false; // Schaut nach rechts
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Skelett hat Spieler berührt.");
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
                    Debug.LogError("ResourceStorageManager.Instance ist nicht gefunden worden, um Schaden zu verursachen.", this);
                }
            }
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            Debug.Log($"<color=orange>[Skeleton]</color> Skelett hat {amount} Schaden erhalten. Aktuelle HP: {_currentHealth}/{maxHealth}", this); // Geändert
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void PlaySkeletalAnimation(string prefix, Vector2 direction)
        {
            if (_animator == null || _spriteRenderer == null)
            {
                Debug.LogError("[Skeleton] Animator oder SpriteRenderer ist null.", this);
                return;
            }

            string targetAnimationName = "";
            bool targetFlipX = _spriteRenderer.flipX; // Standardmäßig aktuellen Flip beibehalten

            // Basierend auf dem Präfix die Zielanimation bestimmen.
            // Für ein rein horizontal bewegtes Skelett sind "Right"-Animationen mit Spiegelung üblich.
            if (prefix == "Walk")
            {
                targetAnimationName = "WalkRight"; 
                targetFlipX = direction.x < 0;     
            }
            else if (prefix == "Stand")
            {
                targetAnimationName = "StandRight"; 
                targetFlipX = direction.x < 0; 
            }
            else
            {
                Debug.LogWarning($"[Skeleton] Unbekannter Animationspräfix: {prefix}. Standard zu StandRight.", this);
                targetAnimationName = "StandRight";
                targetFlipX = _currentFacingDirection.x < 0;
            }
            
            // Sicherstellen, dass targetAnimationName nicht leer ist (sollte durch obige Logik nicht passieren)
            if (string.IsNullOrEmpty(targetAnimationName))
            {
                Debug.LogError($"[Skeleton] targetAnimationName wurde für Präfix '{prefix}' leer. Standard zu StandRight.", this);
                targetAnimationName = "StandRight"; 
                targetFlipX = _currentFacingDirection.x < 0;
            }

            // Debug-Ausgabe vor dem Abspielen
            // Debug.Log($"[Skeleton] Versuch: Pref:{prefix}, Dir:{direction}, TargetAnim:{targetAnimationName}, TargetFlip:{targetFlipX}. LastAnim:{_lastPlayedAnimationName}, CurrentFlip:{_spriteRenderer.flipX}", this);

            // Nur Animation aktualisieren, wenn sie sich geändert hat oder der Flip-Status anders ist
            if (targetAnimationName != _lastPlayedAnimationName || targetFlipX != _spriteRenderer.flipX)
            {
                // Überprüfen, ob der State existiert, bevor er abgespielt wird
                if (_animator.HasState(0, Animator.StringToHash(targetAnimationName)))
                {
                    Debug.Log($"[Skeleton] Spiele Animation: {targetAnimationName}, Setze FlipX auf: {targetFlipX}", this);
                    _animator.Play(targetAnimationName, 0, 0f); // Spiele Animation von vorne ab
                    _spriteRenderer.flipX = targetFlipX;
                    _lastPlayedAnimationName = targetAnimationName;
                }
                else
                {
                    Debug.LogError($"[Skeleton] Animations-State '{targetAnimationName}' existiert nicht im Animator Controller auf Layer 0. Aktuell spielt möglicherweise '{_lastPlayedAnimationName}' oder Default.", this);
                    // Wenn der gewünschte State nicht existiert, könnte hier ein Fallback zu einer bekannten Default-Animation (z.B. "StandFront", falls vorhanden) sinnvoll sein.
                    // Vorerst wird nur ein Fehler geloggt und die vorherige Animation/Flip beibehalten oder der Animator-Default greift.
                }
            }
            // else
            // {
            //     Debug.Log($"[Skeleton] Animation {targetAnimationName} (FlipX: {targetFlipX}) ist bereits aktiv oder Flip-Status ist identisch.", this);
            // }
        }

        private void Die()
        {
            Debug.Log("<color=orange>[Skeleton]</color> Skelett besiegt! Versuch, GameObject zu deaktivieren.", this); // Geändert
            // Hier könntest du noch eine Todesanimation, Soundeffekte oder Loot-Drops hinzufügen
            // Destroy(gameObject); // Vorerst auskommentiert für Diagnose
            gameObject.SetActive(false); // Testweise deaktivieren
            Debug.Log($"<color=orange>[Skeleton]</color> GameObject {this.gameObject.name} Active-Status nach SetActive(false): {this.gameObject.activeSelf}", this);
            // TODO: Implementiere eine Todesanimation und verzögere das Zerstören des GameObjects
            // Zum Beispiel:
            // StartCoroutine(DeathSequence()); 
        }
    }
}