using BeneathTheSurface.Events;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BeneathTheSurface.Trigger
{
    public abstract class Trigger : MonoBehaviour
    {
        [SerializeField] protected bool _canTriggerMoreThanOnce = false;

        [SerializeField, ReadOnly] protected bool _isTriggered = false;

        protected abstract void OnTrigger();

        protected virtual bool Condition()
        {
            return true;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if ((_isTriggered && !_canTriggerMoreThanOnce) || !Condition()) return;

            if (other.gameObject.CompareTag("Player"))
            {
                _isTriggered = true;
                OnTrigger();
            }
        }
    }

    public class Trigger1 : Trigger
    {
        protected override bool Condition()
        {
            return BeneathTheSurfaceGameManager.eventProgresss == 2;
        }

        protected override void OnTrigger()
        {
            GameManager.EmitEvent(new BSEvents.PipeTutorial());
        }
    }
}
