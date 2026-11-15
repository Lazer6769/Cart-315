using System.Collections.Generic;
using UnityEngine;

namespace Cart315.FirstPerson
{
    [RequireComponent(typeof(Collider))]
    public class light_follow : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform spotlight;

        private readonly HashSet<Collider> playerColliders = new HashSet<Collider>();
        private Vector3 followOffset;

        private void Awake()
        {
            if (player == null)
                Debug.LogError("Assign the player Transform to Light Follow.", this);

            if (spotlight == null)
                Debug.LogError("Assign the spotlight Transform to Light Follow.", this);

            if (!GetComponent<Collider>().isTrigger)
                Debug.LogError("The maze-room Collider must have Is Trigger enabled.", this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsPlayerCollider(other))
                return;

            if (playerColliders.Count == 0)
                followOffset = spotlight.position - player.position;

            playerColliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsPlayerCollider(other))
                playerColliders.Remove(other);
        }

        private void LateUpdate()
        {
            if (playerColliders.Count > 0)
                spotlight.position = player.position + followOffset;
        }

        private void OnDisable()
        {
            playerColliders.Clear();
        }

        private bool IsPlayerCollider(Collider other)
        {
            return player != null
                && spotlight != null
                && (other.transform == player || other.transform.IsChildOf(player));
        }
    }
}
