using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BeneathTheSurface.Radar
{
    public class RadarIcon : MonoBehaviour
    {
        [SerializeField] private GameObject _icon;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private bool _destoryOnFinish = false;
        [SerializeField, Min(0f)] private float _lifeSpan;

        private float _timeSinceLastPing = 0;

        public void SetDestoryState(bool destoryOnFinish)
        {
            _destoryOnFinish = destoryOnFinish;
        }

        public void Ping()
        {
            _timeSinceLastPing = 0;
            _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, 1f);
        }

        private void Awake()
        {
            if (_renderer == null && _icon != null) _renderer = _icon.GetComponent<Renderer>();
            _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, 0f);
        }

        private void Update()
        {
            _timeSinceLastPing += Time.deltaTime;

            if (_timeSinceLastPing > _lifeSpan)
            {
                if (_destoryOnFinish) Destroy(_icon.gameObject);
                return;
            }
            else
            {
                _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, Mathf.Clamp01(1f - _timeSinceLastPing / _lifeSpan));
            }
        }
    }
}
