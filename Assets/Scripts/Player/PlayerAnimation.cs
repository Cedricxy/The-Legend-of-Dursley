using UnityEngine;

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

            Vector2 movementDirection = PlayerMovement.Instance.MovementDirection;
            bool isMoving = PlayerMovement.Instance.IsMoving;

            if (isMoving)
            {
                _lastDirection = movementDirection;
                
                _walkAnimationLock = 0.15f;

                if (movementDirection.x > 0)
                {
                    _animator.Play("WalkRight");
                    _spriteRenderer.flipX = false;
                }
                else if (movementDirection.x < 0)
                {
                    _animator.Play("WalkRight");
                    _spriteRenderer.flipX = true;
                }
                else if (movementDirection.y > 0)
                {
                    _animator.Play("WalkBack");
                    _spriteRenderer.flipX = false;
                }
                else if (movementDirection.y < 0)
                {
                    _animator.Play("WalkFront");
                    _spriteRenderer.flipX = false;
                }
            }
            else
            {
                if (_walkAnimationLock > 0f) return;

                if (_lastDirection.x > 0)
                {
                    _animator.Play("StandRight");
                    _spriteRenderer.flipX = false;
                }
                else if (_lastDirection.x < 0)
                {
                    _animator.Play("StandRight");
                    _spriteRenderer.flipX = true;
                }
                else if (_lastDirection.y > 0)
                {
                    _animator.Play("StandBack");
                    _spriteRenderer.flipX = false;
                }
                else
                {
                    _animator.Play("StandFront");
                    _spriteRenderer.flipX = false;
                }
            }
        }
    }
}