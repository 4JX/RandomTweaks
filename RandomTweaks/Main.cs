using HarmonyLib;
using ModLoader;
using SFS.World;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomTweaks
{
    public class Main : SFSMod
    {

        public Main() : base(
        "RandomTweakss", // Mod id
        "RandomTweaks", // Mod Name
        "Infinity", // Mod Author
        "v1.1.x", // Mod loader version
        "v1.2" // Mod version
        )
        { }

        public override void early_load()
        {
            Main.patcher = new Harmony("mods.Infinity.RandomTweaks");
            Main.patcher.PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void load()
        {
            //Nothing, yet
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "World_PC")
            {
                worldViewObject = new GameObject("RandomTweaksWorldPcGuiObject");
                worldViewObject.AddComponent<WorldView>();
                worldViewObject.AddComponent<Throttle>();
                UnityEngine.Object.DontDestroyOnLoad(worldViewObject);
                worldViewObject.SetActive(true);
                return;
            }

            UnityEngine.Object.Destroy(Main.worldViewObject);
        }

        public override void unload()
        {
            throw new NotImplementedException();
        }

        public static Harmony patcher;

        public static GameObject worldViewObject;
    }
}
