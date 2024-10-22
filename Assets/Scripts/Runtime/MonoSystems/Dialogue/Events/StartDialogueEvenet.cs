using BeneathTheSurface.Events;
using BeneathTheSurface.MonoSystems;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeneathTheSurface
{
    [CreateAssetMenu(fileName = "DialogueOneEvent", menuName = "Dialogue/Events/New Dialogue One Event")]
    public class DialogueOneEvent : DialogueEvent
    {
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {
            BeneathTheSurfaceGameManager.player.UncoverScreen();
        }

        public override void OnUpdate()
        {

        }
    }
}
