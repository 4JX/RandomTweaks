using HarmonyLib;
using SFS.Input;
using SFS.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SFS.Input.KeybindingsPC;

namespace RandomTweaks
{
    // Make everything use the same CustomKeybinds
    public static class KeybindsHolder
    {
        public static CustomKeybinds keybinds = new CustomKeybinds();
    }

    // Save hook for... Saving
    [HarmonyPatch(typeof(Settings<Data>), "Save")]
    public class KeybindsSaveHook
    {
        [HarmonyPostfix]
        public static void SaveHook(Settings<Data> __instance)
        {
            // This works. I don't know why. At least it sucessfully prevents infinite loops
            if (__instance is KeybindingsPC)
            {
                KeybindsHolder.keybinds.SaveData();
            }
        }
    }

    // Awake hook to add the menu items
    [HarmonyPatch(typeof(KeybindingsPC), "Awake")]
    public class KeybindsAwakeHook
    {
        [HarmonyPrefix]
        public static void Prefix(KeybindingsPC __instance)
        {
            KeybindsHolder.keybinds.AwakeHook(__instance);
        }
    }

    // Akin to KeybindingsPC, holds the relevant keys not already included
    public class CustomKeybinds : Settings<CustomKeybinds.DefaultData>
    {
        protected override string FileName => "RandomTweaksKeybinds";

        public static DefaultData custom_keys = new DefaultData();
        public class DefaultData
        {
            public Key Placeholder_Key = KeyCode.Hash;

            public Key[] Move_Parts = new Key[4]
            {
                KeyCode.UpArrow,
                KeyCode.DownArrow,
                KeyCode.LeftArrow,
                KeyCode.RightArrow
            };

            public Key[] Resize_Parts = new Key[4]
            {
                Key.Ctrl_(KeyCode.UpArrow),
                Key.Ctrl_(KeyCode.DownArrow),
                Key.Ctrl_(KeyCode.LeftArrow),
                Key.Ctrl_(KeyCode.RightArrow)
            };

            public Key Toggle_New_Build_System = KeyCode.N;

        }

        // Loads automatically(?), no need for a hook here
        protected override void OnLoad()
        {
            custom_keys = settings;
        }

        public void AwakeHook(KeybindingsPC __instance)
        {

            // Load the saved and default keys
            Load();
            DefaultData defaultData = new DefaultData();

            // Reflection needed since these are private
            Traverse createTraverse = Traverse.Create(__instance).Method("Create", new object[] { custom_keys.Move_Parts[0], defaultData.Move_Parts[0], "Move_Selected_Up" });
            Traverse createSpaceTraverse = Traverse.Create(__instance).Method("CreateSpace");

            // Finally actually call the code
            createTraverse.GetValue(new object[] { defaultData.Placeholder_Key, defaultData.Placeholder_Key, "--RandomTweaks Keybinds--" });
            createTraverse.GetValue(new object[] { custom_keys.Move_Parts[0], defaultData.Move_Parts[0], "Move_Selected_Up" });
            createTraverse.GetValue(new object[] { custom_keys.Move_Parts[1], defaultData.Move_Parts[1], "Move_Selected_Down" });
            createTraverse.GetValue(new object[] { custom_keys.Move_Parts[2], defaultData.Move_Parts[2], "Move_Selected_Left" });
            createTraverse.GetValue(new object[] { custom_keys.Move_Parts[3], defaultData.Move_Parts[3], "Move_Selected_Right" });
            createSpaceTraverse.GetValue();
            createTraverse.GetValue(new object[] { custom_keys.Resize_Parts[0], defaultData.Resize_Parts[0], "Increase_Part_Height" });
            createTraverse.GetValue(new object[] { custom_keys.Resize_Parts[1], defaultData.Resize_Parts[1], "Decrease_Part_Height" });
            createTraverse.GetValue(new object[] { custom_keys.Resize_Parts[2], defaultData.Resize_Parts[2], "Increase_Part_Width" });
            createTraverse.GetValue(new object[] { custom_keys.Resize_Parts[3], defaultData.Resize_Parts[3], "Decrease_Part_Width" });
            createSpaceTraverse.GetValue();
            createTraverse.GetValue(new object[] { custom_keys.Toggle_New_Build_System, defaultData.Toggle_New_Build_System, "Toggle_New_Build_Mode" });
            createSpaceTraverse.GetValue();
            createSpaceTraverse.GetValue();
            createSpaceTraverse.GetValue();
        }

        public void SaveData()
        {
            Save();
        }

    }
}
