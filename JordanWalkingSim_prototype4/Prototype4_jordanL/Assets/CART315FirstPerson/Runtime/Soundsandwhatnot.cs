using System.Collections.Generic;
using UnityEngine;

namespace Cart315.FirstPerson
{
    [RequireComponent(typeof(AudioSource))]
    public class Soundsandwhatnot : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _sounds = new List<AudioClip>();
        [SerializeField, Min(0.1f)] private float _intervalSeconds = 15f;

        public AudioSource _audioSource;

        public List<AudioClip> Sounds => _sounds;
        private float _timeUntilNextSound;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        private void Update()
        {
            if (_audioSource.isPlaying) return;

            _timeUntilNextSound += Time.deltaTime;
            if (_timeUntilNextSound < _intervalSeconds) return;

            if (PlayRandomSound())
                _timeUntilNextSound = 0f;
        }

        private bool PlayRandomSound()
        {
            int availableSounds = 0;
            foreach (AudioClip sound in _sounds)
            {
                if (sound != null) availableSounds++;
            }

            if (availableSounds == 0) return false;

            int selectedSound = Random.Range(0, availableSounds);
            foreach (AudioClip sound in _sounds)
            {
                if (sound == null) continue;
                if (selectedSound-- == 0)
                {
                    _audioSource.PlayOneShot(sound);
                    return true;
                }
            }

            return false;
        }
    }
}
