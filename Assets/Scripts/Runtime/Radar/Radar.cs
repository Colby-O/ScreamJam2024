using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Radar
{
    public class Radar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _scanSpeed;
        [SerializeField] private float _scanDistance;
        [SerializeField, Range(0f, 180f)] private float _scanRadius;
        [SerializeField] private LayerMask _scanableLayer;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private RadarIcon _prefab;
        private void CheckScanline()
        {
            if (Mathf.Abs(Vector3.Angle(transform.forward, Vector3.forward)) > _scanRadius) return;

            if (Physics.BoxCast(transform.position, new Vector3(1f, 1000f, 1f), transform.forward, out RaycastHit hit, Quaternion.identity, _scanDistance))
            {
                if ((_scanableLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    // Hit Scannable
                    Debug.DrawLine(transform.position, hit.point, Color.red, 1);

                    RadarIcon icon = hit.collider.GetComponent<RadarIcon>();
                    icon.Ping();
                }
                else if ((_obstacleLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    // Hit Obstacle
                    Debug.DrawLine(transform.position, hit.point, Color.magenta, 1);

                    RadarIcon icon = GameObject.Instantiate(_prefab, hit.point, Quaternion.identity);
                    icon.transform.parent = hit.transform;
                    icon.SetDestoryState(true);
                    icon.Ping();
                }
                else
                {
                    // Hit Unknown
                    Debug.DrawLine(transform.position, hit.point, Color.blue, 1);
                }
            }
            else
            {
                // Hit Nothing
                Debug.DrawLine(transform.position, transform.position + transform.forward * _scanDistance, Color.green, 1);
            }
        }

        private void RotateScanline()
        {
            transform.RotateAround(transform.position, Vector3.up, _scanSpeed * Time.deltaTime);
        }

        private void Update()
        {
            CheckScanline();
            RotateScanline();
        }
    }
}
