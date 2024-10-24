using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class MenuMusic : MonoBehaviour
    {
        private AudioSource _musicStartAudioSource;
        private AudioSource _musicLoopAudioSource;
        private bool _firstPlay = true;
        private float _startTime;
        [SerializeField] private float _fadeOutTime = 2.0f;

        private void Start()
        {
            _musicStartAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
            _musicLoopAudioSource = transform.GetChild(1).GetComponent<AudioSource>();
            Restart();
        }

        private IEnumerator DoFadeOut()
        {
            AudioSource active = _musicStartAudioSource.isPlaying ? _musicStartAudioSource : _musicLoopAudioSource;
            float startVolume = active.volume;
            while (active.volume > 0)
            {
                active.volume -= startVolume * Time.deltaTime / _fadeOutTime;
                yield return null;
            }
            _musicStartAudioSource.Stop();
            _musicLoopAudioSource.Stop();
            active.volume = startVolume;
        }

        public void Restart()
        {
            _startTime = Time.time;
            _firstPlay = true;
            _musicStartAudioSource.Stop();
            _musicLoopAudioSource.Stop();
            _musicStartAudioSource.Play();
        }

        public void FadeOut()
        {
            StartCoroutine(DoFadeOut());
        }

        private void Update()
        {
            if (_firstPlay && Time.time - _startTime > _musicStartAudioSource.clip.length)
            {
                _musicLoopAudioSource.Play();
                _firstPlay = false;
            }
        }
    }
}