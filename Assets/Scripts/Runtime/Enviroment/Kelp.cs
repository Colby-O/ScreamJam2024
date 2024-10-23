using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class Kelp : MonoBehaviour
    {
        [SerializeField] private float _renderDistance = 50f;
        [SerializeField] private Renderer _renderer;

        
        void Update()
        {
            _renderer.enabled =  Vector3.Distance(BeneathTheSurfaceGameManager.player.transform.position, transform.position) <= _renderDistance;
        }
    }
}
