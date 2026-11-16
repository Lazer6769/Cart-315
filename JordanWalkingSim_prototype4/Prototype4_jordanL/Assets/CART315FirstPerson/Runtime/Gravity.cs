using UnityEngine;
using System.Collections.Generic;

namespace Cart315.FirstPerson
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class Gravity : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float lunarGravity = 1.62f;

        private readonly HashSet<Collider> playerColliders = new HashSet<Collider>();
        private BoxCollider gravityZone;
        private FPController player;
        private float originalGravityMultiplier;
        private float lunarGravityMultiplier;

        private void Awake()
        {
            gravityZone = GetComponent<BoxCollider>();
            gravityZone.isTrigger = true;

            Rigidbody body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            float normalGravity = Mathf.Abs(Physics.gravity.y);
            if (normalGravity <= Mathf.Epsilon)
            {
                Debug.LogError("Gravity zone requires non-zero world gravity on the Y axis.", this);
                enabled = false;
                return;
            }

            lunarGravityMultiplier = lunarGravity / normalGravity;
        }

        private void OnTriggerEnter(Collider other)
        {
            FPController enteringPlayer = other.GetComponentInParent<FPController>();
            if (enteringPlayer == null || (player != null && player != enteringPlayer))
                return;

            if (playerColliders.Count == 0)
            {
                player = enteringPlayer;
                originalGravityMultiplier = player.GravityMultiplier;
                player.SetGravityMultiplier(lunarGravityMultiplier);
            }

            if (player == enteringPlayer)
                playerColliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            FPController exitingPlayer = other.GetComponentInParent<FPController>();
            if (exitingPlayer != player || !playerColliders.Remove(other))
                return;

            if (playerColliders.Count == 0)
                RestorePlayerGravity();
        }

        private void OnDisable()
        {
            RestorePlayerGravity();
        }

        private void RestorePlayerGravity()
        {
            if (player != null)
                player.SetGravityMultiplier(originalGravityMultiplier);

            player = null;
            playerColliders.Clear();
        }
    }
}
