using BeneathTheSurface.MonoSystems;
using PlazmaGames.Animation;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public class BeneathTheSurfaceGameManager : GameManager
    {
        [Header("Holders")]
        [SerializeField] private GameObject _monoSystemParnet;

        [Header("MonoSystems")]
        [SerializeField] private UIMonoSystem _uiMonoSystem;
        [SerializeField] private AnimationMonoSystem _animationMonoSystem;
        [SerializeField] private AudioMonoSystem _audioMonoSystem;
        [SerializeField] private OceanMonoSystem _oceanMonoSystem;
        [SerializeField] private BuildingMonoSystem _buildingMonoSystem;
        [SerializeField] private PipeSystemMonoSystem _pipeSystemMonoSystem;

        public static bool allowInput = true;

        /// <summary>
        /// Adds all events listeners
        /// </summary>
        private void AddListeners()
        {

        }

        /// <summary>
        /// Removes all events listeners
        /// </summary>
        private void RemoveListeners()
        {

        }

        /// <summary>
        /// Attaches all MonoSystems to the GameManager
        /// </summary>
        private void AttachMonoSystems()
        {
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiMonoSystem);
            AddMonoSystem<AnimationMonoSystem, IAnimationMonoSystem>(_animationMonoSystem);
            AddMonoSystem<AudioMonoSystem, IAudioMonoSystem>(_audioMonoSystem);
            AddMonoSystem<OceanMonoSystem, IOceanMonoSystem>(_oceanMonoSystem);
            AddMonoSystem<BuildingMonoSystem, IBuildingMonoSystem>(_buildingMonoSystem);
            AddMonoSystem<PipeSystemMonoSystem, IPipeSystemMonoSystem>(_pipeSystemMonoSystem);
        }

        public override string GetApplicationName()
        {
            return nameof(BeneathTheSurfaceGameManager);
        }

        protected override void OnInitalized()
        {
            // Ataches all MonoSystems to the GameManager
            AttachMonoSystems();

            // Adds Event Listeners
            AddListeners();

            // Ensures all MonoSystems call Awake at the same time.
            _monoSystemParnet.SetActive(true);
        }

        private void Awake()
        {

        }

        private void Start()
        {

        }

        public override string GetApplicationVersion()
        {
            return "Beta V0.0.1";
        }
    }
}