using BeneathTheSurface.Events;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Trigger
{
    public class Trigger2 : Trigger
    {
        protected override bool Condition()
        {
            return BeneathTheSurfaceGameManager.eventProgresss == 5;
        }

        protected override void OnTrigger()
        {
            GameManager.EmitEvent(new BSEvents.EnterDivingBell());
        }
    }
}
