using BeneathTheSurface.MonoSystems;
using BeneathTheSurface.UI;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.Core.Events;
using PlazmaGames.Core.Utils;
using PlazmaGames.Rendering.CRT;
using PlazmaGames.UI;
using PsychoSerum.Interactables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BeneathTheSurface.Events
{
    public static class GenericGameEvents
    {
        private static EventResponse _quitResponse;
        private static EventResponse _startResponse;
        private static EventResponse _itemsFetchedResponse;
        private static EventResponse _pipeTutorialResponse;
        private static EventResponse _finishedPipeTutorialResponse;
        private static EventResponse _allItemsTestedResponse;
        private static EventResponse _enterDivingBellResponse;
        private static EventResponse _startDescentResponse;
        private static EventResponse _reachedOceanFloorResponse;
        private static EventResponse _resetResponse;
        private static EventResponse _finishedPipesResponse;
        private static EventResponse _endingResponse;
      
        public static EventResponse QuitResponse { 
            get { 
                _quitResponse ??= new EventResponse(QuitEvent);
                return _quitResponse;
            }
        }

        public static EventResponse StartResponse
        {
            get
            {
                _startResponse ??= new EventResponse(StartEvent);
                return _startResponse;
            }
        }

        public static EventResponse ItemsFetchedResponse
        {
            get
            {
                _itemsFetchedResponse ??= new EventResponse(ItemsFetchedEvent);
                return _itemsFetchedResponse;
            }
        }

        public static EventResponse PipeTutorialResponse
        {
            get
            {
                _pipeTutorialResponse ??= new EventResponse(PipeTutorialEvent);
                return _pipeTutorialResponse;
            }
        }

        public static EventResponse FinishedPipeTutorialResponse
        {
            get
            {
                _finishedPipeTutorialResponse ??= new EventResponse(FinishedPipeTutorialEvent);
                return _finishedPipeTutorialResponse;
            }
        }
        public static EventResponse AllItemsTestedResponse
        {
            get
            {
                _allItemsTestedResponse ??= new EventResponse(AllItemsTestedEvent);
                return _allItemsTestedResponse;
            }
        }
        public static EventResponse EnterDivingBellResponse
        {
            get
            {
                _enterDivingBellResponse ??= new EventResponse(EnterDivingBellEvent);
                return _enterDivingBellResponse;
            }
        }

        public static EventResponse StartDescentResponse
        {
            get
            {
                _startDescentResponse ??= new EventResponse(StartDescentEvent);
                return _startDescentResponse;
            }
        }

        public static EventResponse ReachedOceanFloorResponse
        {
            get
            {
                _reachedOceanFloorResponse ??= new EventResponse(ReachedOceanFloorEvent);
                return _reachedOceanFloorResponse;
            }
        }

        public static EventResponse ResetResponse
        {
            get
            {
                _resetResponse ??= new EventResponse(ResetEvent);
                return _resetResponse;
            }
        }

        public static EventResponse FinishedPipesResponse
        {
            get
            {
                _finishedPipesResponse ??= new EventResponse(FinishedPipesEvent);
                return _finishedPipesResponse;
            }
        }

        public static EventResponse EndingResponse
        {
            get
            {
                _endingResponse ??= new EventResponse(EndingEvent);
                return _endingResponse;
            }
        }

        private static GameObject _underwaterLight;

        private static void StartEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 0) return;
            BeneathTheSurfaceGameManager.isPlaying = true;
            BeneathTheSurfaceGameManager.eventProgresss++;
            BeneathTheSurfaceGameManager.player.CoverScreen();
            BeneathTheSurfaceGameManager.allowInput = true;
            GameObject.FindObjectOfType<MenuMusic>()?.FadeOut();
            GameObject.FindWithTag("MenuLight").SetActive(false);
            _underwaterLight = GameObject.FindWithTag("UnderwaterLight");
            _underwaterLight.SetActive(false);
            GameObject.FindWithTag("StartDoor").GetComponent<Door>().Lock();
            GameObject.FindWithTag("DiveBellDoor").GetComponent<Door>().Lock();
            GameObject.FindWithTag("DiveBellHatch").GetComponent<Hatch>().Lock();
            GameManager.EmitEvent(new BSEvents.OpenMenu(true, true, typeof(SurfaceView)));
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(0, PlazmaGames.Audio.AudioType.Sfx, false, true);
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }

        private static void ItemsFetchedEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 1) return;
            BeneathTheSurfaceGameManager.eventProgresss++;
            GameObject.FindWithTag("StartDoor").GetComponent<Door>().Unlock();
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }

        private static void PipeTutorialEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 2) return;
            BeneathTheSurfaceGameManager.eventProgresss++;
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }
       
        private static void FinishedPipeTutorialEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 3) return;
            BeneathTheSurfaceGameManager.eventProgresss++;
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }
        
        private static void AllItemsTestedEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 4) return;
            BeneathTheSurfaceGameManager.eventProgresss++;
            GameObject.FindWithTag("DiveBellDoor").GetComponent<Door>().Unlock();
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }

        private static void EnterDivingBellEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 5) return;
            BeneathTheSurfaceGameManager.eventProgresss++;
            BeneathTheSurfaceGameManager.player.CoverScreen();
            GameObject.FindWithTag("DiveBellDoor").GetComponent<Door>().Close();
            GameObject.FindWithTag("DiveBellDoor").GetComponent<Door>().Lock();
            GameManager.EmitEvent(new BSEvents.OpenMenu(true, true, typeof(UnderwaterView)));
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
            UniversalRenderPipelineUtils.SetRendererFeatureActive<CRTRendererFeature>(true);
        }
        
        private static void StartDescentEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 6) return;
            BeneathTheSurfaceGameManager.eventProgresss++;
            BeneathTheSurfaceGameManager.player.UncoverScreen();
            GameObject.FindWithTag("DiveBell").GetComponent<DiveBellController>().Decend();
            GameManager.GetMonoSystem<IWeatherMonoSystem>().SetWeatherState(false, false);
            GameObject.FindWithTag("Surface").SetActive(false);
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("DiveBellMove", PlazmaGames.Audio.AudioType.Ambient, true, false);
        }
        private static void ReachedOceanFloorEvent(Component _, object __)
        {
            _underwaterLight.SetActive(true);
            GameObject.FindWithTag("DiveBellHatch").GetComponent<Hatch>().Unlock();
            GameManager.GetMonoSystem<IAudioMonoSystem>().StopAudio(PlazmaGames.Audio.AudioType.Ambient);
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("OceanAmbient", PlazmaGames.Audio.AudioType.Ambient, true, false);
        }

        private static void ResetEvent(Component _, object __)
        {
            GameObject.Destroy(Object.FindObjectOfType<SquidAi>().gameObject);
            GameObject.Instantiate(Resources.Load<SquidAi>("Prefabs/Squid"), new Vector3(0f, -900f, 0f), Quaternion.identity);
            BeneathTheSurfaceGameManager.player.transform.position = GameObject.FindWithTag("DiveBell").transform.position;
            BeneathTheSurfaceGameManager.player.Reset();
            GameManager.EmitEvent(new BSEvents.OpenMenu(true, true, typeof(UnderwaterView)));
        }

        private static void FinishedPipesEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 7) return;
            BeneathTheSurfaceGameManager.eventProgresss++;
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }

        private static void EndingEvent(Component _, object __)
        {
            if (BeneathTheSurfaceGameManager.eventProgresss != 8) return;
            BeneathTheSurfaceGameManager.eventProgresss++;
            BeneathTheSurfaceGameManager.player.Disable();
            BeneathTheSurfaceGameManager.End();
        }

        private static void QuitEvent(Component _, object __)
        {
            Application.Quit();
        }
    }
}
