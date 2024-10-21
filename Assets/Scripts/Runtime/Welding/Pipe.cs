using BeneathTheSurface.MonoSystems;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.Core.Debugging;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;

namespace BeneathTheSurface.Wielding
{
    public class Pipe : MonoBehaviour
    {
        [SerializeField] List<Transform> _connectionsLocations;
        [SerializeField] float _wieldTime;
        [SerializeField] MeshRenderer _renderer;

        [SerializeField, ReadOnly] private Vector3Int _lastPosition;
        [SerializeField, ReadOnly]  private bool _isWielding = false;
        [SerializeField, ReadOnly]  private float _wieldProgress = 0;

        public List<Vector3Int> Connections;

        private float GridSize => GameManager.GetMonoSystem<IBuildingMonoSystem>().GetGridSize();

        public void SetWieldingState(bool isWielding)
        {
            _isWielding = isWielding;
        }

        public bool IsWielded()
        {
            return _wieldProgress >= _wieldTime;
        }

        public void ComputeConeections()
        {
            Connections = new List<Vector3Int>();

            foreach (Transform t in _connectionsLocations)
            {
                if (transform.position.x - t.position.x > 0) Connections.Add(_lastPosition + new Vector3Int(1, 0, 0));
                else if (transform.position.x - t.position.x < 0) Connections.Add(_lastPosition + new Vector3Int(-1, 0, 0));
                else if (transform.position.y - t.position.y > 0) Connections.Add(_lastPosition + new Vector3Int(0, -1, 0));
                else if (transform.position.y - t.position.y < 0) Connections.Add(_lastPosition + new Vector3Int(0, 1, 0));
                else if (transform.position.z - t.position.z > 0) Connections.Add(_lastPosition + new Vector3Int(0, 0, -1));
                else if (transform.position.z - t.position.z < 0) Connections.Add(_lastPosition + new Vector3Int(0, 0, 1));
            }
        }

        public Vector3Int GetGridPosition()
        {
            return _lastPosition;
        }

        public bool HasConnection()
        {
            foreach (Vector3Int conn in Connections)
            {
                if (GameManager.GetMonoSystem<IBuildingMonoSystem>().CheckConnectionState(_lastPosition, conn)) return true;
            }
            return false;
        }

        private void Awake()
        {
            Connections = new List<Vector3Int>();
            _lastPosition = new Vector3Int(
                Mathf.FloorToInt(transform.position.x / GridSize),
                Mathf.FloorToInt(transform.position.y / GridSize),
                Mathf.FloorToInt(transform.position.z / GridSize)
            );

            ComputeConeections();
            GameManager.GetMonoSystem<IBuildingMonoSystem>().AddPipe(this, _lastPosition);
        }

        private void LateUpdate()
        {
            if (IsWielded()) GetComponent<MoveableObject>().allowInteract = false;

            Vector3Int pos = new Vector3Int(
                 Mathf.FloorToInt(transform.position.x / GridSize),
                 Mathf.FloorToInt(transform.position.y / GridSize),
                 Mathf.FloorToInt(transform.position.z / GridSize)
             );
            
            if (_lastPosition != pos)
            {
                bool canMovePipe = GameManager.GetMonoSystem<IBuildingMonoSystem>().AddPipe(this, pos);

                if (canMovePipe)
                {
                    GameManager.GetMonoSystem<IBuildingMonoSystem>().RemovePipe(_lastPosition);
                    _lastPosition = pos;
                }
            }

            transform.position = new Vector3(_lastPosition.x, _lastPosition.y, _lastPosition.z) * GridSize;
            ComputeConeections();

            if (_isWielding && HasConnection()) _wieldProgress += Time.deltaTime;
            if (!HasConnection()) _wieldProgress = 0;

            //if (HasConnection() && IsWielded()) _renderer.material.color = Color.green;
            //else _renderer.material.color = Color.red;
        }
    }
}
