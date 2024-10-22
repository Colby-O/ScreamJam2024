using PlazmaGames.Attribute;
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
        [SerializeField] private RadarIcon _prefab;

        [SerializeField, ReadOnly] private bool _isCoolingDown = false;
        [SerializeField, ReadOnly] private float _cooldown;
        [SerializeField, ReadOnly] private float _rot = 0;
        [SerializeField, ReadOnly] private Vector3 scanPos;

        private void CheckScanline()
        {
            if (Mathf.Abs(Vector3.Angle(-transform.forward, Vector3.forward)) > _scanRadius) return;

            if (Physics.BoxCast(scanPos, new Vector3(1f, 5f, 1f), Quaternion.AngleAxis(_rot, Vector3.up) * Vector3.forward, out RaycastHit hit, Quaternion.identity, _scanDistance))
            {
                if ((_obstacleLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    // Hit Obstacle
                    Debug.DrawLine(scanPos, hit.point, Color.magenta, 1);

                    RadarIcon icon = GameObject.Instantiate(_prefab, hit.point, Quaternion.identity);
                    icon.transform.parent = hit.transform;
                    icon.SetDestoryState(true);
                    icon.Ping();
                }
            }

            if (Physics.BoxCast(scanPos, new Vector3(1f, 1000f, 1f), Quaternion.AngleAxis(_rot, Vector3.up) * Vector3.forward, out hit, Quaternion.identity, _scanDistance))
            {
                if ((_scanableLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    // Hit Scannable
                    Debug.DrawLine(scanPos, hit.point, Color.red, 1);

                    RadarIcon icon = hit.collider.GetComponent<RadarIcon>();
                    icon.Ping();
                }
            }
        }

        private void RotateScanline()
        {
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
        }

        private void Update()
        {
            CheckScanline();
            RotateScanline();
        }
    }
}
