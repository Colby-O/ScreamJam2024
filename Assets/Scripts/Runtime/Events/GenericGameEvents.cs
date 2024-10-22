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

        private static void StartEvent(Component _, object __)
        {
            BeneathTheSurfaceGameManager.player.CoverScreen();
            BeneathTheSurfaceGameManager.allowInput = true;
            GameManager.EmitEvent(new BSEvents.OpenMenu(true, true, typeof(SurfaceView)));
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(0, PlazmaGames.Audio.AudioType.Sfx, false, true);
            GameManager.GetMonoSystem<IDialogueMonoSystem>().Load(BeneathTheSurfaceGameManager.DialogueDB.GetAllEntries().Where(e => e.order == 1).FirstOrDefault());
        }

        private static void QuitEvent(Component _, object __)
        {
            Application.Quit();
        }
    }
}
