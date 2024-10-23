using BeneathTheSurface.Player;
using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace BeneathTheSurface.Wielding
{
    public class Wielder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _head;
        [SerializeField] private ToolController _tools;

        [Header("Sparks")]
        [SerializeField] private ParticleSystem _sparks;
        [SerializeField] private Light _sparkLight;
        [SerializeField] private int sparkEmissionRate = 200;

        [Header("Decal")]
        [SerializeField] private GameObject _decalPrefab;

        [Header("Wielding")]
        [SerializeField] private float _wieldingDist = 3f;
        [SerializeField, ReadOnly] private bool _wielderEnabled = false;

        [Header("Audio")]
        [SerializeField] private AudioSource _as;
        [SerializeField] private AudioClip _onClip;
        [SerializeField] private AudioClip _usingClip;
        [SerializeField] private AudioClip _offClip;

        private Pipe _lastPipe = null;

        private bool _isOn = false;

        private void CheckForHit()
        {
            bool wasOn = _isOn;
            _isOn = false;
            Debug.DrawRay(_head.position, _head.forward, Color.white, 1f);
            if (Physics.Raycast(_head.position, _head.forward, out RaycastHit hit, _wieldingDist))
            {
                Pipe pipe = hit.collider.GetComponent<Pipe>();
                if (pipe != null)
                {
                    if (!wasOn)
                    {
                        _as.PlayOneShot(_onClip);
                    }
                    else if (!_as.isPlaying)
                    {
                        _as.clip = _usingClip;
                        _as.loop = true;
                        _as.Play();
                    }
                    _isOn = true;
                    FindAnyObjectByType<SquidAi>().ApplyNoise((1f / 15f) * Time.deltaTime);
                    if (_lastPipe != null && _lastPipe != pipe) _lastPipe.SetWieldingState(false);
                    pipe.SetWieldingState(true);
                    _lastPipe = pipe;
                    GameObject decal = GameObject.Instantiate(_decalPrefab);
                    decal.transform.position = hit.point + -_head.forward * 0.1f;
                    decal.transform.forward = _head.forward;
                    decal.transform.parent = hit.transform;

                    _sparkLight.gameObject.SetActive(true);
                    _sparks.transform.parent.position = hit.point + -_head.forward * 0.02f;
                    var sparkEmission = _sparks.emission;
                    sparkEmission.rateOverTime = sparkEmissionRate;
                }
            }

            if (wasOn && !_isOn)
            {
                _as.Stop();
                _as.PlayOneShot(_offClip);

                if (_lastPipe != null) _lastPipe.SetWieldingState(false);
                var sparkEmission = _sparks.emission;
                sparkEmission.rateOverTime = 0;
                _sparkLight.gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            if (_tools == null) _tools = GetComponent<ToolController>();
        }

        private void Update()
        {
            _wielderEnabled = Mouse.current.leftButton.isPressed && _tools.CurrentTool() == Tool.Welder;
            if (!_wielderEnabled)
            {
                if (_isOn)
                {
                    _as.Stop();
                    _as.PlayOneShot(_offClip);
                    _isOn = false;
                }

                if (_lastPipe != null) _lastPipe.SetWieldingState(false);
                var sparkEmission = _sparks.emission;
                sparkEmission.rateOverTime = 0;
                _sparkLight.gameObject.SetActive(false);
                return;
            }

            CheckForHit();
        }
    }
}
