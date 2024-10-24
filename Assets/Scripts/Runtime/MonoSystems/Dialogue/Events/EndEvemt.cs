using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    [CreateAssetMenu(fileName = "DialogueEvent", menuName = "Dialogue/Events/New End Event")]
    public class EndDialogueEvent : DialogueEvent
    {
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {
            Application.Quit();
        }

        public override void OnUpdate()
        {

        }
    }
}
