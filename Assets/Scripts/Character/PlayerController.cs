using UnityEngine;
using UnityEngine.InputSystem;

namespace Pie
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Animator _anim;
        private float _movement;

        // Jump information
        private const float _distanceWithGround = .1f; // Used for jump raycast
        private const float _jumpForce = 3.5f;
        private bool _jump;
        private bool _isGrounded;

        // Dash information
        private bool _wasGoingLeft;
        private bool _isDashingLeft;
        private float _dashTimer;
        private const float _dashTimerRef = .5f; // Duration of the dash in seconds
        private const float _dashSpeed = 3f;
        private float _doubleTapDelay; // Delay between 2 press to double tap
        private const float _doubleTapDelayRef = .5f; // Nb of seconds we have to double tap

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            if (_dashTimer > 0f)
            {
                _rb.velocity = new Vector2(_dashSpeed * (_isDashingLeft ? -1f : 1f), _rb.velocity.y);
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
                }
            }
            if (_doubleTapDelay > 0f)
            {
                _doubleTapDelay -= Time.deltaTime;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>().x;

            // We only check for dash when we are pressing the button
            if (context.action.phase == InputActionPhase.Started
                && _dashTimer <= 0f) // Can't dash while we are already dashing
            {
                // Dash
                if (_doubleTapDelay > 0f // Can we still double tap
                    && ((_wasGoingLeft && _movement < 0f) || (!_wasGoingLeft && _movement > 0f))) // Are we going in same direction as previously
                {
                    _dashTimer = _dashTimerRef; // Dashing
                    _doubleTapDelay = 0f; // Reset delay
                    _isDashingLeft = _movement < 0f; // Set dashing direction (used for movements)
                    _anim.SetBool("IsDashing", true);
                }
                else if (_movement != 0f)
                {
                    _doubleTapDelay = _doubleTapDelayRef; // Reset double tap delay since we pressed a key
                    _wasGoingLeft = _movement < 0f; // Old direction we were going to
                }
            }


            // Movement
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
            if (context.action.phase != InputActionPhase.Started)
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