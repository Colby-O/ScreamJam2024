using System;
using BeneathTheSurface.Events;
using BeneathTheSurface.MonoSystems;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace BeneathTheSurface.Player
{
	[RequireComponent(typeof(PlayerInput))]
	public sealed class PlayerController : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private Inspector _inspector;
		[SerializeField] private PlayerSettings _playerSettings;
		[SerializeField] private PlayerInput _playerInput;
		[SerializeField] private GameObject _screenCover;

		[Header("Body Part References")]
		[SerializeField] private GameObject _head;

		[Header("Physics")]
		[SerializeField] private float _underWaterGravityMultiplier = 3.0f;
		[SerializeField] private float _gravityMultiplier = 3.0f;
		private float _gravity = -9.81f;
		private float _velY;

		[SerializeField] private float _oxygenDepletionRate = 0.01f;
		[SerializeField] private float _oxygenLevel = 1.0f;

		private AudioSource _audioSource;
		[SerializeField] private List<AudioClip> _swimSounds;
		private int _swimSoundIndex = 0;
		
		private Rigidbody _rigidbody;

		private InputAction _moveAction;
		private InputAction _lookAction;
		private InputAction _pauseAction;

		private Vector3 _playerRotation;
		private Vector3 _headRotation;

		private Vector2 _rawMovementInput;
		private Vector2 _rawViewInput;
		
		private Transform _squid;

		private bool _inDeathScene = false;
		private bool _isIndoors = false;

		private float _lastSwimTime;
		[SerializeField] private float _swimLength = 2.0f;
		[SerializeField] private float _swimStrength = 4.0f;
		[SerializeField] private float _swimFriction = 0.04f;
		private IOceanMonoSystem _oceanMonoSystem;
		[SerializeField] private float _walkingFriction = 0.1f;

		public void CoverScreen()
		{
			_screenCover.SetActive(true);
		}

		public void UncoverScreen()
		{
			_screenCover.SetActive(false);
		}

		private bool CanSwim()
		{
			return Time.time - _lastSwimTime > _swimLength;
		}

		private void SetSwim()
		{
			_lastSwimTime = Time.time;
			_audioSource.PlayOneShot(_swimSounds[_swimSoundIndex]);
			_swimSoundIndex = (_swimSoundIndex + 1) % _swimSounds.Count;
		}

		public void DeathScene()
		{
			_inDeathScene = true;
			_rigidbody.isKinematic = true;
		}

		private void HandleMovementAction(InputAction.CallbackContext e)
		{
			_rawMovementInput = e.ReadValue<Vector2>();
		}

		private void HandleLookAction(InputAction.CallbackContext e)
		{
			_rawViewInput = e.ReadValue<Vector2>();
		}

		private void ProcessUnderwaterMovement()
		{
			_rigidbody.AddForce(_head.transform.right * (-1.0f * Vector3.Dot(_head.transform.right, _rigidbody.velocity)));
			_rigidbody.AddForce(_head.transform.up * (-1.0f * Vector3.Dot(_head.transform.up, _rigidbody.velocity)));
			_rigidbody.AddForce(Vector3.up * (_gravity * _underWaterGravityMultiplier));
			_rigidbody.AddForce(_head.transform.forward * (-_swimFriction * Vector3.Dot(_head.transform.forward, _rigidbody.velocity)));
			if (CanSwim() && _rawMovementInput.y > 0)
			{
				_rigidbody.AddForce(_head.transform.forward * _rawMovementInput.y * _swimStrength, ForceMode.Impulse);
				SetSwim();
			}
		}
		
		private void ProcessMovement()
		{
			_rigidbody.AddForce(Vector3.up * (_gravity * _gravityMultiplier));
			_rigidbody.AddForce(transform.forward * _rawMovementInput.y * _playerSettings.walkingForwardSpeed);
			_rigidbody.AddForce(transform.right * _rawMovementInput.x * _playerSettings.walkingForwardSpeed);
			_rigidbody.AddForce(new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z) * -_walkingFriction);
		}

		private void ProcessView()
		{
			_headRotation.x += ((_playerSettings.invertedViewY) ? -1 : 1) * _playerSettings.sensitivityY * _rawViewInput.y * Time.deltaTime;
			_headRotation.x = Mathf.Clamp(_headRotation.x, _playerSettings.minViewY, _playerSettings.maxViewY);
			_head.transform.localRotation = Quaternion.Euler(_headRotation);

			_playerRotation.y += ((_playerSettings.invertedViewX) ? -1 : 1) * _playerSettings.sensitivityX * _rawViewInput.x * Time.deltaTime;
			transform.localRotation = Quaternion.Euler(_playerRotation);
		}

		public void ZeroInput()
		{
			_rawMovementInput = Vector2.zero;
			_rawViewInput = Vector2.zero;
		}

		public void Disable()
		{
			_rigidbody.isKinematic = true;
		}

		public void Enable()
		{
			_rigidbody.isKinematic = false;
		}

		public void Rotate(float angle)
		{
			_playerRotation = (transform.localRotation * Quaternion.Euler(0f, angle, 0f)).eulerAngles;
		}

		public PlayerSettings GetPlayerSetings()
		{
			return _playerSettings;
		}

		public void RefillOxygen()
		{
			_oxygenLevel = 1.0f;
		}

		private void HandlePuase(InputAction.CallbackContext e)
		{
			if (_inspector.IsExaming) return;
			GameManager.EmitEvent(new BSEvents.Pause());
		}

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log(collision.gameObject.name);
		}

		private void CheckIfIndoors()
		{
			if (!BeneathTheSurfaceGameManager.allowInput) return;
			_isIndoors = Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 20f, ~gameObject.layer);
			GameManager.GetMonoSystem<IWeatherMonoSystem>().SetWeatherState(true, _isIndoors);
		}

		private void Awake()
		{
			_squid = FindObjectOfType<SquidAi>().transform;
			_oceanMonoSystem = GameManager.GetMonoSystem<IOceanMonoSystem>();
			_audioSource = GetComponent<AudioSource>();
			if (_playerSettings == null) _playerSettings = new PlayerSettings();
			if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
			if (_inspector == null) _inspector = GetComponent<Inspector>();
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.maxLinearVelocity = float.MaxValue;

			_headRotation = _head.transform.localRotation.eulerAngles;

			_moveAction = _playerInput.actions["Movement"];
			_lookAction = _playerInput.actions["Look"];
			_pauseAction = _playerInput.actions["Esc"];

			_moveAction.performed += HandleMovementAction;
			_lookAction.performed += HandleLookAction;
			_pauseAction.performed += HandlePuase;
		}

		private void Update()
		{
			_oxygenLevel -= _oxygenDepletionRate * Time.deltaTime;
			CheckIfIndoors();

			if (_inDeathScene)
			{
				_head.transform.LookAt(_squid);
			}
			else
			{
				if (!BeneathTheSurfaceGameManager.allowInput || (_inspector.IsExaming && !_inspector.IsMoveable))
				{
					ZeroInput();
					_playerInput.enabled = false;
				}
				else _playerInput.enabled = true;
				ProcessView();
				if (transform.position.y > _oceanMonoSystem.GetSeaLevel()) ProcessMovement();
				else ProcessUnderwaterMovement();
			}
		}
	}
}
