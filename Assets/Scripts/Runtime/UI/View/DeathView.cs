using BeneathTheSurface.Events;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeneathTheSurface.UI
{
    public class DeathView : View
    {
        [SerializeField] private GameObject _blur;

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
    }   
}
