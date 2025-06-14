/* using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private static PlayerAnimation _instance;
        public static PlayerAnimation Instance => _instance;
        
        private Vector2 _lastDirection = Vector2.down;

        private float _walkAnimationLock;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            _instance = this;

            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (Time.timeScale != 0f)
            {
                HandleAnimation();
                if (_walkAnimationLock > 0f)
                    _walkAnimationLock -= Time.deltaTime;
            }    
        }

        private void HandleAnimation()
        {
            if (_animator == null || _spriteRenderer == null) return;
            if (PlayerMovement.Instance == null) return;

            // 1. Todesanimation (Höchste Priorität)
            if (ResourceStorageManager.Instance != null && ResourceStorageManager.Instance.HeartStorageValue <= 0)
            {
                if (!_damageAnimationStarted) // _damageAnimationStarted wird auch für die Todesanimation verwendet
                {
                    PlayDirectionalAnimation("Damage", _lastDirection); // Oder eine spezifische "Dead"-Animation
                    _damageAnimationStarted = true; 
                }
                return; // Keine anderen Animationen, wenn tot
            }

            // 2. Nicht-tödliche Schadensanimation
            if (_damageAnimationLock > 0f) // Dieser Lock ist für nicht-tödlichen Schaden
            {
                if (!_damageAnimationStarted)
                {
                    PlayDirectionalAnimation("Damage", _lastDirection);
                    _damageAnimationStarted = true;
                }
                return; // Blockiere andere Animationen während der Schadensanimation
            }

            // 3. Angriffsanimation
            if (_attackAnimationLock > 0f) 
            {
                if (!_attackAnimationStarted) // Nur beim ersten Frame des Angriffs die Animation starten
                {
                    PlayDirectionalAnimation("Attack", _lastDirection);
                    _attackAnimationStarted = true;
                }
                // Solange der _attackAnimationLock aktiv ist, soll die Angriffsanimation
                // (oder das, wozu sie im Animator Controller übergeht, z.B. Stand nach Exit Time)
                // die Priorität haben. Das Skript wird in dieser Zeit keine Walk/Stand Animation setzen.
                return; 
            }
            
            // 4. Bewegungs- und Stehanimationen (wird nur erreicht, wenn keine Locks für Tod, Schaden oder Angriff aktiv sind)
            Vector2 movementDirection = PlayerMovement.Instance.MovementDirection;
            bool isMoving = PlayerMovement.Instance.IsMoving;

            if (isMoving)
            {
                _lastDirection = movementDirection.normalized;
                _walkAnimationLock = 0.15f; 
                PlayDirectionalAnimation("Walk", _lastDirection);
            }
            else // Spieler steht
            {
                if (_attackAnimationLock > 0f)
                {
                    PlayDirectionalAnimation("Stand", _lastDirection);
                }
                else
                {
                    if (_walkAnimationLock > 0f) {
                        return;
                    }
                    PlayDirectionalAnimation("Stand", _lastDirection);
                }
            }
        }

        private void PlayDirectionalAnimation(string prefix, Vector2 direction)
        {
            string targetAnimationName = "";
            bool targetFlipX = _spriteRenderer.flipX; // Behalte aktuellen Flip-Status bei, falls nicht explizit geändert

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                targetAnimationName = prefix + "Right";
                targetFlipX = direction.x < 0;
            }
            else
            {
                if (direction.y > 0)
                {
                    targetAnimationName = prefix + "Back";
                    targetFlipX = false; // Normalerweise nicht geflippt für Rückenansicht
                }
                else // direction.y <= 0 (beinhaltet Stehenbleiben in Frontansicht oder Laufen nach vorne)
                {
                    targetAnimationName = prefix + "Front";
                    targetFlipX = false; // Normalerweise nicht geflippt für Frontansicht
                }
            }

            // Spiele Animation nur, wenn sich der Name oder der Flip-Status geändert hat,
            // um unnötiges Neustarten der Animation zu verhindern.
            if (targetAnimationName != _lastPlayedAnimationName || targetFlipX != _lastFlipXState)
            {
                _animator.Play(targetAnimationName);
                _spriteRenderer.flipX = targetFlipX;

                _lastPlayedAnimationName = targetAnimationName;
                _lastFlipXState = targetFlipX;
            }
        }
    }
}

*/

