using System.Collections;
using System.Collections.Generic;
using BeneathTheSurface.Player;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.Serialization;

namespace BeneathTheSurface
{
    public class SquidAi : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 2.0f;
        [SerializeField] private float _maxMoveDistance = 40.0f;
        [SerializeField] private float _visionRange = 30.0f;
        [SerializeField] private float _visionThickness = 5.0f;
        [SerializeField] private float _heightLimit = 5.0f;
        [SerializeField] private float _lonelyTime = 10.0f;
        [SerializeField] private float _nonLonelyMoveTowardsPlayerChance = 0.1f;
        [SerializeField] private float _noisyNonLonelyMoveTowardsPlayerChance = 0.5f;
        [SerializeField] private float _lonelyMoveTowardsPlayerChance = 0.7f;
        [SerializeField] private float _playerGeneralArea = 60.0f;
        [SerializeField] private float _flairDuration = 4.0f;
        [SerializeField] private float _noiseDampingRate;
        [SerializeField] private float _noiseLevel = 0.0f;
        private float _flairTime = 0.0f;
        private float _lastWithPlayerTime = 0.0f;
        
        private bool IsLonely() => Time.time - _lastWithPlayerTime >= _lonelyTime;
        
        [SerializeField] Vector3 _target;
        private Vector3 _targetDirection;
        
        private Transform _player;
        [SerializeField] private int _maxRetries = 20;

        public void SendFlair()
        {
            _flairTime = Time.time;
        }

        public void ApplyNoise(float level)
        {
            _noiseLevel += level;
            if (_noiseLevel > 1.0f) _noiseLevel = 1.0f;
        }

        private void Start()
        {
            _player = GameObject.FindObjectOfType<PlayerController>().transform;
            _lastWithPlayerTime = Time.time;
            NextTarget();
        }

        private bool IsValidMove()
        {
            return (
                !Physics.SphereCast(transform.position, transform.localScale.x, _targetDirection, out var _, _maxMoveDistance) &&
                _target.y < _heightLimit
            );
        }

        private void RandomMove(float maxMoveDistance = 0, int trys = 0)
        {
            if (trys == 5) return;
            if (maxMoveDistance == 0) maxMoveDistance = _maxMoveDistance;
            int giveUp = 0;
            do
            {
                _targetDirection = Random.insideUnitSphere;
                _target = transform.position + _targetDirection * maxMoveDistance;
            } while (!IsValidMove() && ++giveUp < _maxRetries);

            if (giveUp == _maxRetries)
            {
                RandomMove(maxMoveDistance / 2.0f, trys + 1);
            }
        }

        private void TowardsPlayerMove()
        {
            int giveUp = 0;
            do
            {
                Vector3 target = _player.position + Random.insideUnitSphere * _playerGeneralArea;
                _targetDirection = Vector3.Normalize(target - transform.position);
                _target = transform.position + _targetDirection * _maxMoveDistance;
            } while (!IsValidMove() && ++giveUp < _maxRetries);
            if (giveUp == _maxRetries) RandomMove();
        }

        private void AwayFromPlayerMove()
        {
            int giveUp = 0;
            do
            {
                Vector3 target = _player.position + Random.insideUnitSphere * _playerGeneralArea;
                _targetDirection = -Vector3.Normalize(target - transform.position);
                _target = transform.position + _targetDirection * _maxMoveDistance;
            } while (!IsValidMove() && ++giveUp < _maxRetries);
            if (giveUp == _maxRetries) RandomMove();
        }

        private void NextTarget()
        {
            if (Time.time - _flairTime < _flairDuration)
            {
                Debug.Log("FLAIR");
                AwayFromPlayerMove();
            }
            else if (
                Random.value < (
                    IsLonely()
                        ? _lonelyMoveTowardsPlayerChance
                        : Mathf.Lerp(_nonLonelyMoveTowardsPlayerChance, _noisyNonLonelyMoveTowardsPlayerChance, _noiseLevel)
                )
            ) {
                Debug.Log("PLAYER");
                TowardsPlayerMove();
            }
            else
            {
                Debug.Log("RANDOM");
                RandomMove();
            }
        }
        
        private void KillPlayer()
        {
            Debug.Log("YOU DEAD");
        }
        
        private void LookForPlayer()
        {
            Vector3 dir = Vector3.Normalize(_player.position - transform.position);
            if (Physics.SphereCast(transform.position, _visionThickness, dir, out RaycastHit hit, _visionRange))
            {
                if (hit.transform.CompareTag("Player")) KillPlayer();
            }
        }

        private void Update()
        {
            transform.Translate(_targetDirection * (_moveSpeed * Time.deltaTime));
            if (Vector3.Distance(transform.position, _player.position) < _playerGeneralArea) _lastWithPlayerTime = Time.time;
            if (Vector3.Distance(transform.position, _player.position) < _visionRange)
            {
                LookForPlayer();
            }
            if (Vector3.Dot(_target - transform.position, _targetDirection) < 0)
            {
                NextTarget();
            }
            _noiseLevel -= _noiseDampingRate * Time.deltaTime;
            if (_noiseLevel < 0.0f) _noiseLevel = 0.0f;
        }
    }
}
