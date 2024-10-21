using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace BeneathTheSurface.Events
{
    public class BSEvents
    {
        public record OpenMenu(bool HidePreviousView = true, bool RememberView = true, System.Type ViewType = default);
        public record CloseMenu();
        public record Pause(bool HidePreviousView = true);
        public record StartGame();
        public record Quit();
    }
}