using UnityEngine;
using Enemies; // Hinzugefügt, um den Slime-Typ aufzulösen
using GameManagement; // Hinzugefügt für den Zugriff auf GameController
using ResourceSystem.Storage; // Hinzugefügt für den Zugriff auf 
using SaveAndLoad; // Hinzugefügt für den Zugriff auf SaveGame

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private static PlayerAnimation _instance;
        public static PlayerAnimation Instance => _instance;

        private Vector2 _lastDirection = Vector2.down;

        private float _walkAnimationLock;

        // Zeiten für die Animations-Locks anpassen
        [SerializeField] private float attackAnimationDuration = 0.4f; // Erhöht für sichtbare Animation
        [SerializeField] private float damageAnimationDuration = 0.3f; // Angepasst
        [SerializeField] private float attackRange = 1.0f; // Reichweite des Angriffs
        [SerializeField] private int attackDamage = 1; // Schaden, den der Spieler verursacht

        private float _attackAnimationLock;
        private float _damageAnimationLock;
        private int _previousHeartValue;

        private bool _attackAnimationStarted;
        private bool _damageAnimationStarted;
        private bool _isActuallyDead = false; 

        // NEU: Felder zum Verfolgen des zuletzt gespielten Animationsstatus
        private string _lastPlayedAnimationName = "";
        private bool _lastFlipXState = false;

        public bool IsAnimationBlockingMovement => _attackAnimationLock > 0f || _damageAnimationLock > 0f;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            _instance = this;

            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (ResourceSystem.Storage.ResourceStorageManager.Instance != null)
            {
                _previousHeartValue = ResourceSystem.Storage.ResourceStorageManager.Instance.HeartStorageValue;
            }
            else
            {
                _previousHeartValue = 1; // Fallback
                Debug.LogWarning("PlayerAnimation: ResourceStorageManager.Instance in Start nicht gefunden.");
            }
        }

        private void Update()
        {
            if (Time.timeScale != 0f)
            {
                if (_isActuallyDead) return; 

                bool isDeadNow = ResourceStorageManager.Instance != null &&
                                 ResourceStorageManager.Instance.HeartStorageValue <= 0;

                if (isDeadNow)
                {
                    if (!_damageAnimationStarted) 
                    {
                        _damageAnimationLock = damageAnimationDuration;
                        _damageAnimationStarted = true; 
                        PlayDirectionalAnimation("Damage", _lastDirection); 
                        if (PlayerMovement.Instance != null)
                        {
                            PlayerMovement.Instance.DisableMovement();
                        }
                    }
                    else if (_damageAnimationLock > 0f) 
                    {
                        _damageAnimationLock -= Time.deltaTime;
                        if (_damageAnimationLock <= 0f) 
                        {
                            _isActuallyDead = true; 
                            Debug.Log("Spieler ist gestorben. Bereite Reset für nächsten Spielstart vor und lade Endscreen.");

                            if (ResourceStorageManager.Instance != null && SaveGame.Instance != null)
                            {
                                Debug.Log("<color=red>[PlayerAnimation]</color> Tod erkannt. Setze RSM für InitialGameStart.");
                                ResourceStorageManager.Instance.IsInitialGameStart = true;
                                ResourceStorageManager.Instance.HeartStorageValue = 0; // Wird von CheckInitialGameStart auf 1 gesetzt
                                ResourceStorageManager.Instance.StarStorageValue = 0;  // Sterne auf 0 für kompletten Neustart
                                ResourceStorageManager.Instance.LastScene = "Tutorial"; // Nächster Start ist Tutorial
                                SaveGame.SaveGameData(); // Speichert diesen Zustand (isInitial=true, 0 Herzen, Tutorial)
                            }
                            else
                            {
                                Debug.LogError("<color=red>[PlayerAnimation]</color> ResourceStorageManager oder SaveGame Instanz nicht gefunden beim Tod! Reset kann nicht korrekt vorbereitet werden.");
                            }

                            if (GameController.Instance != null)
                            {
                                GameController.Instance.LoadEndScreen();
                            }
                            else
                            {
                                Debug.LogError("GameController.Instance ist nicht gefunden worden, um Endscreen zu laden.");
                            }
                            Destroy(gameObject); 
                            return; 
                        }
                    }
                    return;
                }

                // Walk animation lock
                if (_walkAnimationLock > 0f)
                    _walkAnimationLock -= Time.deltaTime;

                // Attack animation lock
                if (_attackAnimationLock > 0f)
                {
                    _attackAnimationLock -= Time.deltaTime;
                    if (_attackAnimationLock <= 0f)
                    {
                        _attackAnimationStarted = false;
                        _walkAnimationLock = 0f; 
                        _lastPlayedAnimationName = ""; // NEU: Erzwingt Neusetzen der nächsten Animation
                    }
                }

                // Non-fatal Damage animation lock countdown
                // This runs if the player is not dead and a damage lock is active.
                if (_damageAnimationLock > 0f) // isDeadNow is false at this point
                {
                    _damageAnimationLock -= Time.deltaTime;
                    if (_damageAnimationLock <= 0f)
                    {
                        _damageAnimationStarted = false;
                        _walkAnimationLock = 0f; // Allow movement immediately after damage animation finishes
                        _lastPlayedAnimationName = ""; // NEU: Auch hier für sauberen Übergang
                    }
                }

                // Detect non-fatal damage and set lock
                if (ResourceStorageManager.Instance != null)
                {
                    int currentHeartValue = ResourceStorageManager.Instance.HeartStorageValue;
                    if (currentHeartValue < _previousHeartValue && currentHeartValue > 0) 
                    {
                        if (_damageAnimationLock <= 0f && _attackAnimationLock <= 0f) 
                        {
                             _damageAnimationLock = damageAnimationDuration;
                             _damageAnimationStarted = false; 
                        }
                    }
                    _previousHeartValue = currentHeartValue;
                }

                // Handle attack input
                if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F)))
                {
                    if (_attackAnimationLock <= 0f && _damageAnimationLock <= 0f) 
                    {
                        _attackAnimationLock = attackAnimationDuration;
                        _attackAnimationStarted = false;
                        PerformAttack();
                    }
                }
                
                HandleAnimation();
            }
        }

        private void PerformAttack()
        {
            // Bestimme die Angriffsposition basierend auf der letzten Bewegungsrichtung
            Vector2 attackPosition = (Vector2)transform.position + _lastDirection.normalized * (attackRange / 2); // Position leicht vor dem Spieler
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange / 2); // Überprüfe einen Kreisbereich
            Debug.Log($"[PlayerAnimation.PerformAttack] OverlapCircleAll an Position {attackPosition} mit Radius {attackRange / 2} hat {hitEnemies.Length} Collider gefunden.");

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                Debug.Log($"[PlayerAnimation.PerformAttack] Getroffener Collider: {enemyCollider.gameObject.name}, Tag: {enemyCollider.gameObject.tag}");
                if (enemyCollider.gameObject.CompareTag("Enemy")) // Stelle sicher, dass der Gegner das Tag "Enemy" hat
                {
                    Slime slime = enemyCollider.GetComponent<Slime>();
                    if (slime != null)
                    {
                        slime.TakeDamage(attackDamage);
                        Debug.Log("Spieler hat Slime getroffen.");
                        // Optional: Treffersound oder -effekt hier abspielen
                    }

                    Skeleton skeleton = enemyCollider.GetComponent<Skeleton>();
                    if (skeleton != null)
                    {
                        skeleton.TakeDamage(attackDamage);
                        Debug.Log("Spieler hat Skelett getroffen.");
                        // Optional: Treffersound oder -effekt hier abspielen
                    }
                }
                else
                {
                    Debug.Log($"[PlayerAnimation.PerformAttack] Getroffener Collider {enemyCollider.gameObject.name} hat NICHT das Tag 'Enemy'. Aktuelles Tag: '{enemyCollider.gameObject.tag}'");
                }
            }
        }

        private void HandleAnimation()
        {
            if (_animator == null || _spriteRenderer == null) return;
            if (PlayerMovement.Instance == null) return;

            // 1. Todesanimation (Höchste Priorität)
            if (ResourceStorageManager.Instance != null && ResourceStorageManager.Instance.HeartStorageValue <= 0)
            {
                if (!_damageAnimationStarted) // _damageAnimationStarted wird auch für die Todesanimation verwendet
                {
                    PlayDirectionalAnimation("Damage", _lastDirection); // Oder eine spezifische "Dead"-Animation
                    _damageAnimationStarted = true; 
                }
                return; // Keine anderen Animationen, wenn tot
            }

            // 2. Nicht-tödliche Schadensanimation
            if (_damageAnimationLock > 0f) // Dieser Lock ist für nicht-tödlichen Schaden
            {
                if (!_damageAnimationStarted)
                {
                    PlayDirectionalAnimation("Damage", _lastDirection);
                    _damageAnimationStarted = true;
                }
                return; // Blockiere andere Animationen während der Schadensanimation
            }

            // 3. Angriffsanimation
            if (_attackAnimationLock > 0f) 
            {
                if (!_attackAnimationStarted) // Nur beim ersten Frame des Angriffs die Animation starten
                {
                    PlayDirectionalAnimation("Attack", _lastDirection);
                    _attackAnimationStarted = true;
                }
                // Solange der _attackAnimationLock aktiv ist, soll die Angriffsanimation
                // (oder das, wozu sie im Animator Controller übergeht, z.B. Stand nach Exit Time)
                // die Priorität haben. Das Skript wird in dieser Zeit keine Walk/Stand Animation setzen.
                return; 
            }
            
            // 4. Bewegungs- und Stehanimationen (wird nur erreicht, wenn keine Locks für Tod, Schaden oder Angriff aktiv sind)
            Vector2 movementDirection = PlayerMovement.Instance.MovementDirection;
            bool isMoving = PlayerMovement.Instance.IsMoving;

            if (isMoving)
            {
                _lastDirection = movementDirection.normalized;
                _walkAnimationLock = 0.15f; 
                PlayDirectionalAnimation("Walk", _lastDirection);
            }
            else // Spieler steht
            {
                if (_attackAnimationLock > 0f)
                {
                    PlayDirectionalAnimation("Stand", _lastDirection);
                }
                else
                {
                    if (_walkAnimationLock > 0f) {
                        return;
                    }
                    PlayDirectionalAnimation("Stand", _lastDirection);
                }
            }
        }

        private void PlayDirectionalAnimation(string prefix, Vector2 direction)
        {
            string targetAnimationName = "";
            bool targetFlipX = _spriteRenderer.flipX; // Behalte aktuellen Flip-Status bei, falls nicht explizit geändert

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                targetAnimationName = prefix + "Right";
                targetFlipX = direction.x < 0;
            }
            else
            {
                if (direction.y > 0)
                {
                    targetAnimationName = prefix + "Back";
                    targetFlipX = false; // Normalerweise nicht geflippt für Rückenansicht
                }
                else // direction.y <= 0 (beinhaltet Stehenbleiben in Frontansicht oder Laufen nach vorne)
                {
                    targetAnimationName = prefix + "Front";
                    targetFlipX = false; // Normalerweise nicht geflippt für Frontansicht
                }
            }

            // Spiele Animation nur, wenn sich der Name oder der Flip-Status geändert hat,
            // um unnötiges Neustarten der Animation zu verhindern.
            if (targetAnimationName != _lastPlayedAnimationName || targetFlipX != _lastFlipXState)
            {
                _animator.Play(targetAnimationName);
                _spriteRenderer.flipX = targetFlipX;

                _lastPlayedAnimationName = targetAnimationName;
                _lastFlipXState = targetFlipX;
            }
        }
    }
}