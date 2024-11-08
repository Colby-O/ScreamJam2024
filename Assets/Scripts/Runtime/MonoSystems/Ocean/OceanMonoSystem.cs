using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Windows;
using PlazmaGames.Attribute;
using UnityEngine.InputSystem;

namespace BeneathTheSurface.MonoSystems
{
    public class OceanMonoSystem : MonoBehaviour, IOceanMonoSystem
    {
        [Header("References")]
        [SerializeField] private Material _oceanMaterial;

        [Header("Wave Vectors")]
        [SerializeField] private Vector4 _waveA;
        [SerializeField] private Vector4 _waveB;
        [SerializeField] private Vector4 _waveC;
        [SerializeField] private Vector4 _waveD;
        [SerializeField] private float _speedA;
        [SerializeField] private float _speedB;
        [SerializeField] private float _speedC;
        [SerializeField] private float _speedD;
        [Header("Settings")]
        [SerializeField] private float _seaLevel = 0.0f;

        [SerializeField, ReadOnly] private float _time;

        Vector3 GerstnerWave(Vector4 wave, float speedScale, Vector3 pt)
        {
            float smoothness = wave.z;
            float wavelength = wave.w;
            float k = 2 * Mathf.PI / wavelength;
            float speed = speedScale * Mathf.Sqrt(9.81f / k);
            Vector2 dir = (new Vector2(wave.x, wave.y)).normalized;
            float f = k * (Vector2.Dot(dir, new Vector2(pt.x, pt.z)) - speed * _time);

            float factor = smoothness / k;//Mathf.Exp(-k / smoothness) / k;

            return (new Vector3(dir.x * Mathf.Cos(f), Mathf.Sin(f), dir.y * Mathf.Cos(f))) * factor;
        }

        public float GetSeaLevel()
        {
            return _seaLevel;
        }

        public Vector3 CalculateWaveAt(Vector2 position)
        {
            Vector3 wavePosition = new Vector3(position.x, _seaLevel, position.y);
            Vector3 pt = wavePosition;
            wavePosition += GerstnerWave(_waveA, _speedA, pt);
            wavePosition += GerstnerWave(_waveB, _speedB, pt);
            wavePosition += GerstnerWave(_waveC, _speedC, pt);
            wavePosition += GerstnerWave(_waveD, _speedD, pt);

            return wavePosition;
        }

        private void Start()
        {
            _time = 0;
            _oceanMaterial.SetVector("_WaveA", _waveA);
            _oceanMaterial.SetVector("_WaveB", _waveB);
            _oceanMaterial.SetVector("_WaveC", _waveC);
            _oceanMaterial.SetVector("_WaveD", _waveD);
            _oceanMaterial.SetFloat("_SpeedScaleA", _speedA);
            _oceanMaterial.SetFloat("_SpeedScaleB", _speedB);
            _oceanMaterial.SetFloat("_SpeedScaleC", _speedC);
            _oceanMaterial.SetFloat("_SpeedScaleD", _speedD);
            _oceanMaterial.SetFloat("_PhysicsTime", _time);
            _oceanMaterial.SetInt("_UsePhysicsTime", 1);
        }

        private void Update()
        {
            _time += Time.deltaTime;
            _oceanMaterial.SetFloat("_PhysicsTime", _time);
        }
    }
}
