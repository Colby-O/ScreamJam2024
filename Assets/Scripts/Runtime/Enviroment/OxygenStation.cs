using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeneathTheSurface.Player;
using BeneathTheSurface.Interfaces;

namespace BeneathTheSurface
{
	public class OxygenStation : MonoBehaviour, IInteractable
	{
		[SerializeField] private Color _outlineColor;
		[SerializeField] private float _outlineScale;

		private PlayerController _player;
		private MeshRenderer _meshRenderer;

		private void Start()
		{
			_player = FindObjectOfType<PlayerController>();
			_meshRenderer = GetComponent<MeshRenderer>();
		}

		public bool Interact(Interactor interactor)
		{
			_player.RefillOxygen();
			return true;
		}

		public void AddOutline()
		{
			_meshRenderer.materials[_meshRenderer.materials.Length - 1].SetColor("_OutlineColor", _outlineColor);
			_meshRenderer.materials[_meshRenderer.materials.Length - 1].SetFloat("_Scale", _outlineScale);
		}

		public void RemoveOutline()
		{
			_meshRenderer.materials[_meshRenderer.materials.Length - 1].SetColor("_OutlineColor", _outlineColor);
			_meshRenderer.materials[_meshRenderer.materials.Length - 1].SetFloat("_Scale", 0);
		}

		public bool IsInteractable() { return true; }

		public void EndInteraction() {}

		public bool IsPickupable() { return false; }

		public void OnPickup(Interactor interactor) {}

		private void Update()
		{
			RemoveOutline();
		}
	}
}
