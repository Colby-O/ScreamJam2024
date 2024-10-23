using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public class DialogueMonoSystem : MonoBehaviour, IDialogueMonoSystem
    {
        private bool _isDialogueInProgress = false;

        // Keep track if the current dialogue is completed
        private bool _nextDialogue = false;

        // Queue of dialogue events
        private Queue<DialogueSO> _dialogueEvents;

        // Current dialogue event SO
        private DialogueSO _currentDialogueEvent = null;

        // current dialogue
        private Dialogue _currentDialogue;

        private DialogueView _view;

        // Starts a dialogue 
        private void OpenDialogue()
        {
            _nextDialogue = false;
            _currentDialogue = _currentDialogueEvent.dialogues.Dequeue();
            if (_currentDialogue.dialogueEvent == null) _currentDialogue.dialogueEvent = new DefaultDialogueEvent();
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<DialogueView>();
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<DialogueView>().Display(_currentDialogue);
            _currentDialogue.dialogueEvent.OnEnter();
        }

        // Closes a dialogue
        public void CloseDialogue()
        {
            _nextDialogue = true;
        }

        // Loads a dialogue event
        public void Load(DialogueSO dialogueEvent)
        {
            _dialogueEvents.Enqueue(dialogueEvent);
        }

        // Starts next dialogue event in the queue
        private void StartNextEvent()
        {
            if (_dialogueEvents.Count == 0)
            {
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<DialogueView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                return;
            }

            if (_isDialogueInProgress == false && _currentDialogueEvent == null)
            {
                _isDialogueInProgress = true;
                _currentDialogueEvent = _dialogueEvents.Dequeue();
                if (_currentDialogueEvent == null)
                {
                    Debug.LogWarning("Trying to load a dialogue event that is null!");
                }
                else _currentDialogueEvent.StartDialogueEvent();
                OpenDialogue();
            }
            else
            {
                Debug.LogWarning("Trying to load a dialogue event when there is already an event loaded!");
            }
        }

        // Update function to be called when a dialogue event is active
        private void DialogueUpdate()
        {
            if (_nextDialogue && _currentDialogue.dialogueEvent.CanProceed())
            {
                _currentDialogue.dialogueEvent.OnExit();
                if (_currentDialogueEvent.dialogues.Count > 0) OpenDialogue();
                else ResetDialogue();
            }
            else
            {
                _currentDialogue.dialogueEvent.OnUpdate();
            }
        }

        // Resets the dialogue manager for the next event
        private void ResetDialogue()
        {
            _isDialogueInProgress = false;
            _currentDialogueEvent = null;
            _currentDialogue = null;

        }

        private void Awake()
        {
            _dialogueEvents = new Queue<DialogueSO>();
            ResetDialogue();
        }

        private void Update()
        {
            if (_isDialogueInProgress) DialogueUpdate();
            else StartNextEvent();
        }
    }
}
