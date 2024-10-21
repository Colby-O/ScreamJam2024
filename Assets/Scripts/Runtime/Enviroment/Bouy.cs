using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class Bouy : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _head;

        [Header("settings")]
        [SerializeField] private float _rotationSpeed = 0.01f;

        private void Update()
        {
            Vector3 rot = _head.transform.rotation.eulerAngles;
            _head.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z + _rotationSpeed * Time.deltaTime);
        }
    }
}
