using PlazmaGames.Attribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BeneathTheSurface
{
	public class Flare : MonoBehaviour
	{
		[SerializeField] private GameObject _fuse;
		[SerializeField] private ParticleSystem _flame;
		[SerializeField] private float _lifeSpan;

		[SerializeField, ReadOnly] private bool _isOn;
		[SerializeField, ReadOnly] private bool _isUsed;
		[SerializeField, ReadOnly] private float _life;

		private SquidAi _squid;
		
		public void SetFuse()
		{
			if (_isUsed) return;

			_life = 0;
			_isUsed = true;
			_isOn = true;

			_flame.gameObject.SetActive(true);
			_fuse.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

			_squid.SendFlare();
		}

		public bool IsOn()
		{
			return _isOn;
		}

		public bool IsUsed()
		{
			return _isUsed;
		}

		private void Awake()
		{
			_squid = FindObjectOfType<SquidAi>();
			_fuse.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
			_flame.gameObject.SetActive(false);
			_isOn = false;
			_isUsed= false;
		}

		private void Update()
		{
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				SetFuse();
			}

			if ( _isOn )
			{
				_life += Time.deltaTime;

				if (_life > _lifeSpan)
				{
					_fuse.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
					_flame.gameObject.SetActive(false);
					_isOn = false;
				}
			}
		}
	}
}
