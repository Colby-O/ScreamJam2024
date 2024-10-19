using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class Radar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _scanSpeed;
        [SerializeField] private float _scanDistance;

        private void CheckScanline()
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _scanDistance))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red, 1);
            }
            else Debug.DrawLine(transform.position, transform.position + transform.forward * _scanDistance, Color.green, 1);
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
