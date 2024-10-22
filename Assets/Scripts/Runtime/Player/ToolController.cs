using BeneathTheSurface.Events;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.Runtime.DataStructures;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeneathTheSurface.Player
{
    public enum Tool
    {
        Hands,
        Welder,
        Radar
    }
 

    public class ToolController : MonoBehaviour
    {
        [SerializeField] SerializableDictionary<Tool, GameObject> _prefabs;
        [SerializeField] SerializableDictionary<Tool, bool> _toolList;
        [SerializeField] private Transform _offset;
        [SerializeField] private Transform _head;

        [SerializeField, ReadOnly] private Tool _toolSelected = Tool.Hands;

        private GameObject _toolInstance;

        private bool _hasAllItmes = false;

        public void GiveTool(Tool tool)
        {
            _toolList[tool] = true;
        }

        public bool HasTool(Tool tool)
        {
            return _toolList[tool];
        }

        public void SwapTool(Tool tool)
        {
            if (_toolInstance != null)
            {
                Destroy(_toolInstance);
                _toolInstance = null;
            }
            _toolSelected = tool;
            if (_prefabs[tool] != null)
            {
                if (_prefabs[tool] == null) return;
                _toolInstance = GameObject.Instantiate(_prefabs[tool]);
                _toolInstance.transform.position = _offset.position;
                _toolInstance.transform.parent = _head;
                _toolInstance.transform.rotation = Camera.main.transform.rotation;
            }
        }

        public Tool CurrentTool()
        {
            return _toolSelected;
        }

        private void Awake()
        {
            GiveTool(Tool.Hands);
        }

        private void Update()
        {
            if (BeneathTheSurfaceGameManager.allowInput)
            {
                if (Keyboard.current[Key.Digit1].wasPressedThisFrame && HasTool(Tool.Hands))
                {
                    SwapTool(Tool.Hands);
                }
                else if (Keyboard.current[Key.Digit2].wasPressedThisFrame && HasTool(Tool.Radar))
                {
                    if (CurrentTool() != Tool.Radar) SwapTool(Tool.Radar);
                    else SwapTool(Tool.Hands);
                }
                else if (Keyboard.current[Key.Digit3].wasPressedThisFrame && HasTool(Tool.Welder))
                {
                    if (CurrentTool() != Tool.Welder) SwapTool(Tool.Welder);
                    else SwapTool(Tool.Hands);
                }
            }

            if (!_hasAllItmes)
            {
                _hasAllItmes = _toolList.Values.Count(e => e) == _toolList.Values.Count;
                if (_hasAllItmes)
                {
                    GameManager.EmitEvent(new BSEvents.ItemsFeteched());
                }
            }
        }
    }
}
