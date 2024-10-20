using BeneathTheSurface.Wielding;
using PlazmaGames.Core.Debugging;
using PlazmaGames.Runtime.DataStructures;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public class BuildingMonoSystem : MonoBehaviour, IBuildingMonoSystem
    {
        [SerializeField] private float _gridSize = 1;
        [SerializeField] private SerializableDictionary<Vector3Int, Pipe> _pipes;

        public float GetGridSize()
        {
            return _gridSize;
        }

        public bool CheckConnectionState(Vector3Int cur, Vector3Int next)
        {
            return _pipes.ContainsKey(cur) && _pipes.ContainsKey(next) && _pipes[cur].Connections.Contains(next) && _pipes[next].Connections.Contains(cur);
        }

        public bool AddPipe(Pipe pipe, Vector3Int position)
        {
            if (_pipes.ContainsKey(position)) return false;
            _pipes.Add(position, pipe);
            return true;
        }

        public bool RemovePipe(Vector3Int position)
        {
            if (!_pipes.ContainsKey(position)) return false;
            _pipes.Remove(position);
            return true;
        }
    }
}
