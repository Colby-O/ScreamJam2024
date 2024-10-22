using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Radar
{
    public class Radar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _scanSpeed;
        [SerializeField] private float _scanCooldown;
        [SerializeField] private float _scanDistance;
        [SerializeField, Range(0f, 180f)] private float _scanRadius;
        [SerializeField] private LayerMask _scanableLayer;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private RadarIcon _prefabObs;
        [SerializeField] private RadarIcon _prefabScan;
        [SerializeField] bool _updatePosEveryFrame = true;
        [SerializeField] Vector2 _beepIntervals;
        [SerializeField] Scanline _scanlinePrefab;

        [SerializeField, ReadOnly] private bool _isCoolingDown = false;
        [SerializeField, ReadOnly] private float _cooldown;
        [SerializeField, ReadOnly] private float _rot = 0;
        [SerializeField, ReadOnly] private Vector3 scanPos;

        [SerializeField, ReadOnly] private float _currentBeepInterval;
        [SerializeField, ReadOnly] private float _beepTime;

        private Scanline _lastScanline;

        private void CheckScanline()
        {
            if (Mathf.Abs(Vector3.Angle(-transform.forward, Vector3.forward)) > _scanRadius) return;

            if (_isCoolingDown) return;

            _lastScanline.SetPosition(scanPos, 0);
            _lastScanline.SetPosition(scanPos + (Quaternion.AngleAxis(_rot, Vector3.up) * Vector3.forward * _scanDistance), 1);

            if (Physics.BoxCast(scanPos, new Vector3(1f, 1f, 1f), Quaternion.AngleAxis(_rot, Vector3.up) * Vector3.forward, out RaycastHit hit, Quaternion.identity, _scanDistance))
            {
                if ((_obstacleLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    // Hit Obstacle
                    Debug.DrawLine(scanPos, hit.point, Color.magenta, 1);

                    RadarIcon icon = GameObject.Instantiate(_prefabObs, hit.point, Quaternion.identity);
                    icon.transform.parent = hit.transform;
                    icon.SetDestoryState(true);
                    icon.Ping();

                }
            }

            if (Physics.BoxCast(scanPos, new Vector3(1f, 1000f, 1f), Quaternion.AngleAxis(_rot, Vector3.up) * Vector3.forward, out hit, Quaternion.identity, _scanDistance, _scanableLayer.value))
            {
                // Hit Scannable
                Debug.DrawLine(scanPos, hit.point, Color.red, 1);

                _currentBeepInterval = Mathf.Lerp(_beepIntervals.x, _beepIntervals.y, hit.distance / _scanDistance);

                RadarIcon icon = hit.collider.GetComponent<RadarIcon>();

                if (icon == null) 
                {
                    icon = GameObject.Instantiate(_prefabScan, hit.point, Quaternion.identity);
                    icon.transform.parent = hit.transform;
                    icon.SetDestoryState(true);
                }
                icon.Ping();
            }
        }

        private void RotateScanline()
        {
            if (_updatePosEveryFrame) scanPos = transform.position;

            _currentBeepInterval = Mathf.Infinity;
            _lastScanline.SetPosition(Vector3.zero, 0);
            _lastScanline.SetPosition(Vector3.zero, 1);

            if (_isCoolingDown)
            {
                _cooldown += Time.deltaTime;
                _isCoolingDown =  _cooldown < _scanCooldown;
            }
            else
            {
                _rot += _scanSpeed * Time.deltaTime;

                if (_rot > 360f)
                {
                    _rot = 0;
                    _isCoolingDown = true;
                    _cooldown = 0;
                    scanPos = transform.position;
                }
            }
        }

        private void Awake()
        {
            scanPos = transform.position;
            _lastScanline = Instantiate(_scanlinePrefab, transform.position, Quaternion.identity, transform);
        }

        private void Update()
        {
            RotateScanline();
            CheckScanline();

            _beepTime += Time.deltaTime;

            if (_currentBeepInterval < _beepTime)
            {
                _beepTime = 0;
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("Beep", PlazmaGames.Audio.AudioType.Sfx, false, true);
            }
        }
    }
}
