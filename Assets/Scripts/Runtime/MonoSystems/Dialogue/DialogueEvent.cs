using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BeneathTheSurface.MonoSystems
{
    public abstract class DialogueEvent : ScriptableObject
    {
        // Function to run at the start of a dialogue event
        public abstract void OnEnter();

        // Function to run during a dialogue event
        public abstract void OnUpdate();

        // Function to run after a dialogue event
        public abstract void OnExit();

        //  Exit condition
        //  i.e. pervents the next dialogue box from opening
        //  until true
        public virtual bool CanProceed() => true;
    }
}
