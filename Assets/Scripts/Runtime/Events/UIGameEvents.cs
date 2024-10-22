using PlazmaGames.Core;
using PlazmaGames.Core.Events;
using PlazmaGames.Core.Debugging;
using PlazmaGames.UI;
using PlazmaGames.UI.Views;
using UnityEngine;
using BeneathTheSurface.UI;

namespace BeneathTheSurface.Events
{
    public static class UIGameEvents
    {
        private static EventResponse _openMenuResponse;
        private static EventResponse _closeMenuResponse;
        private static EventResponse _pauseResponse;

        public static EventResponse OpenMenuResponse
        {
            get
            {
                _openMenuResponse ??= new EventResponse(OpenMenuEvent);
                return _openMenuResponse;
            }
        }

        public static EventResponse CloseMenuResponse
        {
            get
            {
                _closeMenuResponse ??= new EventResponse(CloseMenuEvent);
                return _closeMenuResponse;
            }
        }

        public static EventResponse PauseResponse
        {
            get
            {
                _pauseResponse ??= new EventResponse(PauseEvent);
                return _pauseResponse;
            }
        }

        private static void PauseEvent(Component _, object raw)
        {
            bool hidePreviousView = true;

            if (raw != null && raw is BSEvents.Pause)
            {
                BSEvents.Pause data = raw as BSEvents.Pause;
                hidePreviousView = data.HidePreviousView;
            }

            if (
                GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<PauseMenuView>() ||
                GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<SettingsView>()
            )
            {
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<PauseMenuView>()) BeneathTheSurfaceGameManager.allowInput = true;
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
            }
            else if (!GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<MainMenuView>())
            {
                BeneathTheSurfaceGameManager.allowInput = false;
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<PauseMenuView>(true, hidePreviousView);
            }
        }

        private static void OpenMenuEvent(Component _, object raw)
        {
            if (raw == null || raw is not BSEvents.OpenMenu)
            {
                PlazmaDebug.LogError("Failed to opne view, data passed is null or wrong type.", "Event Error");
                return;
            }

            BSEvents.OpenMenu data = raw as BSEvents.OpenMenu;

            bool hidePreviousView = data.HidePreviousView;
            bool rememberView = data.RememberView;
            System.Type viewType = data.ViewType;

            if (viewType == null)
            {
                PlazmaDebug.LogError("Trying to open an invaild View Type.", "Event Error");
                return;
            }

           GameManager.GetMonoSystem<IUIMonoSystem>().Show(viewType, rememberView, hidePreviousView);
        }

        private static void CloseMenuEvent(Component _, object raw)
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }
    }
}
