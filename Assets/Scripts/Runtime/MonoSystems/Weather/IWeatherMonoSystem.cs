using PlazmaGames.Core.MonoSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public interface IWeatherMonoSystem : IMonoSystem
    {
        public void SetWeatherState(bool isStormy, bool isIndoors);
        public bool IsStromy();
    }
}
