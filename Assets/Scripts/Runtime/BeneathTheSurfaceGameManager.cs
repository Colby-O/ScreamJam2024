using BeneathTheSurface.Events;
using BeneathTheSurface.MonoSystems;
using BeneathTheSurface.Player;
using PlazmaGames.Animation;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.Core.Utils;
using PlazmaGames.Rendering.CRT;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public static bool isPaused = false;
        public static PlayerController player;

        public static Languages language;
        public static int eventProgresss = 0;
        public static DialogueDatabase DialogueDB => ((BeneathTheSurfaceGameManager)_instance)._dialogueDB;

        public static bool isPlaying = false;

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
            AddEventListener<BSEvents.ReachedOceanFloor>(GenericGameEvents.ReachedOceanFloorResponse);
            AddEventListener<BSEvents.ResetEvent>(GenericGameEvents.ResetResponse);
            AddEventListener<BSEvents.FinishedPipes>(GenericGameEvents.FinishedPipesResponse);
            AddEventListener<BSEvents.Ending>(GenericGameEvents.EndingResponse);
            AddEventListener<BSEvents.Quit>(GenericGameEvents.QuitResponse);
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
            UniversalRenderPipelineUtils.SetRendererFeatureActive<CRTRendererFeature>(false);
        }

        public static void End()
        {
            BeneathTheSurfaceGameManager.player.CoverScreen();
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
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