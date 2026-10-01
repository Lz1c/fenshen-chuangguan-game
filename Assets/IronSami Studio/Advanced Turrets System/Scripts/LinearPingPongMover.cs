using UnityEngine;

namespace IronSamiStudio.AdvancedTurretAI
{
    /// <summary>
    /// Moves the GameObject back-and-forth along a straight line (local or world space).
    /// Great for demoing turret tracking / lead-prediction.
    /// </summary>
    [RequireComponent(typeof(Collider))]      // keeps the enemy trigger so turrets can detect it
    public class LinearPingPongMover : MonoBehaviour
    {
        [Header("Movement Path")]
        [Tooltip("Direction of travel. (1,0,0) = +X / -X, (0,0,1) = +Z / -Z, etc.")]
        public Vector3 moveDirection = Vector3.right;     // left <-----------> right

        [Tooltip("Total distance covered (in units).")]
        [Min(0f)] public float travelDistance = 5f;

        [Header("Timing")]
        [Tooltip("Cycles per second (higher = faster).")]
        [Min(0f)] public float speed = 1f;

        [Tooltip("Start at a random point in the cycle so multiple enemies desync.")]
        public bool randomizeStartPhase = false;

        // ──────────────────────────────────────────────────────────────────────────────
        Vector3 _startPos;
        float   _phaseOffset = 0f;

        void Awake()
        {
            _startPos = transform.position;

            // Normalise direction so magnitude only comes from travelDistance
            if (moveDirection == Vector3.zero) moveDirection = Vector3.right;
            moveDirection = moveDirection.normalized;

            if (randomizeStartPhase)
                _phaseOffset = Random.value;   // 0-1 fraction of the loop
        }

        void Update()
        {
            // Ping-pong gives a saw-wave 0 → travelDistance → 0 … we center it on the
            // starting position so the object never drifts.
            float t   = (Time.time * speed + _phaseOffset) % 1f;         // 0-1 loop
            float pos = Mathf.PingPong(t * 2f, 1f) * travelDistance;     // 0-d

            // Center around zero (-d/2 … +d/2) then convert to world position
            Vector3 offset = moveDirection * (pos - travelDistance * 0.5f);
            transform.position = _startPos + offset;
        }

#if UNITY_EDITOR
        // Optional gizmo so you can see the path in Scene view
        void OnDrawGizmosSelected()
        {
            if (moveDirection == Vector3.zero) return;

            Vector3 dir = moveDirection.normalized * travelDistance * 0.5f;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position - dir, transform.position + dir);
            Gizmos.DrawSphere(transform.position - dir, 0.1f);
            Gizmos.DrawSphere(transform.position + dir, 0.1f);
        }
#endif
    }
}
