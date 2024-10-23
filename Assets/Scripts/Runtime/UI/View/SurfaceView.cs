using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BeneathTheSurface
{
    public abstract class GameView : View
    {
        [SerializeField] protected TMP_Text _aid;

        public void ToggleAid(bool statis)
        {
            _aid.gameObject.SetActive(statis);
        }
    }

    public class SurfaceView : GameView
    {
        [SerializeField] private SerializableDictionary<Languages, List<string>> _titles;
        public override void Init()
        {

        }

        private void Update()
        {
            _aid.text = _titles[BeneathTheSurfaceGameManager.language][0];
        }
    }
}
