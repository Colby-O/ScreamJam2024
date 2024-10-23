using BeneathTheSurface.Inspectables;
using BeneathTheSurface.Player;
using BeneathTheSurface.Wielding;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class MoveableObject : InspectableObject
    {
        public override bool Interact(Interactor interactor)
        {
            if (!allowInteract) return false;
            Inspector inspector = interactor.GetComponent<Inspector>();
            if (inspector.IsExaming) return false;
            if (_auidoSource != null && _auidoclip != null) _auidoSource.PlayOneShot(_auidoclip);
            Pipe pipe = GetComponent<Pipe>();
            if ( pipe != null) pipe.EnableSnap();
            inspector.StartExamine(transform, _type, offsetPoint, string.Empty, true);
            return true;
        }
    }
}
