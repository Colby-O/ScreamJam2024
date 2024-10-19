using BeneathTheSurface.Interfaces;
using BeneathTheSurface.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Inspectables
{
    internal enum ExamineType
    {
        Goto,
        ComeTo
    }

    internal class InspectableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] protected ExamineType _type = ExamineType.Goto;
        [SerializeField] protected GameObject offsetPoint = null;

        [SerializeField] protected AudioSource _auidoSource;
        [SerializeField] protected AudioClip _auidoclip;

        public virtual bool Interact(Interactor interactor)
        {
            if (_auidoclip != null) _auidoSource.PlayOneShot(_auidoclip);
            Inspector inspector = interactor.GetComponent<Inspector>();
            PlayerController pc = interactor.GetComponent<PlayerController>();
            pc.ZeroInput();
            inspector.StartExamine(transform, _type, offsetPoint);
            return true;
        }

        public void EndInteraction()
        {

        }

        public virtual bool IsPickupable()
        {
            return false;
        }

        public virtual void OnPickup(Interactor interactor)
        {

        }
    }
}
