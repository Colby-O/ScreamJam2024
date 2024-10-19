using BeneathTheSurface.MonoSystems;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.GamePhysics
{
    public class Floater : MonoBehaviour
    {
        static private int _floaterCount = 0;

        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _depthBeforeSubmerged = 1f;
        [SerializeField] private float _displacementAmount = 3f;
        [SerializeField] private float _waterDrag = 0.99f;
        [SerializeField] private float _waterAngularDrag = 0.5f;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody>();
            _floaterCount++;
        }

        private void FixedUpdate()
        {
            _rb.AddForceAtPosition(Physics.gravity / _floaterCount, transform.position, ForceMode.Acceleration);
            float waveHeight = GameManager.GetMonoSystem<IOceanMonoSystem>().CalculateWaveAt(new Vector2(transform.position.x, transform.position.z)).y;
            if (transform.position.y < waveHeight)
            {
                float displacementMul = Mathf.Clamp01((waveHeight - transform.position.y) / _depthBeforeSubmerged) * _displacementAmount;
                _rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMul, 0f), transform.position, ForceMode.Acceleration);
                _rb.AddForce(displacementMul * -_rb.velocity * _waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                _rb.AddTorque(displacementMul * -_rb.angularVelocity * _waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }

        private void OnDestroy()
        {
            _floaterCount--;
        }
    }
}
