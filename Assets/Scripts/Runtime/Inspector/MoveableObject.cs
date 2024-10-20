using BeneathTheSurface.Inspectables;
using BeneathTheSurface.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class MoveableObject : InspectableObject
    {
        public bool allowInteract = false;

        public override bool Interact(Interactor interactor)
        {
            if (!allowInteract) return false;

            _auidoSource.PlayOneShot(_auidoclip);
            Inspector inspector = interactor.GetComponent<Inspector>();
            if (inspector.IsExaming) return false;
            inspector.StartExamine(transform, _type, offsetPoint, string.Empty, true);
            return true;
        }
    }
}
