using BeneathTheSurface.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeneathTheSurface.Player
{
	[RequireComponent(typeof(PlayerInput))]
	internal sealed class Interactor : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Transform _playerhead;

        [SerializeField] private Transform _interactionPoint;
		[SerializeField] private LayerMask _interactionLayer;
		[SerializeField] private float _interactionRadius = 0.1f;
        [SerializeField] private float _spehreCastRadius = 0.1f;
        
		private InputAction _interactAction;
        private InputAction _pickupAction;
        
		private void StartInteraction(IInteractable interactable)
		{
			interactable.Interact(this);
		}
        private void StartPickup(IInteractable interactable)
        {
            interactable.OnPickup(this);
        }

        private void CheckForInteractionInteract()
		{
			Debug.DrawRay(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, Color.red, 10000f);
			if (Physics.SphereCast(_playerhead.position, _spehreCastRadius, (_interactionPoint.position - _playerhead.position).normalized, out RaycastHit hit, _interactionRadius, _interactionLayer))
			{
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null) StartInteraction(interactable);
            }
		}

        private void CheckForInteractionPickup()
        {
            Debug.DrawRay(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, Color.red, 10000f);
            if (Physics.SphereCast(_playerhead.position, _spehreCastRadius, (_interactionPoint.position - _playerhead.position).normalized, out RaycastHit hit, _interactionRadius, _interactionLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.IsPickupable()) StartPickup(interactable);
            }
        }

        private void Interact(InputAction.CallbackContext e)
		{
            if (!BeneathTheSurfaceGameManager.allowInput) return;
            CheckForInteractionInteract();
		}

        private void Pickup(InputAction.CallbackContext e)
        {
            if (!BeneathTheSurfaceGameManager.allowInput) return;
            CheckForInteractionPickup();
        }

        private void Awake()
		{
			if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();

            _interactAction = _playerInput.actions["Interact"];
            //_pickupAction = _playerInput.actions["Pickup"];

            _interactAction.performed += Interact;
			//_pickupAction.performed += Pickup;
		}
	}
}
