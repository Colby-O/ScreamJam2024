using BeneathTheSurface.Player;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Inspectables
{
    public class PickableObject : InspectableObject
    {
        [SerializeField] private Tool _tool;

        public override bool Interact(Interactor interactor)
        {
            if (!allowInteract) return false;

            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_auidoclip, PlazmaGames.Audio.AudioType.Sfx, false, true);
            ToolController tools = interactor.GetComponent<ToolController>();
            tools.GiveTool(_tool);
            Destroy(gameObject);
            return true;
        }
    }
}
