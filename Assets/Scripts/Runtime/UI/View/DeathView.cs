using BeneathTheSurface.Events;
using PlazmaGames.Core;
using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeneathTheSurface.UI
{
    public class DeathView : View
    {
        [SerializeField] private GameObject _blur;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _aid;

        [SerializeField] private SerializableDictionary<Languages, List<string>> _titles;

        public override void Init()
        {

        }

        public override void Show()
        {
            base.Show();
            _blur.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            _blur.SetActive(false);
        }

        private void Update()
        {
            _title.text = _titles[BeneathTheSurfaceGameManager.language][0];
            _aid.text = _titles[BeneathTheSurfaceGameManager.language][1];
        }
    }   
}
