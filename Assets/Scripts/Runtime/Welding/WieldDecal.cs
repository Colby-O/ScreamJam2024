using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class WieldDecal : MonoBehaviour
    {
        [SerializeField] private float _lifeSpan;

        private float _life;

        private void Awake()
        {
            _life = 0;
        }

        private void Update()
        {
            _life += Time.deltaTime;
            if (_life > _lifeSpan)
            {
                Destroy(gameObject);
            }
        }
    }
}
