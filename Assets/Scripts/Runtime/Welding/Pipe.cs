using BeneathTheSurface.MonoSystems;
using PlazmaGames.Attribute;
using PlazmaGames.Audio;
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
        [SerializeField] float _rendererDist = 100f;

        [SerializeField] private GameObject _icon;

        [SerializeField] private bool _isWelded;
        [SerializeField] private bool _exemptFromConnection;

        [SerializeField, ColorUsage(true, true)] private Color _needConnectionOutlineColor;

        [SerializeField, ReadOnly] private Vector3Int _lastPosition;
        [SerializeField, ReadOnly]  private bool _isWielding = false;
        [SerializeField, ReadOnly]  private float _wieldProgress = 0;

        public List<Vector3Int> Connections;

        private float GridSize => GameManager.GetMonoSystem<IBuildingMonoSystem>().GetGridSize();

        private Quaternion _lastRot;

        [SerializeField] private bool _snap = true;

        private bool _hasHitGroud = false;

        public bool _debug = false;

        public void EnableIcon()
        {
            _icon.SetActive(true);
        }

        public void DisableIcon()
        {
            _icon.SetActive(false);
        }

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

            int i = 0;
            foreach (Transform t in _connectionsLocations)
            {
                if (_debug) Debug.Log(i + ": " + (transform.position - t.position));

                float xMax = Mathf.Abs((transform.position - t.position).x);
                float yMax = Mathf.Abs((transform.position - t.position).y);
                float zMax = Mathf.Abs((transform.position - t.position).z);


                if (xMax > yMax && xMax > zMax)
                {

                    if (transform.position.x - t.position.x > 0) Connections.Add(_lastPosition + new Vector3Int(-1, 0, 0));
                    else if (transform.position.x - t.position.x < 0) Connections.Add(_lastPosition + new Vector3Int(1, 0, 0));
                }
                else if (yMax > xMax && yMax > zMax)
                {
                    if (transform.position.y - t.position.y > 0) Connections.Add(_lastPosition + new Vector3Int(0, -1, 0));
                    else if (transform.position.y - t.position.y < 0) Connections.Add(_lastPosition + new Vector3Int(0, 1, 0));
                }
                else
                {
                    if (transform.position.z - t.position.z > 0) Connections.Add(_lastPosition + new Vector3Int(0, 0, -1));
                    else if (transform.position.z - t.position.z < 0) Connections.Add(_lastPosition + new Vector3Int(0, 0, 1));
                }

                i++;
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

        public void DisableSnap()
        {
            _snap = false;
        }

        public void EnableSnap()
        {
            _snap = true;
        }

        private void Awake()
        {
            DisableIcon();

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
            _renderer.enabled = Vector3.Distance(BeneathTheSurfaceGameManager.player.transform.position, transform.position) <= _rendererDist;

            if (!_snap) return;

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

            if (!IsWielded())
            {
                SetWeldedState(_wieldProgress >= _wieldTime);
                if (IsWielded()) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("FinishedWeld", PlazmaGames.Audio.AudioType.Sfx, false, true);
            }

            Vector3Int pos = new Vector3Int(
                 Mathf.FloorToInt(transform.position.x / GridSize),
                 Mathf.FloorToInt(transform.position.y / GridSize),
                 Mathf.FloorToInt(transform.position.z / GridSize)
             );

            Vector3Int lastPos = _lastPosition;

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

            if (HasConnection() && (lastPos != _lastPosition || _lastRot != transform.rotation) && BeneathTheSurfaceGameManager.isPlaying) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("PlacePipe", PlazmaGames.Audio.AudioType.Sfx, false, true);

            if (_isWielding && HasConnection() && !IsWielded()) _wieldProgress += Time.deltaTime;
            if (!HasConnection() && !IsWielded()) _wieldProgress = 0;

            _lastRot = transform.rotation;
        }
    }
}
