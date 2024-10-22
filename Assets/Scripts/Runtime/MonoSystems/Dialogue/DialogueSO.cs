using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BeneathTheSurface.MonoSystems
{
    [System.Serializable]
    public class Dialogue
    {
        public SerializableDictionary<Languages, string> msg;
        public DialogueEvent dialogueEvent;
    }

    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New Dialogue")]
    public class DialogueSO : BaseSO
    {
        [SerializeField] private List<Dialogue> _dialogues = new List<Dialogue>();
        public Queue<Dialogue> dialogues;
        public int order;

        // To be called before a dialogue event is started
        public void StartDialogueEvent() => dialogues = new Queue<Dialogue>(_dialogues);
    }
}
