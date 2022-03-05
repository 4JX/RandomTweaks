using System;
using System.Collections.Generic;
using HarmonyLib;
using ModLoader;
using SFS.Builds;
using SFS.Input;
using SFS.IO;
using SFS.Parts;
using SFS.Parts.Modules;
using UnityEngine;
using static SFS.Input.KeybindingsPC;

namespace RandomTweaks
{

    // Makes it so that the outline width for parts shrinks as the camera gets closer
    [HarmonyPatch(typeof(BuildSelector), nameof(BuildSelector.DrawRegionalOutline))]
    public static class SetOutlineWidth
    {
        [HarmonyPrefix]
        public static void DrawRegionalOutline(List<PolygonData> polygons, bool symmetry, Color color, ref float width, float depth = 1f)
        {
            float cameraDistance = BuildManager.main.buildCamera.CameraDistance;
            float newWidth = (width * (cameraDistance * 0.12f));
            width = Math.Min(newWidth, 0.1f);
        }
    }

    // Allow a lot more zooming
    [HarmonyPatch(typeof(BuildManager), "Awake")]
    public static class PatchZoomLimits
    {
        [HarmonyPrefix]
        public static void Prefix(ref BuildManager __instance)
        {
            __instance.buildCamera.maxCameraDistance = 300;
            __instance.buildCamera.minCameraDistance = 0.1f;
        }
    }

    // Bind the custom keybinds to the BuildMenu
    [HarmonyPatch(typeof(BuildMenus), "Start")]
    public static class AddBuildKeybinds
    {
        [HarmonyPrefix]
        public static void Prefix(ref BuildMenus __instance)
        {
            KeysNode keysNode = BuildManager.main.build_Input.keysNode;

            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Move_Parts[0], () => MoveSelectedParts(PartMoveDirection.Up));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Move_Parts[1], () => MoveSelectedParts(PartMoveDirection.Down));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Move_Parts[2], () => MoveSelectedParts(PartMoveDirection.Left));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Move_Parts[3], () => MoveSelectedParts(PartMoveDirection.Right));
        }

        public enum PartMoveDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        public static void MoveSelectedParts(PartMoveDirection direction)
        {
            Part[] parts = BuildManager.main.holdGrid.selector.selected.ToArray();

            switch (direction) {
                case PartMoveDirection.Up:
                    Part_Utility.OffsetPartPosition(new Vector2(0, 0.5f), true, parts);
                    break;
                case PartMoveDirection.Down:
                    Part_Utility.OffsetPartPosition(new Vector2(0, -0.5f), true, parts);
                    break;
                case PartMoveDirection.Left:
                    Part_Utility.OffsetPartPosition(new Vector2(-0.5f, 0), true, parts);
                    break;
                case PartMoveDirection.Right:
                    Part_Utility.OffsetPartPosition(new Vector2(0.5f, 0), true, parts);
                    break;
            }
            
        }

    }

    // Make everything use the same CustomKeybinds
    public static class KeybindsHolder
    {
        public static CustomKeybinds keybinds = new CustomKeybinds();
    }

    // Akin to KeybindingsPC, holds the relevant keys not already included
    public class CustomKeybinds: Settings<CustomKeybinds.DefaultData>
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
            createSpaceTraverse.GetValue();
            createSpaceTraverse.GetValue();
            createTraverse.GetValue(new object[] { defaultData.Placeholder_Key, defaultData.Placeholder_Key, "--Vanilla Keybinds--" });
        }

        public void SaveData()
        {
            Save();
        }

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

}
