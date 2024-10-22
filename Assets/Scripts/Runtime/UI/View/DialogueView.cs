using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public class DialogueView : View
    {
        [Header("References")]
        [SerializeField] private GameObject _dialoguetext;
        [SerializeField] private TMP_Text _dialogueBox;
        [SerializeField] private AudioSource _audioSource;

        [Header("Settings")]
        [SerializeField] private float _textSpeed = 1f;
        [SerializeField] private float _delayBetweenTextBlocks = 2f;
        [SerializeField] private float _timeout = 3;
        [SerializeField] private bool _allowTextSkip = false;
        private bool _isWriting = false;

        private float _timeSinceDialogueStarted;


        IEnumerator TypeMessage(string msg)
        {
            _isWriting = true;
            if (_audioSource != null) _audioSource.Play();
            _dialogueBox.text = string.Empty;
            foreach (char c in msg)
            {
                _dialogueBox.text += c;
                yield return new WaitForSeconds(_textSpeed);
            }
            if (_audioSource != null) _audioSource.Stop();
            yield return new WaitForSeconds(_delayBetweenTextBlocks);
            _isWriting = false;
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

        }

        private void Update()
        {
            if (_isWriting) {
                _timeSinceDialogueStarted += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space) && !_isWriting) Next(); 

            if (_timeSinceDialogueStarted > _timeout) 
            {
                _isWriting = false;
                if (_audioSource != null) _audioSource.Stop();
            }
        }
    }
}
