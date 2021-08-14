using UnityEngine;
using UnityEngine.InputSystem;

namespace Pie
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Animator _anim;
        private float _movement;

        private const float _distanceWithGround = .1f; // Used for jump raycast
        private const float _jumpForce = 3.5f;

        private bool _jump;
        private bool _isGrounded;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_movement, _rb.velocity.y);
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

        public void OnMove(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>().x;
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