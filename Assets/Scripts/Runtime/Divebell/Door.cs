using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PlazmaGames.Core;
using BeneathTheSurface.Interfaces;
using BeneathTheSurface.Player;
using PlazmaGames.Animation;

namespace PsychoSerum.Interactables
{
    internal class Door : MonoBehaviour, IInteractable
    {
        //[SerializeField] private float _rotationDuration = 1.0f;
        [SerializeField] private float _rotationAmount = 90.0f;
        [SerializeField][Range(-1, 1)] private float _forwardDirection = 0;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _lockedSound;
        [SerializeField] private AudioClip _unlockSound;
        [SerializeField] private AudioClip _bangSound;

        [SerializeField] private bool _isLocked;

        [SerializeField] protected MeshRenderer _meshRenderer;
        [SerializeField, ColorUsage(true, true)] protected Color _outlineColor;
        [SerializeField] protected float _outlineScale = 1.03f;
        [SerializeField] private bool _allowInteract = true;

        private bool _isOpen;
        private Vector3 _startRotation;
        private Vector3 _forward;

        private void Rotate(float progress, Quaternion start, Quaternion end)
        {
            transform.localRotation = Quaternion.Slerp(start, end, progress);
        }

        private void Open(Vector3 playerPos)
        {
            _isOpen = true;
            float dot = Vector3.Dot(_forward, (playerPos - transform.position).normalized);
            Quaternion start = transform.localRotation;
            Quaternion end;

            if (dot >= _forwardDirection) end = Quaternion.Euler(0, _startRotation.y - _rotationAmount, 0);
            else end = Quaternion.Euler(0, _startRotation.y + _rotationAmount, 0);

            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _audioSource.clip.length,
                (float progress) => Rotate(progress, start, end)
            );
        }

        public void Close()
        {
            _isOpen = false;
            Quaternion start = transform.localRotation;
            Quaternion end = Quaternion.Euler(_startRotation);
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                 this,
                 _audioSource.clip.length,
                 (float progress) => Rotate(progress, start, end)
             );
        }

        public bool Interact(Interactor interactor)
        {
            if (_isLocked)
            {
                _audioSource.PlayOneShot(_lockedSound);
                return true;
            }

            if (!_isOpen)
            {
                _audioSource.Play();
                Open(interactor.transform.position);
            }
            else
            {
                _audioSource.Play();
                Close();
            }

            return true;
        }

        public void EndInteraction()
        {

        }

        public void Unlock()
        {
            _audioSource.PlayOneShot(_unlockSound);
            _isLocked = false;
        }

        public void Lock()
        {
            _audioSource.PlayOneShot(_unlockSound);
            _isLocked = true;
        }

        public void PlayBang()
        {
            _audioSource.clip = _bangSound;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        public void StopBang()
        {
            _audioSource.Stop();
        }

        private void Awake()
        {
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            _forward = -transform.right;
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
    }
}
