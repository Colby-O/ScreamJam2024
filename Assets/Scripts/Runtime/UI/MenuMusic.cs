using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
	public class MenuMusic : MonoBehaviour
	{
		private AudioSource _musicStartAudioSource;
		private AudioSource _musicLoopAudioSource;
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
			active.Stop();
			active.volume = startVolume;
		}

		public void Restart()
		{
			_musicStartAudioSource.Stop();
			_musicLoopAudioSource.Stop();
			_musicStartAudioSource.Play();
			_musicLoopAudioSource.PlayDelayed(_musicStartAudioSource.clip.length);
		}

		public void FadeOut()
		{
			StartCoroutine(DoFadeOut());
		}

	}
}
