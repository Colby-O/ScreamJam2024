using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BeneathTheSurface
{
    public abstract class GameView : View
    {
        [SerializeField] private TMP_Text _aid;

        public void ToggleAid(bool statis)
        {
            _aid.gameObject.SetActive(statis);
        }
    }

    public class SurfaceView : GameView
    {
        public override void Init()
        {

        }
    }
}
