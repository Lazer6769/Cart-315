using UnityEngine;

namespace Cart315.FirstPerson
{
    public class light_follow : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform spotlight;
        [SerializeField] private BoxCollider lightBound;

        private CharacterController playerController;
        private bool playerInsideBounds;

        private void Awake()
        {
            if (player == null || spotlight == null || lightBound == null)
            {
                Debug.LogError("Assign the player, spotlight, and room BoxCollider to Light Follow.", this);
                enabled = false;
                return;
            }

            playerController = player.GetComponent<CharacterController>();
            if (playerController == null)
            {
                Debug.LogError("The assigned player must have a CharacterController.", this);
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            bool isPlayerInside = lightBound.bounds.Intersects(playerController.bounds);

            if (isPlayerInside)
            {
                Vector3 overheadPosition = player.position;
                overheadPosition.y = playerController.bounds.max.y + 0.25f;
                spotlight.position = lightBound.ClosestPoint(overheadPosition);
            }
            else if (playerInsideBounds)
            {
                spotlight.position = lightBound.ClosestPoint(spotlight.position);
            }

            playerInsideBounds = isPlayerInside;
        }
    }
}
