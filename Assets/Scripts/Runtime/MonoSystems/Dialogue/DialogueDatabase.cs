using PlazmaGames.Console;
using PlazmaGames.SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    [CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Database/Dialogue")]
    public class DialogueDatabase : SODatabase<DialogueSO>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Initialize()
        {
            SODatabase<DialogueSO>[] databases = Resources.LoadAll<SODatabase<DialogueSO>>("");
            foreach (SODatabase<DialogueSO> database in databases) database.InitDatabase();
        }
    }
}
