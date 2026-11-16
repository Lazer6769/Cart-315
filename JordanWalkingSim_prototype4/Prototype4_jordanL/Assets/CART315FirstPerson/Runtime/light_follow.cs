using System.Collections.Generic;
using UnityEngine;

namespace Cart315.FirstPerson
{
    [RequireComponent(typeof(BoxCollider))]
    public class light_follow : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform spotlight;

        private BoxCollider lightBound;
        private readonly HashSet<Collider> playerColliders = new HashSet<Collider>();
        private Vector3 followOffset;

        private void Awake()
        {
            lightBound = GetComponent<BoxCollider>();

            if (player == null)
                Debug.LogError("Assign the player Transform to Light Follow.", this);

            if (spotlight == null)
                Debug.LogError("Assign the spotlight Transform to Light Follow.", this);

            if (lightBound == null)
                Debug.LogError("Assign the light_bound BoxCollider to Light Follow.", this);
            else if (!lightBound.isTrigger)
                Debug.LogError("The light_bound BoxCollider must have Is Trigger enabled.", this);
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
            {
                playerColliders.Remove(other);

                if (playerColliders.Count == 0)
                    KeepSpotlightInsideBounds();
            }
        }

        private void LateUpdate()
        {
            if (playerColliders.Count > 0)
            {
                spotlight.position = lightBound.ClosestPoint(player.position + followOffset);
            }
        }

        private void OnDisable()
        {
            playerColliders.Clear();
        }

        private bool IsPlayerCollider(Collider other)
        {
            return player != null
                && lightBound != null
                && spotlight != null
                && (other.transform == player || other.transform.IsChildOf(player));
        }

        private void KeepSpotlightInsideBounds()
        {
            if (spotlight != null && lightBound != null)
                spotlight.position = lightBound.ClosestPoint(spotlight.position);
        }
    }
}
