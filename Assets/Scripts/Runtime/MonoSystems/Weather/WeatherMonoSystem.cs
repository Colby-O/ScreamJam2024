using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public class WeatherMonoSystem : MonoBehaviour, IWeatherMonoSystem
    {
        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem _lighting;
        [SerializeField] private ParticleSystem _rain;

        [Header("Lighting Settings")]
        [SerializeField] private Vector2 _lightingInterval;
        [SerializeField] private Vector2 _thunderDelay;
        [SerializeField] private List<AudioClip> _thunderClips;

        [Header("Rain Settiings")]
        [SerializeField] AudioClip _rainOutdoorClip;
        [SerializeField] AudioClip _rainIndoorClip;

        private float _timeToNextLighting = 0;
        private float _timeToNextThunder = 0;

        private float _timerLighting = 0;
        private float _timerThunder = 0;

        private bool _thunderAwiting = false;

        private bool _isStormy = true;

        public void SetWeatherState(bool isStormy, bool isIndoors)
        {
            _isStormy = isStormy;
            GameManager.GetMonoSystem<IAudioMonoSystem>().StopAudio(PlazmaGames.Audio.AudioType.Sfx);
            if (isStormy && isIndoors)
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_rainIndoorClip, PlazmaGames.Audio.AudioType.Sfx, true, true);
            }
            else if (isStormy && !isIndoors) 
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_rainOutdoorClip, PlazmaGames.Audio.AudioType.Sfx, true, true);
            }
        }

        private void Awake()
        {
            _timeToNextLighting = Random.Range(_lightingInterval.x, _lightingInterval.y);
            _timeToNextThunder = Random.Range(_thunderDelay.x, _thunderDelay.y);
            _timerLighting = 0;
            _timerThunder = 0;
            _thunderAwiting = false;
            SetWeatherState(true, false);

        }

        private void Start()
        {
            _lighting = GameObject.FindWithTag("LightingSystem").GetComponent<ParticleSystem>();
            _rain = GameObject.FindWithTag("RainSystem").GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            _lighting.gameObject.SetActive(_isStormy);
            _rain.gameObject.SetActive(_isStormy);

            if (_isStormy)
            {
                _timerLighting += Time.deltaTime;
                if (_thunderAwiting) _timerThunder += Time.time;

                if (_timerThunder > _timeToNextThunder && _thunderAwiting)
                {
                    if (_thunderClips != null && _thunderClips.Count > 0) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_thunderClips[Random.Range(0, _thunderClips.Count)], PlazmaGames.Audio.AudioType.Sfx, false, true);
                    _thunderAwiting = false;
                    _timeToNextThunder = Random.Range(_thunderDelay.x, _thunderDelay.y);
                }

                if (_timerLighting > _timeToNextLighting)
                {
                    _lighting.Emit(1);
                    _timerLighting = 0;
                    _timerThunder = 0;
                    _thunderAwiting = true;
                    _timeToNextLighting = Random.Range(_lightingInterval.x, _lightingInterval.y);
                }
            }
        }
    }
}
