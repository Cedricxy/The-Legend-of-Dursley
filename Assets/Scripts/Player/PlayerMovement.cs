/* using UnityEngine;

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
            if (Time.timeScale != 0f)
                HandleInput();
        }

        private void FixedUpdate()
        {
            if (Time.timeScale != 0f)
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

*/

using UnityEngine;
using Player; // Erforderlich für den Zugriff auf PlayerAnimation

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

        private bool _movementDisabled = false; // Neuer Flag, um die Bewegung zu sperren

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
            if (Time.timeScale != 0f)
                HandleInput();
        }

        private void FixedUpdate()
        {
            if (Time.timeScale != 0f)
                MovePlayer();
        }

        private void HandleInput()
        {
            // Wenn eine Animation die Bewegung blockiert oder die Bewegung gesperrt ist, setze die Bewegungsrichtung auf Null und ignoriere die Eingabe.
            if ((PlayerAnimation.Instance != null && PlayerAnimation.Instance.IsAnimationBlockingMovement) || _movementDisabled)
            {
                _movementDirection = Vector2.zero;
                return;
            }

            _movementDirection = Vector2.zero; // Setze die Bewegungsrichtung für die aktuelle Frame-Eingabe zurück

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

            // Wenn eine Animation die Bewegung blockiert oder die Bewegung gesperrt ist, stoppe den Spieler aktiv.
            if ((PlayerAnimation.Instance != null && PlayerAnimation.Instance.IsAnimationBlockingMovement) || _movementDisabled)
            {
                _rb.linearVelocity = Vector2.zero;
            }
            else // Andernfalls normale Bewegungslogik anwenden
            {
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

        // Neue öffentliche Methode, um die Bewegung von außen zu sperren (z.B. bei Tod)
        public void DisableMovement()
        {
            _movementDisabled = true;
            _movementDirection = Vector2.zero; // Stellt sicher, dass keine Restbewegung vorhanden ist
            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero; // Stoppt den Rigidbody sofort
            }
            Debug.Log("PlayerMovement: Bewegung deaktiviert.");
        }
    }
}