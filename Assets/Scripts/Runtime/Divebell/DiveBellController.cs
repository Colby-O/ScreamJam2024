using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class DiveBellController : MonoBehaviour
    {
        [SerializeField] private GameObject _door;
        [SerializeField] private GameObject _hatch;

        [SerializeField] private float _doorOpenRot = 90f;
        [SerializeField] private float _hatchOpenRot = 90f;

        public void Decend()
        {
            Debug.Log("Decending");
        }

    }
}
