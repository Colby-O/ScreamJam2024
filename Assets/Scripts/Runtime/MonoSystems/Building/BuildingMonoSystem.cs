using BeneathTheSurface.Wielding;
using PlazmaGames.Core.Debugging;
using PlazmaGames.Runtime.DataStructures;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public class BuildingMonoSystem : MonoBehaviour, IBuildingMonoSystem
    {
        [SerializeField] private float _gridSize = 1;
        [SerializeField] private SerializableDictionary<Vector3Int, Pipe> _pipes = new();

        public void SetPipeAt(Pipe pipe, Vector3Int pos)
        {
            if (_pipes.ContainsKey(pos)) _pipes[pos] = pipe;
            else _pipes.Add(pos, pipe);
        }

        public List<Vector3Int> GetAllPipesLocations()
        {
            return _pipes.Keys.ToList();
        }

        public float GetGridSize()
        {
            return _gridSize;
        }

        public Pipe GetPipeAt(Vector3Int pos)
        {
            return HasPipeAt(pos) ? _pipes[pos] : null;
        }

        public bool HasPipeAt(Vector3Int loc)
        {
            return _pipes.ContainsKey(loc);
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
