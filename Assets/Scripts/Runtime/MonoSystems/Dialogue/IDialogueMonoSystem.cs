using PlazmaGames.Core.MonoSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public interface IDialogueMonoSystem : IMonoSystem
    {
        public void CloseDialogue();
        public void Load(DialogueSO dialogueEvent);
    }
}
