using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 7.5f;

        private Vector2 _movementDirection = Vector2.zero;
        private Rigidbody2D _rb;
        
        private static PlayerMovement _instance;
        public static PlayerMovement Instance => _instance;
        
        public Vector2 MovementDirection => _movementDirection;
        public bool IsMoving => _movementDirection != Vector2.zero;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            _instance = this;
            
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void HandleInput()
        {
            _movementDirection = Vector2.zero;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                _movementDirection.x = +1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                _movementDirection.x = -1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                _movementDirection.y = +1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                _movementDirection.y = -1f;
        }

        private void MovePlayer()
        {
            if (_rb == null)
            {
                return;
            }

            if (_movementDirection != Vector2.zero)
            {
                Vector2 movement = _movementDirection.normalized * movementSpeed;
                _rb.linearVelocity = movement;
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
            }
        }
    }
}