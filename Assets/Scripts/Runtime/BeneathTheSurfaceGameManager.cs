using BeneathTheSurface.Events;
using BeneathTheSurface.MonoSystems;
using BeneathTheSurface.Player;
using PlazmaGames.Animation;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface
{
    public enum Languages
    {
        EN,
        FR
    }

    public class BeneathTheSurfaceGameManager : GameManager
    {
        [Header("Holders")]
        [SerializeField] private GameObject _monoSystemParnet;

        [Header("Databases")]
        [SerializeField] private DialogueDatabase _dialogueDB;

        [Header("MonoSystems")]
        [SerializeField] private UIMonoSystem _uiMonoSystem;
        [SerializeField] private AnimationMonoSystem _animationMonoSystem;
        [SerializeField] private AudioMonoSystem _audioMonoSystem;
        [SerializeField] private OceanMonoSystem _oceanMonoSystem;
        [SerializeField] private BuildingMonoSystem _buildingMonoSystem;
        [SerializeField] private PipeSystemMonoSystem _pipeSystemMonoSystem;
        [SerializeField] private WeatherMonoSystem _weatherMonoSystem;
        [SerializeField] private DialogueMonoSystem _dialogueMonoSystem;

        public static bool allowInput = true;
        public static PlayerController player;

        public static Languages language;
        public static int eventProgresss = 0;
        public static DialogueDatabase DialogueDB => ((BeneathTheSurfaceGameManager)_instance)._dialogueDB;


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
            AddMonoSystem<WeatherMonoSystem, IWeatherMonoSystem>(_weatherMonoSystem);
            AddMonoSystem<DialogueMonoSystem, IDialogueMonoSystem>(_dialogueMonoSystem);
        }

        private void AddEvents()
        {
            AddEventListener<BSEvents.OpenMenu>(UIGameEvents.OpenMenuResponse);
            AddEventListener<BSEvents.CloseMenu>(UIGameEvents.CloseMenuResponse);
            AddEventListener<BSEvents.Pause>(UIGameEvents.PauseResponse);

            AddEventListener<BSEvents.StartGame>(GenericGameEvents.StartResponse);
            AddEventListener<BSEvents.ItemsFeteched>(GenericGameEvents.ItemsFetchedResponse);
            AddEventListener<BSEvents.PipeTutorial>(GenericGameEvents.PipeTutorialResponse);
            AddEventListener<BSEvents.FinishedPipeTutorial>(GenericGameEvents.FinishedPipeTutorialResponse);
            AddEventListener<BSEvents.AllItemsTested>(GenericGameEvents.AllItemsTestedResponse);
            AddEventListener<BSEvents.EnterDivingBell>(GenericGameEvents.EnterDivingBellResponse);
            AddEventListener<BSEvents.StartDescent>(GenericGameEvents.StartDescentResponse);
            AddEventListener<BSEvents.Quit>(GenericGameEvents.QuitResponse);
        }

        private void RemoveEvents()
        {   
            RemoveEventListener<BSEvents.OpenMenu>(UIGameEvents.OpenMenuResponse);
            RemoveEventListener<BSEvents.CloseMenu>(UIGameEvents.CloseMenuResponse);
            RemoveEventListener<BSEvents.Pause>(UIGameEvents.PauseResponse);

            RemoveEventListener<BSEvents.StartGame>(GenericGameEvents.StartResponse);
            RemoveEventListener<BSEvents.ItemsFeteched>(GenericGameEvents.ItemsFetchedResponse);
            RemoveEventListener<BSEvents.PipeTutorial>(GenericGameEvents.PipeTutorialResponse);
            RemoveEventListener<BSEvents.FinishedPipeTutorial>(GenericGameEvents.FinishedPipeTutorialResponse);
            RemoveEventListener<BSEvents.AllItemsTested>(GenericGameEvents.AllItemsTestedResponse);
            RemoveEventListener<BSEvents.EnterDivingBell>(GenericGameEvents.EnterDivingBellResponse);
            RemoveEventListener<BSEvents.StartDescent>(GenericGameEvents.StartDescentResponse);
            RemoveEventListener<BSEvents.Quit>(GenericGameEvents.QuitResponse);
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
            AddEvents();

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
            player = FindObjectOfType<PlayerController>();
        }

        public override string GetApplicationVersion()
        {
            return "Beta V0.0.1";
        }
    }
}