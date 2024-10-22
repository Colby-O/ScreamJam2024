using BeneathTheSurface.Events;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    [CreateAssetMenu(fileName = "EnterDiveBellEvent", menuName = "Dialogue/Events/New Enter Dive Bel lEvent")]
    public class EnterDiveBellEvent : DialogueEvent
    {
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {
            GameManager.EmitEvent(new BSEvents.StartDescent());
        }

        public override void OnUpdate()
        {

        }
    }
}
