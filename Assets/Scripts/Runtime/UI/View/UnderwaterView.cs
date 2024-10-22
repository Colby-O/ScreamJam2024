using PlazmaGames.Core.Utils;
using PlazmaGames.Rendering.CRT;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BeneathTheSurface
{
    public class UnderwaterView : GameView
    {
        private const string date = "2024-10-22";

        [SerializeField] private TMP_Text _timeStamp;

        public override void Init()
        {

        }

        public override void Show()
        {
            base.Show();
            UniversalRenderPipelineUtils.SetRendererFeatureActive<CRTRendererFeature>(true);
        }

        public override void Hide()
        {
            base.Hide();
            UniversalRenderPipelineUtils.SetRendererFeatureActive<CRTRendererFeature>(false);
        }

        public void Update()
        {
            _timeStamp.text = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
        }
    }
}
