using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeneathTheSurface.MonoSystems
{
    public class DialogueView : View
    {
        [Header("References")]
        [SerializeField] private GameObject _dialoguetext;
        [SerializeField] private TMP_Text _dialogueBox;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private GameObject _hint;

        [Header("Settings")]
        [SerializeField] private float _textSpeed = 1f;
        [SerializeField] private float _timeout = 3;
        [SerializeField] private bool _allowTextSkip = false;
        private bool _isWriting = false;

        private float _timeSinceDialogueStarted;


        IEnumerator TypeMessage(string msg)
        {
            _isWriting = true;
            if (_audioSource != null && !_audioSource.isPlaying) _audioSource.Play();
            _dialogueBox.text = string.Empty;
            foreach (char c in msg)
            {
                _dialogueBox.text += c;
                yield return new WaitForSeconds(_textSpeed);
            }
            if (_audioSource != null) _audioSource.Stop();
            _isWriting = false;
            _hint.SetActive(true);
        }

        private void Next()
        {
            GameManager.GetMonoSystem<IDialogueMonoSystem>().CloseDialogue();
            Clear();
        }

        public void Display(Dialogue dialogue)
        {
            _timeSinceDialogueStarted = 0;
            StartCoroutine(TypeMessage(dialogue.msg[BeneathTheSurfaceGameManager.language]));
        }

        public void Clear()
        {
            _dialogueBox.text = " ";
            _hint.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
            _dialoguetext.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            _dialoguetext.SetActive(false);
        }

        public override void Init()
        {
            _hint.SetActive(false);
        }

        private void Update()
        {
            if (_isWriting) {
                _timeSinceDialogueStarted += Time.deltaTime;
            }

            if (Keyboard.current[Key.Space].wasPressedThisFrame && (!_isWriting || _allowTextSkip)) Next();

            _timeSinceDialogueStarted += Time.deltaTime;

            if (_timeSinceDialogueStarted > _timeout) 
            {
                _isWriting = false;
                if (_audioSource != null) _audioSource.Stop();
            }
        }
    }
}
