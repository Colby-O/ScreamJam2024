using BeneathTheSurface.MonoSystems;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Wielding
{
    public class Pipe : MonoBehaviour
    {
        [SerializeField] List<Transform> _connectionsLocations;
        [SerializeField] float _wieldTime;
        [SerializeField] MeshRenderer _renderer;

        [SerializeField] private bool _isWelded;
        [SerializeField] private bool _exemptFromConnection;

        [SerializeField, ColorUsage(true, true)] private Color _needConnectionOutlineColor;

        [SerializeField, ReadOnly] private Vector3Int _lastPosition;
        [SerializeField, ReadOnly]  private bool _isWielding = false;
        [SerializeField, ReadOnly]  private float _wieldProgress = 0;

        public List<Vector3Int> Connections;

        private float GridSize => GameManager.GetMonoSystem<IBuildingMonoSystem>().GetGridSize();

        public void SetWeldedState(bool isWelded)
        {
            _isWelded = isWelded;

            if (!_isWelded)
            {
                MoveableObject mo = GetComponent<MoveableObject>();
                mo.allowInteract = true;
                mo.SetOutlineScale(1.03f);
                mo.RestoreOutlineColor();
            }
            else
            {
                MoveableObject mo = GetComponent<MoveableObject>();
                mo.allowInteract = false;
            }
        }

        public void SetExemptState(bool isExempt)
        {
            _exemptFromConnection = isExempt;
        }

        public void SetWieldingState(bool isWielding)
        {
            _isWielding = isWielding;
        }

        public bool IsWielded()
        {
            return _isWelded;
        }

        public void ComputeConeections()
        {
            Connections = new List<Vector3Int>();

            foreach (Transform t in _connectionsLocations)
            {
                if (transform.position.x - t.position.x > 0) Connections.Add(_lastPosition + new Vector3Int(-1, 0, 0));
                else if (transform.position.x - t.position.x < 0) Connections.Add(_lastPosition + new Vector3Int(1, 0, 0));
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

        public bool HasConnection(out Pipe pipe)
        {
            foreach (Vector3Int conn in Connections)
            {
                if (GameManager.GetMonoSystem<IBuildingMonoSystem>().CheckConnectionState(_lastPosition, conn))
                {
                    pipe = GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(conn);
                    return true;
                }
            }

            pipe = null;
            return false;
        }

        public bool IsFullyConnected()
        {
            bool isFullyConnected = true;

            foreach (Vector3Int conn in Connections)
            {

                bool hasConn = GameManager.GetMonoSystem<IBuildingMonoSystem>().CheckConnectionState(_lastPosition, conn);
                isFullyConnected &= (hasConn && GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(conn).IsWielded());
            }
            return isFullyConnected || (_exemptFromConnection && HasConnection(out Pipe pipe) && (pipe?.IsWielded() ?? false));
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
            if (IsWielded() && !HasConnection() && !_exemptFromConnection)
            {
                MoveableObject mo = GetComponent<MoveableObject>();
                mo.allowInteract = true;
                mo.SetOutlineScale(1.03f);
                mo.RestoreOutlineColor();

                SetWeldedState(false);
                _wieldProgress = 0f;
            }

            if (IsWielded())
            {
                MoveableObject mo = GetComponent<MoveableObject>();

                mo.allowInteract = false;

                if (IsFullyConnected())
                {
                    mo.SetOutlineScale(0);
                }
                else
                {
                    mo.SetOutlineScale(1.03f);
                    mo.SetOutlineColor(_needConnectionOutlineColor);
                    mo.AddOutline();
                }
            }

            if (!IsWielded()) SetWeldedState(_wieldProgress >= _wieldTime);

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

            if (_isWielding && HasConnection() && !IsWielded()) _wieldProgress += Time.deltaTime;
            if (!HasConnection() && !IsWielded()) _wieldProgress = 0;

            //if (HasConnection() && IsWielded()) _renderer.material.color = Color.green;
            //else _renderer.material.color = Color.red;
        }
    }
}
