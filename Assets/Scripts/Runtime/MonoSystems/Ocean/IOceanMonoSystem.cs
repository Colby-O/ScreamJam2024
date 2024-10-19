using PlazmaGames.Core.MonoSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public interface IOceanMonoSystem : IMonoSystem
    {
        public Vector3 CalculateWaveAt(Vector2 position);
        public float GetSeaLevel();
    }
}
