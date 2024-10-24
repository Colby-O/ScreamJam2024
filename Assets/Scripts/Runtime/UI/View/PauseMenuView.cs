using BeneathTheSurface.Events;
using PlazmaGames.Core;
using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeneathTheSurface.UI
{
    public class PauseMenuView : View
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private float _wiggleAmount = 5f;
        [SerializeField] private float _wiggleSpeed = 2f;

        [SerializeField] private Button _resume;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _exit;

        [SerializeField] private TMP_Text _resumeT;
        [SerializeField] private TMP_Text _settingsT;
        [SerializeField] private TMP_Text _exitT;

        [SerializeField] private GameObject _blur;

        [SerializeField] private SerializableDictionary<Languages, List<string>> _titles;

        private float _titleHeight = 0;

        private void Exit()
        {
            GameManager.EmitEvent(new BSEvents.Quit());
        }

        private void Settings()
        {
            GameManager.EmitEvent(new BSEvents.OpenMenu(true, true, typeof(SettingsView)));
        }

        private void Resume()
        {
            BeneathTheSurfaceGameManager.isPaused = false;
            BeneathTheSurfaceGameManager.player.Enable();
            _blur.SetActive(false);
            GameManager.EmitEvent(new BSEvents.Pause());
        }

        public override void Init()
        {
            _titleHeight = _title.transform.position.y;

            _resume.onClick.AddListener(Resume);
            _settings.onClick.AddListener(Settings);   
            _exit.onClick.AddListener(Exit);    

            _blur.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
            BeneathTheSurfaceGameManager.isPaused = true;
            _blur.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            BeneathTheSurfaceGameManager.player.Disable();
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

            _title.text = _titles[BeneathTheSurfaceGameManager.language][0];
            _resumeT.text = _titles[BeneathTheSurfaceGameManager.language][1];
            _settingsT.text = _titles[BeneathTheSurfaceGameManager.language][2];
            _exitT.text = _titles[BeneathTheSurfaceGameManager.language][3];
        }
    }
}
