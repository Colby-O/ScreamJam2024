using UnityEngine;
using UnityEngine.InputSystem;

namespace BeneathTheSurface.Player
{
	[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
	internal sealed class PlayerController : MonoBehaviour
	{
		[Header("References")]
        [SerializeField] private Inspector _inspector;
        [SerializeField] private PlayerSettings _playerSettings;
		[SerializeField] private CharacterController _characterController;
		[SerializeField] private PlayerInput _playerInput;
        //[SerializeField] private AudioSource _audioSource;

        [Header("Body Part References")]
		[SerializeField] private GameObject _head;

        [Header("Physics")]
        [SerializeField] private float _gravityMultiplier = 3.0f;
        private float _gravity = -9.81f;
		private float _velY;

		private InputAction _moveAction;
		private InputAction _lookAction;
        private InputAction _pauseAction;

        private Vector3 _playerRotation;
		private Vector3 _headRotation;

		private Vector2 _rawMovementInput;
		private Vector2 _rawViewInput;

		private Vector3 _movementSpeed;
		private Vector3 _movementSpeedVel;

		private bool _inTaskMenu = false;

		private void HandleMovementAction(InputAction.CallbackContext e)
		{
			_rawMovementInput = e.ReadValue<Vector2>();
		}

		private void HandleLookAction(InputAction.CallbackContext e)
		{
			_rawViewInput = e.ReadValue<Vector2>();
		}

		private void ProcessMovement()
		{
			float verticalSpeed = (_rawMovementInput.y == 1) ? _playerSettings.walkingForwardSpeed : _playerSettings.walkingBackwardSpeed;
			float horizontalSpeed = _playerSettings.walkingStrateSpeed;

			verticalSpeed *= _playerSettings.speedEffector;
			horizontalSpeed *= _playerSettings.speedEffector;

			_movementSpeed = Vector3.SmoothDamp(
				_movementSpeed,
				new Vector3(-horizontalSpeed * _rawMovementInput.x * Time.deltaTime, 0, -verticalSpeed * _rawMovementInput.y * Time.deltaTime),
				ref _movementSpeedVel,
				_playerSettings.movementSmoothing
			);
		}

		private void ProcessView()
		{
			_headRotation.x += ((_playerSettings.invertedViewY) ? 1 : -1) * _playerSettings.sensitivityY * _rawViewInput.y * Time.deltaTime;
			_headRotation.x = Mathf.Clamp(_headRotation.x, _playerSettings.minViewY, _playerSettings.maxViewY);
			_head.transform.localRotation = Quaternion.Euler(_headRotation);

			_playerRotation.y += ((_playerSettings.invertedViewX) ? -1 : 1) * _playerSettings.sensitivityX * _rawViewInput.x * Time.deltaTime;
			transform.localRotation = Quaternion.Euler(_playerRotation);
		}

        private void ProcessGravity()
        {
			if (_characterController.isGrounded && _velY < 0.0f) _velY = -1.0f;
			else _velY += _gravity * _gravityMultiplier * Time.deltaTime;
			_movementSpeed.y = _velY;
        }
		
		public void ZeroInput()
		{
            _rawMovementInput = Vector2.zero;
            _rawViewInput = Vector2.zero;
        }

        public void Disable()
        {
            _characterController.enabled = false;
        }

        public void Enable()
        {
            _characterController.enabled = true;
        }

        public void Rotate(float angle)
        {
            _playerRotation = (transform.localRotation * Quaternion.Euler(0f, angle, 0f)).eulerAngles;
        }

		public PlayerSettings GetPlayerSetings()
		{
			return _playerSettings;
		}

        private void HandlePuase(InputAction.CallbackContext e)
		{
			if (!BeneathTheSurfaceGameManager.allowInput || _inspector.IsExaming) return;
            BeneathTheSurfaceGameManager.allowInput = false;
			//GameManager.GetMonoSystem<IUIMonoSystem>().Show<PausedView>();
        }

        private void OnCollisionEnter(Collision collision)
        {
			Debug.Log(collision.gameObject.name);
        }

        private void Awake()
		{
			if (_playerSettings == null) _playerSettings = new PlayerSettings();
			if (_characterController == null) _characterController = GetComponent<CharacterController>();
			if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
			//if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            if (_inspector == null) _inspector = GetComponent<Inspector>();

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
			if (!BeneathTheSurfaceGameManager.allowInput || (_inspector.IsExaming && !_inspector.IsMoveable))
			{
				ZeroInput();
                _playerInput.enabled = false;
			}
            else _playerInput.enabled = true;
            ProcessView();
            ProcessMovement();
            //if (_movementSpeed.magnitude < 0.01f) _audioSource.Stop();
            //else if (!_audioSource.isPlaying) _audioSource.Play();
            ProcessGravity();
            _characterController.Move(transform.TransformDirection(_movementSpeed));
        }
	}
}
