using BeneathTheSurface.Interfaces;
using BeneathTheSurface.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Inspectables
{
    public enum ExamineType
    {
        Goto,
        ComeTo
    }

    public class InspectableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] protected ExamineType _type = ExamineType.Goto;
        [SerializeField] protected GameObject offsetPoint = null;

        [SerializeField] protected AudioSource _auidoSource;
        [SerializeField] protected AudioClip _auidoclip;

        public virtual bool Interact(Interactor interactor)
        {
            Inspector inspector = interactor.GetComponent<Inspector>();
            if (inspector.IsExaming) return false;
            if (_auidoclip != null) _auidoSource.PlayOneShot(_auidoclip);
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
