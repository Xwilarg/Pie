using UnityEngine;
using UnityEngine.InputSystem;

namespace Pie
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private float _movement;

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

        public void OnJump(InputAction.CallbackContext _)
        {
            _rb.AddForce(Vector2.up, ForceMode2D.Impulse);
        }
    }
}