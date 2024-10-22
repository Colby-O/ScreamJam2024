using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Radar
{
    public class Scanline : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lr;

        public void SetPosition(Vector3 pos, int id)
        {
            _lr.SetPosition(id, pos);
        }

        private void Awake()
        {
            _lr.positionCount = 2;
        }
    }
}
