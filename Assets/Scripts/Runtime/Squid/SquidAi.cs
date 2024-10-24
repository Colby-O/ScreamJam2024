using System.Collections;
using System.Collections.Generic;
using BeneathTheSurface.Player;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.Serialization;

namespace BeneathTheSurface
{
	[RequireComponent(typeof(Rigidbody))]
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
		[SerializeField] private float _flareDuration = 4.0f;
		[SerializeField] private float _noiseDampingRate;
		[SerializeField] private float _noiseLevel = 0.0f;
		private float _flareTime = 0.0f;
		private float _lastWithPlayerTime = 0.0f;

		private Rigidbody _rigidbody;
		private Animator _animator;
		private Transform _squidBody;

        private Quaternion _startRotation;

        private bool IsLonely() => Time.time - _lastWithPlayerTime >= _lonelyTime;
		
		[SerializeField] Vector3 _target;
		private Vector3 _targetDirection => Vector3.Normalize(_target - transform.position);
		
		private PlayerController _player;
		[SerializeField] private int _maxRetries = 20;
		[SerializeField] private float _swimFriction;
		[SerializeField] private float _swimLength;
		[SerializeField] private float _swimStrength;
		[SerializeField] private float _rotateSpeed = 40.0f;
		private float _lastSwimTime;

		private float _swimAnimationTime;

		public void SendFlare()
		{
			_flareTime = Time.time;
			_target = transform.position;
		}

		public void ApplyNoise(float level)
		{
			_noiseLevel += level;
			if (_noiseLevel > 1.0f) _noiseLevel = 1.0f;
		}

		private void Start()
		{
			_animator = GetComponentInChildren<Animator>();
			_squidBody = transform.GetChild(0);
            _startRotation = _squidBody.rotation;
            _swimAnimationTime = _animator.GetCurrentAnimatorStateInfo(0).length;
			Debug.Log(_swimAnimationTime);
			_rigidbody = GetComponent<Rigidbody>();
			Debug.Log(_rigidbody);
			_player = FindObjectOfType<PlayerController>();
			_lastWithPlayerTime = Time.time;
			NextTarget();
		}

		private bool IsValidMove()
		{
			return (
				!Physics.SphereCast(transform.position, _visionThickness, _targetDirection, out var _, _maxMoveDistance) &&
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
				Vector3 dir = Random.insideUnitSphere;
				_target = transform.position + dir * maxMoveDistance;
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
				Vector3 target = _player.transform.position + Random.insideUnitSphere * _playerGeneralArea;
				Vector3 dir = Vector3.Normalize(target - transform.position);
				_target = transform.position + dir * _maxMoveDistance;
			} while (!IsValidMove() && ++giveUp < _maxRetries);
			if (giveUp == _maxRetries) RandomMove();
		}

		private void AwayFromPlayerMove()
		{
			int giveUp = 0;
			do
			{
				Vector3 target = _player.transform.position + Random.insideUnitSphere * _playerGeneralArea;
				Vector3 dir = -Vector3.Normalize(target - transform.position);
				_target = transform.position + dir * _maxMoveDistance;
			} while (!IsValidMove() && ++giveUp < _maxRetries);
			if (giveUp == _maxRetries) RandomMove();
		}

		private void NextTarget()
		{
			if (Time.time - _flareTime < _flareDuration)
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
			_animator.SetTrigger("Attack");
			_attacking = true;
			_target = _player.transform.position;
			_swimLength /= 3.0f;
			_rotateSpeed *= 3.0f;
			_player.DeathScene();
		}

        public void Reset()
        {
            _animator.SetTrigger("Reset");
            _attacking = false;
            _swimLength *= 3.0f;
            _rotateSpeed /= 3.0f;
            _squidBody.rotation = _startRotation;
            NextTarget();
        }

        private void LookForPlayer()
		{
			if (Time.time - _flareTime < _flareDuration) return;
			if (_player.IsHidden()) return;
			Vector3 dir = Vector3.Normalize(_player.transform.position - transform.position);
			if (Physics.SphereCast(transform.position, _visionThickness, dir, out RaycastHit hit, _visionRange))
			{
				if (hit.transform.CompareTag("Player")) KillPlayer();
			}
		}


		private bool _swimTrigger = false;
		private bool _attacking = false;
		private float _attackRotationProgress = 0.0f;

		private void Update()
		{
			_rigidbody.isKinematic = BeneathTheSurfaceGameManager.isPaused;

			if (_attacking && _attackRotationProgress < 1.0f)
			{
				_squidBody.rotation *= Quaternion.AngleAxis(180.0f * Time.deltaTime, Vector3.right);
				_attackRotationProgress += Time.deltaTime;
			}
			if (_attacking) _target = _player.transform.position;
			_rigidbody.AddForce(transform.right * (-1.0f * Vector3.Dot(transform.right, _rigidbody.velocity)));
			_rigidbody.AddForce(transform.up * (-1.0f * Vector3.Dot(transform.up, _rigidbody.velocity)));
			_rigidbody.AddForce(transform.forward * (-_swimFriction * Vector3.Dot(transform.forward, _rigidbody.velocity)));

			if (!_attacking && !_swimTrigger && Time.time + _swimAnimationTime * 0.75f - _lastSwimTime > _swimLength)
			{
				_animator.SetTrigger("Swim");
				_swimTrigger = true;
			}
			if (Time.time - _lastSwimTime > _swimLength)
			{
				_rigidbody.AddForce(transform.forward * _swimStrength, ForceMode.Impulse);
				_lastSwimTime = Time.time;
				_swimTrigger = false;
			}

			Quaternion _targetRotation = Quaternion.LookRotation(_targetDirection, transform.up);
			transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * _rotateSpeed);
			
			float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
			if (distanceToPlayer < _playerGeneralArea) _lastWithPlayerTime = Time.time;
			if (!_attacking && distanceToPlayer < _visionRange)
			{
				LookForPlayer();
			}
			if (!_attacking && Vector3.Distance(_target, transform.position) < _visionThickness * 2.0f)
			{
				NextTarget();
			}
			_noiseLevel -= _noiseDampingRate * Time.deltaTime;
			if (_noiseLevel < 0.0f) _noiseLevel = 0.0f;
		}
	}
}
