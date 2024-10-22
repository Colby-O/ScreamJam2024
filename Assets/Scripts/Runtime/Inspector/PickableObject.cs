using BeneathTheSurface.Interfaces;
using BeneathTheSurface.Player;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Inspectables
{
    public class PickableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private Tool _tool;
        [SerializeField] private bool _allowInteract = true;

        [SerializeField] protected AudioClip _auidoclip;

        [SerializeField] protected MeshRenderer _meshRenderer;
        [SerializeField, ColorUsage(true, true)] protected Color _outlineColor;
        [SerializeField] protected float _outlineScale = 1.03f;

        public void AddOutline()
        {
            _meshRenderer.materials[_meshRenderer.materials.Length - 1].SetColor("_OutlineColor", _outlineColor);
            _meshRenderer.materials[_meshRenderer.materials.Length - 1].SetFloat("_Scale", _outlineScale);
        }

        public void EndInteraction()
        {
            throw new System.NotImplementedException();
        }

        public bool Interact(Interactor interactor)
        {
            if (!_allowInteract) return false;

            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_auidoclip, PlazmaGames.Audio.AudioType.Sfx, false, true);
            ToolController tools = interactor.GetComponent<ToolController>();
            tools.GiveTool(_tool);
            Destroy(gameObject);
            return true;
        }

        public bool IsInteractable()
        {
            return _allowInteract;
        }

        public bool IsPickupable()
        {
            return false;
        }

        public void OnPickup(Interactor interactor)
        {

        }

        public void RemoveOutline()
        {
            _meshRenderer.materials[_meshRenderer.materials.Length - 1].SetColor("_OutlineColor", _outlineColor);
            _meshRenderer.materials[_meshRenderer.materials.Length - 1].SetFloat("_Scale", 0);
        }

        protected virtual void Update()
        {
            RemoveOutline();
        }
    }
}
