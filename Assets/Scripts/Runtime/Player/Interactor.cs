using BeneathTheSurface.Inspectables;
using BeneathTheSurface.Interfaces;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeneathTheSurface.Player
{
	[RequireComponent(typeof(PlayerInput))]
	public sealed class Interactor : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Transform _playerhead;
        [SerializeField] private ToolController _tools;

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

        private void CheckForInteractionInteract()
		{
            if (_tools.CurrentTool() != Tool.Hands) return;
            Debug.DrawRay(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, Color.red, 10000f);
			if 
            (
                Physics.Raycast(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, out RaycastHit hit, _interactionRadius, _interactionLayer) ||
                Physics.SphereCast(_playerhead.position, _spehreCastRadius, (_interactionPoint.position - _playerhead.position).normalized, out hit, _interactionRadius, _interactionLayer)
            )
			{
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null) StartInteraction(interactable);
            }
		}

        private void CheckForPossibleInteractionInteract()
        {
            GameView gv = GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentView<GameView>();
            if (gv == null) return;
            gv.ToggleAid(false);
            if (_tools.CurrentTool() != Tool.Hands) return;
            if 
            (
                Physics.Raycast(_playerhead.position, (_interactionPoint.position - _playerhead.position).normalized, out RaycastHit hit, _interactionRadius, _interactionLayer) ||
                Physics.SphereCast(_playerhead.position, _spehreCastRadius, (_interactionPoint.position - _playerhead.position).normalized, out hit, _interactionRadius, _interactionLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    if (!interactable.IsInteractable()) return;
                    gv.ToggleAid(true);
                    interactable.AddOutline();
                }
            }
        }

        private void Interact(InputAction.CallbackContext e)
		{
            if (!BeneathTheSurfaceGameManager.allowInput) return;
            CheckForInteractionInteract();
		}

        private void Awake()
		{
			if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
            if (_tools == null) _tools = GetComponent<ToolController>();

            _interactAction = _playerInput.actions["Interact"];

            _interactAction.performed += Interact;
		}

        private void LateUpdate()
        {
            CheckForPossibleInteractionInteract();
        }
    }
}
