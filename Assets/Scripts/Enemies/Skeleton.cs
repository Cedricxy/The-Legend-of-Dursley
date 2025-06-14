using UnityEngine;
using ResourceSystem.Storage;

namespace Enemies
{
    public class Skeleton : MonoBehaviour
    {
        [SerializeField] private int damageAmount = 2;
        [SerializeField] private int maxHealth = 10;
        private int _currentHealth;
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float waitTimeAtPoints = 1f; // Optionale Wartezeit an den Punkten.
        private Transform _currentTarget;
        private bool _isWaiting = false;
        private float _waitTimer = 0f;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rb;
        private Animator _animator;
        private Vector2 _currentFacingDirection = Vector2.right;
        private string _lastPlayedAnimationName = "";

        private void Awake()
        {
            _currentHealth = maxHealth;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (pointA == null || pointB == null)
            {
                enabled = false; 
                return;
            }
            // Start bei Punkt A, dann Punkt B.
            _currentTarget = pointA;
            FlipSpriteToTarget(_currentTarget.position); 
        }

        void Update()
        {
            if (pointA == null || pointB == null || _currentTarget == null) return;

            string animationPrefix;
            Vector2 animationDirectionForLogic;

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
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(_currentTarget.position.x, transform.position.y), speed * Time.deltaTime);
            }
            else
            {
                 _rb.linearVelocity = new Vector2(directionToTarget.x * speed, _rb.linearVelocity.y);
            }

            // Überprüft, ob das Ziel erreicht wurde.
            if (Mathf.Abs(transform.position.x - _currentTarget.position.x) < 0.1f)
            {
                transform.position = new Vector2(_currentTarget.position.x, transform.position.y);
                if (_rb.bodyType == RigidbodyType2D.Dynamic) _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
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
                    _spriteRenderer.flipX = true;
                }
                else if (targetPosition.x > transform.position.x)
                {
                    _spriteRenderer.flipX = false;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
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

        private void PlaySkeletalAnimation(string prefix, Vector2 direction)
        {
            if (_animator == null || _spriteRenderer == null)
            {
                return;
            }

            string targetAnimationName = "";
            bool targetFlipX = _spriteRenderer.flipX;

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
                targetAnimationName = "StandRight";
                targetFlipX = _currentFacingDirection.x < 0;
            }
            
            if (string.IsNullOrEmpty(targetAnimationName))
            {
                targetAnimationName = "StandRight"; 
                targetFlipX = _currentFacingDirection.x < 0;
            }
            
            if (targetAnimationName != _lastPlayedAnimationName || targetFlipX != _spriteRenderer.flipX)
            {
                if (_animator.HasState(0, Animator.StringToHash(targetAnimationName)))
                {
                    _animator.Play(targetAnimationName, 0, 0f); // Spielt Animation von vorne ab.
                    _spriteRenderer.flipX = targetFlipX;
                    _lastPlayedAnimationName = targetAnimationName;
                }
            }
        }

        private void Die()
        {
            if (GameManagement.ObjectCounter.Instance != null)
            {
                GameManagement.ObjectCounter.Instance.ReportObjectCleared();
            }

            gameObject.SetActive(false);
        }
    }
}