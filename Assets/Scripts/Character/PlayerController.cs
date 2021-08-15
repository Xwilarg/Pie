using UnityEngine;
using UnityEngine.InputSystem;

namespace Pie
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Animator _anim;
        private float _movement;
        private float _baseGravityScale;

        // Jump information
        private const float _distanceWithGround = .1f; // Used for jump raycast
        private const float _jumpForce = 3.5f;
        private bool _jump;
        private bool _isGrounded;

        // Dash information
        private Vector2 _dashDirection;
        private float _dashTimer;
        private const float _dashTimerRef = .5f; // Duration of the dash in seconds
        private const float _dashSpeed = 3f;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();

            _baseGravityScale = _rb.gravityScale;
        }

        private void FixedUpdate()
        {
            if (_dashTimer > 0f)
            {
                _rb.velocity = _dashDirection * _dashSpeed;
            }
            else
            {
                _rb.velocity = new Vector2(_movement, _rb.velocity.y);
            }
            if (_jump)
            {
                _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                _isGrounded = false;
                _jump = false;
                _anim.SetBool("IsJumping", true);
            }
            else if (!_isGrounded)
            {
                _isGrounded = CanJump();
                if (_isGrounded)
                {
                    _anim.SetBool("IsJumping", false);
                }
            }
        }

        private void Update()
        {
            if (_dashTimer > 0f)
            {
                _dashTimer -= Time.deltaTime;
                if (_dashTimer <= 0f) // We are not dashing anymore, remove dashing animation
                {
                    _anim.SetBool("IsDashing", false);
                    _rb.gravityScale = _baseGravityScale;
                }
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.action.phase == InputActionPhase.Started
                && _dashTimer <= 0f) // Can't dash while we are already dashing
            {
                _dashTimer = _dashTimerRef; // Dashing
                _anim.SetBool("IsDashing", true);
                _rb.gravityScale = 0f; // We disable gravity while dashing
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var mov = context.ReadValue<Vector2>();
            // If we are not dashing we set the dashing direction to the current one
            if (_dashTimer <= 0f && context.action.phase == InputActionPhase.Started)
            {
                _dashDirection = mov.normalized;
            }
            _movement = mov.x;
            if (_movement < 0f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                _anim.SetBool("IsRunning", true);
            }
            else if (_movement > 0f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
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
                || _dashTimer >= 0f) // Can't jump if we are dashing
            {
                return;
            }
            if (_isGrounded)
            {
                _jump = true;
            }
        }

        private bool CanJump()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, _distanceWithGround, LayerMask.GetMask("Wall"));
            return hit.collider != null;
        }
    }
}