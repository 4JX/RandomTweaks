using HarmonyLib;
using SFS;
using SFS.Builds;
using SFS.Input;
using SFS.Platform;
using SFS.UI;

namespace RandomTweaks
{
    [HarmonyPatch(typeof(WorldsMenu), nameof(WorldsMenu.OpenBuild))]
    public class OverridePauseMenu
    {
        [HarmonyPrefix]
        public static bool OpenBuild(WorldsMenu __instance)
        {
            WorldElement selected = Traverse.Create(__instance).Field("selected").GetValue() as WorldElement;

            if (PlatformManager.current == PlatformType.PC && selected.World.worldPersistentPath.FolderExists())
            {
                MenuGenerator.ShowChoices(null, ButtonBuilder.CreateButton(null, () => "Continue Build", EnterBuildWithCurrent, CloseMode.Current), ButtonBuilder.CreateButton(null, () => "Build New Rocket", EnterBuildWithClean, CloseMode.Current), ButtonBuilder.CreateButton(null, () => "Resume Flight", __instance.OpenWorld, CloseMode.Current));
            }
            else
            {
                EnterBuildWithCurrent();
            }
            void EnterBuildWithCurrent()
            {
                WorldsMenu.EnterWorld(selected.Name, delegate
                {
                    Base.sceneLoader.LoadBuildScene(askBuildNew: false, autoCenterBuild: false);
                    ClearBuildMenuOnEnter.clearBuildGridOnEnter = false;
                });
            }

            void EnterBuildWithClean()
            {
                WorldsMenu.EnterWorld(selected.Name, delegate
                {
                    Base.sceneLoader.LoadBuildScene(askBuildNew: false, autoCenterBuild: false);
                    ClearBuildMenuOnEnter.clearBuildGridOnEnter = true;
                });
            }


            return false;
        }
    }

    [HarmonyPatch(typeof(BuildManager), "Start")]
    public class ClearBuildMenuOnEnter
    {

        public static bool clearBuildGridOnEnter;

        [HarmonyPostfix]
        public static void Start(BuildManager __instance)
        {
            if (clearBuildGridOnEnter)
            {
                BuildState.main.Clear(updateSave: true);
            }
        }
    }
}

