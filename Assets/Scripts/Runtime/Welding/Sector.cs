using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.Wielding
{
    public class SectorObject : MonoBehaviour
    {
        [SerializeField] private Material _onMat;
        [SerializeField] private Material _offMat;
        [SerializeField] private GameObject _icon;

        public void Enable()
        {
            _icon.GetComponent<Renderer>().material = _onMat;
        }

        public void Disable()
        {
            _icon.GetComponent<Renderer>().material = _offMat;
        }
    }
}
