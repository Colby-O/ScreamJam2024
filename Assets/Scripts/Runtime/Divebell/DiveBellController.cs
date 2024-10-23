using BeneathTheSurface.Events;
using PlazmaGames.Animation;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class DiveBellController : MonoBehaviour
    {
        [SerializeField] private GameObject _door;
        [SerializeField] private GameObject _hatch;

        [SerializeField] private float _oceanFloorDepth = 100f;
        [SerializeField] private float _desandTime = 15f;

        private float _playerY = 0;

        private void MoveStep(float t, float start, float end)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(start, end, t), transform.position.z);
            BeneathTheSurfaceGameManager.player.transform.position = new Vector3(BeneathTheSurfaceGameManager.player.transform.position.x, _playerY + transform.position.y, BeneathTheSurfaceGameManager.player.transform.position.z);
        }

        public void Decend()
        {
            float start = transform.position.y;
            _playerY = BeneathTheSurfaceGameManager.player.transform.position.y - transform.position.y;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(this, _desandTime, (float t) => MoveStep(t, start, _oceanFloorDepth), () => GameManager.EmitEvent(new BSEvents.ReachedOceanFloor()));
        }

    }
}
