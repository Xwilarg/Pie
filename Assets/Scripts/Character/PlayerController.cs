using UnityEngine;
using UnityEngine.InputSystem;

namespace Pie
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(Input.GetAxis("Horizontal"), _rb.velocity.y);
        }
    }
}