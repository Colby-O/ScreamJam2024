using BeneathTheSurface.Events;
using BeneathTheSurface.MonoSystems;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeneathTheSurface.UI
{
    public class MainMenuView : View
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private float _wiggleAmount = 2f;
        [SerializeField] private float _wiggleSpeed = 2f;

        [SerializeField] private Button _play;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _exit;

        private float _titleHeight = 0;

        private void Play()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().HideAllViews();
        }

        private void Settings()
        {
            GameManager.EmitEvent(new BSEvents.OpenMenu(true, true, typeof(SettingsView)));
        }

        private void Exit()
        {
            GameManager.EmitEvent(new BSEvents.Quit());
        }

        public override void Init()
        {
            _titleHeight = _title.transform.position.y;

            _play.onClick.AddListener(Play);
            _settings.onClick.AddListener(Settings);
            _exit.onClick.AddListener(Exit);
        }

        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public override void Hide()
        {
            base.Hide();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            _title.transform.position = new Vector3(_title.transform.position.x, _titleHeight + _wiggleAmount * Mathf.Sin(_wiggleSpeed * Time.time), _title.transform.position.z);
        }
    }
}
