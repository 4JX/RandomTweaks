using System;
using HarmonyLib;
using ModLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using SFS.World;

namespace RandomTweaks
{
	public class Main : SFSMod
	{
		public string getModAuthor()
		{
			return "Infinity";
		}

		public string getModName()
		{
			return "RandomTweaks";
		}

		public void load()
		{
			Main.patcher = new Harmony("mods.Infinity.RandomTweaks");
			Main.patcher.PatchAll();
			Loader.modLoader.suscribeOnChangeScene(new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded));
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			
		}

		public void unload()
		{
			throw new NotImplementedException();
		}

		public static Harmony patcher;
	}
}
