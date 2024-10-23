using PlazmaGames.Core.Utils;
using PlazmaGames.Rendering.CRT;
using PlazmaGames.Runtime.DataStructures;
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

        [SerializeField] private TMP_Text _oxygenLevel;
        [SerializeField] private TMP_Text _depth;

        [SerializeField] private SerializableDictionary<Languages, List<string>> _titles;

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
            _oxygenLevel.text = _titles[BeneathTheSurfaceGameManager.language][0] + (BeneathTheSurfaceGameManager.player.GetOxygenLevel() * 100.0f).ToString("0.00") + " / 100.00";
            _depth.text = _titles[BeneathTheSurfaceGameManager.language][1] + Mathf.Abs(BeneathTheSurfaceGameManager.player.transform.position.y).ToString("0.00") + " m";
        }
    }
}
