using BeneathTheSurface.Wielding;
using PlazmaGames.Core.MonoSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public interface IBuildingMonoSystem : IMonoSystem
    {
        public float GetGridSize();
        public bool CheckConnectionState(Vector3Int cur, Vector3Int next);
        public bool AddPipe(Pipe pipe, Vector3Int position);
        public bool RemovePipe(Vector3Int position);
    }
}
