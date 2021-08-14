using UnityEngine;
using UnityEngine.InputSystem;

namespace Pie
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private float _movement;

        private const float _distanceWithGround = .1f; // Used for jump raycast
        private const float _jumpForce = 3.5f;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_movement, _rb.velocity.y);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>().x;
            if (_movement < 0f)
            {
                _sr.flipX = true;
            }
            else if (_movement > 0f)
            {
                _sr.flipX = false;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.action.phase != InputActionPhase.Started)
            {
                return;
            }
            var hit = Physics2D.Raycast(transform.position, Vector2.down, _distanceWithGround, LayerMask.GetMask("Wall"));
            if (hit.collider != null)
            {
                _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}