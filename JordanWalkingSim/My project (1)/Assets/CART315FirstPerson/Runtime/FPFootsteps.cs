using System;
using UnityEngine;

namespace Cart315.FirstPerson
{
    [RequireComponent(typeof(FPController), typeof(AudioSource))]
    public class FPFootsteps : MonoBehaviour
    {
        [SerializeField] private AudioClip _leftFootstep;
        [SerializeField] private AudioClip _rightFootstep;
        [Tooltip("Horizontal metres travelled between steps; sprinting produces faster steps.")]
        [SerializeField, Min(0.1f)] private float _stepDistance = 1.8f;
        [SerializeField, Range(0, 1)] private float _volume = 0.35f;

        public event Action FootstepPlayed;
        private FPController _player;
        private AudioSource _audioSource;
        private float _distance;
        private bool _nextStepIsLeft = true;

        private void Awake()
        {
            _player = GetComponent<FPController>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
        }

        private void LateUpdate()
        {
            if (!_player.isActiveAndEnabled || !_player.IsGrounded || !_player.IsInputMoving)
            {
                _distance = 0;
                return;
            }
            _distance += _player.GroundDistanceThisFrame;
            if (_distance < Mathf.Max(0.1f, _stepDistance)) return;
            _distance %= Mathf.Max(0.1f, _stepDistance);

            AudioClip footstep = _nextStepIsLeft ? _leftFootstep : _rightFootstep;
            _nextStepIsLeft = !_nextStepIsLeft;
            if (footstep == null) return;

            _audioSource.PlayOneShot(footstep, _volume);
            FootstepPlayed?.Invoke();
        }

        private void OnDisable() => _distance = 0;
    }
}
