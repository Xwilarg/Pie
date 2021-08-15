using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Pie.Character
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerDetector : MonoBehaviour
    {
        private readonly List<Collider2D> _colliders = new();

        public UnityEvent TriggerOn { get; } = new();
        public UnityEvent TriggerOff { get; } = new();

        public string Tag { set; private get; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Tag == null || collision.CompareTag(Tag))
            {
                _colliders.Add(collision);
                TriggerOn?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _colliders.Remove(collision);
            if (!_colliders.Any())
            {
                TriggerOff?.Invoke();
            }
        }
    }
}
