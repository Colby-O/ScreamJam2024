using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class RigLight : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _head;

        [Header("settings")]
        [SerializeField] private float _rotationSpeed = 0.01f;

        private void Update()
        {
            Vector3 rot = _head.transform.rotation.eulerAngles;
            _head.transform.rotation = Quaternion.Euler(rot.x, rot.y + _rotationSpeed * Time.deltaTime, rot.z);
        }
    }
}
