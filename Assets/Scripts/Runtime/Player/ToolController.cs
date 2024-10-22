using PlazmaGames.Attribute;
using PlazmaGames.Runtime.DataStructures;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Transform _offset;
        [SerializeField] private Transform _head;

        [SerializeField, ReadOnly] private Tool _toolSelected = Tool.Hands;

        GameObject _toolInstance;

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

        private void Update()
        {
            if (BeneathTheSurfaceGameManager.allowInput)
            {
                if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
                {
                    SwapTool(Tool.Hands);
                }
                else if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
                {
                    if (CurrentTool() != Tool.Radar) SwapTool(Tool.Radar);
                    else SwapTool(Tool.Hands);
                }
                else if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
                {
                    if (CurrentTool() != Tool.Welder) SwapTool(Tool.Welder);
                    else SwapTool(Tool.Hands);
                }
            }
        }
    }
}
