using BeneathTheSurface.Interfaces;
using BeneathTheSurface.Player;
using PlazmaGames.Animation;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class Hatch : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] protected MeshRenderer _meshRenderer;
        [SerializeField, ColorUsage(true, true)] protected Color _outlineColor;
        [SerializeField] protected float _outlineScale = 1.03f;
        [SerializeField] private bool _allowInteract = true;
        [SerializeField] private bool _isLocked;
        [SerializeField] private float _openRot;
        [SerializeField] private float _closeRot;

        private bool _isOpen = false;

        private void Rotate(float progress, Quaternion start, Quaternion end)
        {
            transform.localRotation = Quaternion.Slerp(start, end, progress);
        }

        public void Close()
        {
            Quaternion o = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, _closeRot));
            Quaternion c = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, _openRot));
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _audioSource.clip.length,
                (float progress) => Rotate(progress, c, o)
            );
        }

        public bool Interact(Interactor interactor)
        {
            if (_isLocked) return false;

            Quaternion o = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, _closeRot));
            Quaternion c = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, _openRot));

            _audioSource.Play();

            GameManager.GetMonoSystem<IAnimationMonoSystem>().StopAllAnimations(this);

            if (_isOpen)
            {
                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    this,
                    _audioSource.clip.length,
                    (float progress) => Rotate(progress, c, o)
                );
            }
            else
            { 
                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    this,
                    _audioSource.clip.length,
                    (float progress) => Rotate(progress, o, c)
                );
            }

            _isOpen = !_isOpen;

            return true;
        }

        public void Lock()
        {
            _isLocked = true;
        }

        public void Unlock()
        {
            _isLocked = false;
        }

        public bool IsPickupable()
        {
            return false;
        }

        public void OnPickup(Interactor interactor)
        {

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

        public bool IsInteractable()
        {
            return _allowInteract && !_isLocked;
        }

        protected virtual void Update()
        {
            RemoveOutline();
        }

        public void EndInteraction()
        {

        }
    }
}
