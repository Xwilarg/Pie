using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Pie.Character
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private TriggerDetector _jumpDetector;
        [SerializeField]
        private TriggerDetector _leftWallDetector, _rightWallDetector;

        private Rigidbody2D _rb;
        private Animator _anim;
        private SpriteRenderer _sr;
        private float _movement;
        private float _baseGravityScale;

        // Jump information
        private const float _jumpForce = 3.5f;
        private bool _jump;
        private bool _isGrounded;

        // Dash information
        private Vector2 _lastDirection; // Last direction we went to
        private Vector2 _dashDirection; // Used while dashing, direction we are heading to
        private float _dashTimer;
        private const float _dashTimerRef = .5f; // Duration of the dash in seconds
        private const float _dashSpeed = 3f;

        private bool? _isAgainstLeftWall;

        private bool IsDashing
        {
            get
            {
                return _dashTimer > 0f;
            }
        }

        private void Start()
        {
            _rb = transform.parent.GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _sr = transform.parent.GetComponentInChildren<SpriteRenderer>();

            Assert.IsNotNull(_jumpDetector, "Jump Detector needs to be assigned in the editor");
            Assert.IsNotNull(_leftWallDetector, "Left Wall Detector needs to be assigned in the editor");
            Assert.IsNotNull(_rightWallDetector, "Right Wall Detector needs to be assigned in the editor");

            _jumpDetector.Tag = "Wall";
            _leftWallDetector.Tag = "Wall";
            _rightWallDetector.Tag = "Wall";

            _jumpDetector.TriggerOn.AddListener(new UnityAction(() =>
            {
                _anim.SetBool("IsJumping", false);
                _isGrounded = true;
            }));
            _jumpDetector.TriggerOff.AddListener(new UnityAction(() =>
            {
                _isGrounded = false;
            }));

            _leftWallDetector.TriggerOn.AddListener(new UnityAction(() =>
            {
                _anim.SetBool("IsAgainstWall", true);
                _isAgainstLeftWall = true;
            }));
            _leftWallDetector.TriggerOff.AddListener(new UnityAction(() =>
            {
                _anim.SetBool("IsAgainstWall", false);
                _isAgainstLeftWall = null;
            }));

            _rightWallDetector.TriggerOn.AddListener(new UnityAction(() =>
            {
                _anim.SetBool("IsAgainstWall", true);
                _isAgainstLeftWall = false;
            }));
            _rightWallDetector.TriggerOff.AddListener(new UnityAction(() =>
            {
                _anim.SetBool("IsAgainstWall", false);
                _isAgainstLeftWall = null;
            }));

            _baseGravityScale = _rb.gravityScale;
        }

        private void FixedUpdate()
        {
            if (IsDashing)
            {
                if (_dashDirection.magnitude == 0f) // If dash direction is 0 we are going to the direction the character is looking at
                {
                    _rb.velocity = (_sr.flipX ? Vector2.left : Vector2.right) * _dashSpeed;
                }
                else
                {
                    _rb.velocity = _dashDirection * _dashSpeed;
                }
            }
            else
            {
                _rb.velocity = new Vector2(_movement, _rb.velocity.y);
                if (!_isGrounded && _isAgainstLeftWall != null)
                {
                    _sr.flipX = _isAgainstLeftWall == false;
                }
                else if (_movement != 0)
                {
                    _sr.flipX = _movement < 0f;
                }
            }
            if (_jump)
            {
                _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                _isGrounded = false;
                _jump = false;
                _anim.SetBool("IsJumping", true);
            }
        }

        private void Update()
        {
            if (IsDashing)
            {
                _dashTimer -= Time.deltaTime;
                if (!IsDashing) // We are not dashing anymore, remove dashing animation
                {
                    _anim.SetBool("IsDashing", false);
                    _rb.gravityScale = _baseGravityScale;
                }
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.action.phase == InputActionPhase.Started
                && !IsDashing) // Can't dash while we are already dashing
            {
                _dashTimer = _dashTimerRef; // Dashing
                _anim.SetBool("IsDashing", true);
                _rb.gravityScale = 0f; // We disable gravity while dashing
                _dashDirection = _lastDirection;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var mov = context.ReadValue<Vector2>();
            _lastDirection = mov.normalized;
            _movement = mov.x;
            if (_movement < 0f)
            {
                _anim.SetBool("IsRunning", true);
            }
            else if (_movement > 0f)
            {
                _anim.SetBool("IsRunning", true);
            }
            else
            {
                _anim.SetBool("IsRunning", false);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.action.phase != InputActionPhase.Started
                || IsDashing) // Can't jump if we are dashing
            {
                return;
            }
            if (_isGrounded)
            {
                _jump = true;
            }
        }
    }
}