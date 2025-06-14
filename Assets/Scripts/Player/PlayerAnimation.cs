using UnityEngine;
using Enemies;
using ResourceSystem.Storage;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        public static event System.Action OnPlayerDied;

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private static PlayerAnimation _instance;
        public static PlayerAnimation Instance => _instance;

        private Vector2 _lastDirection = Vector2.down;

        private float _walkAnimationLock;

        // Animationswerte
        [SerializeField] private float attackAnimationDuration = 0.4f;
        [SerializeField] private float damageAnimationDuration = 0.3f;
        [SerializeField] private float attackRange = 1.0f;
        [SerializeField] private int attackDamage = 1;

        private float _attackAnimationLock;
        private float _damageAnimationLock;
        private int _previousHeartValue;

        private bool _attackAnimationStarted;
        private bool _damageAnimationStarted;
        private bool _isActuallyDead = false; 

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
                _previousHeartValue = 1;
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
                            Debug.Log("Spieler ist gestorben. Benachrichtige GameController.");

                            OnPlayerDied?.Invoke();
                            
                            Destroy(gameObject);
                            return; 
                        }
                    }
                    return;
                }

                // Laufanimations Sperre
                if (_walkAnimationLock > 0f)
                    _walkAnimationLock -= Time.deltaTime;

                // Angriffanimations Sperre
                if (_attackAnimationLock > 0f)
                {
                    _attackAnimationLock -= Time.deltaTime;
                    if (_attackAnimationLock <= 0f)
                    {
                        _attackAnimationStarted = false;
                        _walkAnimationLock = 0f; 
                        _lastPlayedAnimationName = "";
                    }
                }
                
                if (_damageAnimationLock > 0f)
                {
                    _damageAnimationLock -= Time.deltaTime;
                    if (_damageAnimationLock <= 0f)
                    {
                        _damageAnimationStarted = false;
                        _walkAnimationLock = 0f;
                        _lastPlayedAnimationName = "";
                    }
                }
                
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

                // Angriffssteuerung
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
            // Bestimme die Angriffsposition basierend auf der letzten Bewegungsrichtung.
            Vector2 attackPosition = (Vector2)transform.position + _lastDirection.normalized * (attackRange / 2);
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange / 2);

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                if (enemyCollider.gameObject.CompareTag("Enemy")) 
                {
                    Slime slime = enemyCollider.GetComponent<Slime>();
                    if (slime != null)
                    {
                        slime.TakeDamage(attackDamage);
                    }

                    Skeleton skeleton = enemyCollider.GetComponent<Skeleton>();
                    if (skeleton != null)
                    {
                        skeleton.TakeDamage(attackDamage);
                    }

                    DestroyableTree tree = enemyCollider.GetComponent<DestroyableTree>();
                    if (tree != null)
                    {
                        tree.TakeDamage(attackDamage);
                    }
                }
            }
        }

        private void HandleAnimation()
        {
            if (_animator == null || _spriteRenderer == null) return;
            if (PlayerMovement.Instance == null) return;

            // 1. Todesanimation
            if (ResourceStorageManager.Instance != null && ResourceStorageManager.Instance.HeartStorageValue <= 0)
            {
                if (!_damageAnimationStarted)
                {
                    PlayDirectionalAnimation("Damage", _lastDirection);
                    _damageAnimationStarted = true; 
                }
                return;
            }

            // 2. Schadensanimation
            if (_damageAnimationLock > 0f)
            {
                if (!_damageAnimationStarted)
                {
                    PlayDirectionalAnimation("Damage", _lastDirection);
                    _damageAnimationStarted = true;
                }
                return;
            }

            // 3. Angriffsanimation
            if (_attackAnimationLock > 0f) 
            {
                if (!_attackAnimationStarted)
                {
                    PlayDirectionalAnimation("Attack", _lastDirection);
                    _attackAnimationStarted = true;
                }
                return; 
            }
            
            // 4. Stand- und Walkanimation
            Vector2 movementDirection = PlayerMovement.Instance.MovementDirection;
            bool isMoving = PlayerMovement.Instance.IsMoving;

            if (isMoving)
            {
                _lastDirection = movementDirection.normalized;
                _walkAnimationLock = 0.15f; 
                PlayDirectionalAnimation("Walk", _lastDirection);
            }
            else
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
            bool targetFlipX = _spriteRenderer.flipX;

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
                    targetFlipX = false;
                }
                else
                {
                    targetAnimationName = prefix + "Front";
                    targetFlipX = false;
                }
            }
            
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