using BeneathTheSurface.MonoSystems;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.Core.Events;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeneathTheSurface.Events
{
    public static class GenericGameEvents
    {
        private static EventResponse _quitResponse;
        private static EventResponse _startResponse;
        private static EventResponse _itemsFetchedResponse;
        private static EventResponse _pipeTutorialResponse;
        private static EventResponse _FinishedPipeTutorialResponse;

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
                _FinishedPipeTutorialResponse ??= new EventResponse(FinishedPipeTutorialEvent);
                return _FinishedPipeTutorialResponse;
            }
        }

        private static void StartEvent(Component _, object __)
        {
            BeneathTheSurfaceGameManager.eventProgresss++;
            BeneathTheSurfaceGameManager.player.CoverScreen();
            BeneathTheSurfaceGameManager.allowInput = true;
            GameManager.EmitEvent(new BSEvents.OpenMenu(true, true, typeof(SurfaceView)));
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(0, PlazmaGames.Audio.AudioType.Sfx, false, true);
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }

        private static void ItemsFetchedEvent(Component _, object __)
        {
            BeneathTheSurfaceGameManager.eventProgresss++;
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }

        private static void PipeTutorialEvent(Component _, object __)
        {
            BeneathTheSurfaceGameManager.eventProgresss++;
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == BeneathTheSurfaceGameManager.eventProgresss).FirstOrDefault());
        }

        private static void FinishedPipeTutorialEvent(Component _, object __)
        {
            BeneathTheSurfaceGameManager.eventProgresss++;
            Debug.Log("Finsihed Pipe Tutorial!");
        }

        private static void QuitEvent(Component _, object __)
        {
            Application.Quit();
        }
    }
}
