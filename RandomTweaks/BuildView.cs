using HarmonyLib;
using SFS.Builds;
using SFS.Input;
using SFS.IO;
using SFS.Parts;
using SFS.Parts.Modules;
using SFS.UI;
using System;
using System.Collections.Generic;
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

            // For moving
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Move_Parts[0], () => MoveSelectedParts(PartMoveDirection.Up));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Move_Parts[1], () => MoveSelectedParts(PartMoveDirection.Down));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Move_Parts[2], () => MoveSelectedParts(PartMoveDirection.Left));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Move_Parts[3], () => MoveSelectedParts(PartMoveDirection.Right));

            // For resizing
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Resize_Parts[2], () => ResizeSelectedParts(PartResizeType.IncreaseWidth));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Resize_Parts[3], () => ResizeSelectedParts(PartResizeType.DecreaseWidth));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Resize_Parts[0], () => ResizeSelectedParts(PartResizeType.IncreaseHeight));
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Resize_Parts[1], () => ResizeSelectedParts(PartResizeType.DecreaseHeight));

            // New build mode
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Toggle_New_Build_System, () => NewBuildSystemHook.ToggleNewBuild());
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

            if (Input.GetKey(KeyCode.LeftShift))
            {
                switch (direction)
                {
                    case PartMoveDirection.Up:
                        Part_Utility.OffsetPartPosition(new Vector2(0, 0.1f), false, parts);
                        break;
                    case PartMoveDirection.Down:
                        Part_Utility.OffsetPartPosition(new Vector2(0, -0.1f), false, parts);
                        break;
                    case PartMoveDirection.Left:
                        Part_Utility.OffsetPartPosition(new Vector2(-0.1f, 0), false, parts);
                        break;
                    case PartMoveDirection.Right:
                        Part_Utility.OffsetPartPosition(new Vector2(0.1f, 0), false, parts);
                        break;
                }
            }
            else
            {
                switch (direction)
                {
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

        public enum PartResizeType
        {
            IncreaseWidth,
            DecreaseWidth,
            IncreaseHeight,
            DecreaseHeight
        }

        public static void ResizeSelectedParts(PartResizeType resize_type)
        {
            Part[] parts = BuildManager.main.holdGrid.selector.selected.ToArray();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                switch (resize_type)
                {
                    case PartResizeType.IncreaseWidth:
                        ResizeParts(new Vector3(0.1f, 0, 0), parts);
                        break;
                    case PartResizeType.DecreaseWidth:
                        ResizeParts(new Vector3(-0.1f, 0, 0), parts);
                        break;
                    case PartResizeType.IncreaseHeight:
                        ResizeParts(new Vector3(0, 0.1f, 0), parts);
                        break;
                    case PartResizeType.DecreaseHeight:
                        ResizeParts(new Vector3(0, -0.1f, 0), parts);
                        break;
                }
            }
            else
            {
                switch (resize_type)
                {
                    case PartResizeType.IncreaseWidth:
                        ResizeParts(new Vector3(0.5f, 0, 0), parts);
                        break;
                    case PartResizeType.DecreaseWidth:
                        ResizeParts(new Vector3(-0.5f, 0, 0), parts);
                        break;
                    case PartResizeType.IncreaseHeight:
                        ResizeParts(new Vector3(0, 0.5f, 0), parts);
                        break;
                    case PartResizeType.DecreaseHeight:
                        ResizeParts(new Vector3(0, -0.5f, 0), parts);
                        break;
                }
            }

        }

        static void ResizeParts(Vector3 resizeAmount, Part[] parts)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                double width_original = parts[i].variablesModule.doubleVariables.GetValue("width_original");
                double width_upper = parts[i].variablesModule.doubleVariables.GetValue("width_a");
                double width_lower = parts[i].variablesModule.doubleVariables.GetValue("width_b");
                double height = parts[i].variablesModule.doubleVariables.GetValue("height");

                double new_width_upper = width_upper + resizeAmount.x;
                double new_width_lower = width_lower + resizeAmount.x;
                // Loosely preserve the final size if the sizes are not equal
                double new_width_original = Math.Min(new_width_upper, new_width_lower);
                double new_height = height + resizeAmount.y;

                parts[i].variablesModule.doubleVariables.SetValue("width_original", new_width_original);
                parts[i].variablesModule.doubleVariables.SetValue("width_a", new_width_upper);
                parts[i].variablesModule.doubleVariables.SetValue("width_b", new_width_lower);
                parts[i].variablesModule.doubleVariables.SetValue("height", new_height);
            }
        }

    }

    // Rotate on small increments if shift is held (Feature parity with the above)
    [HarmonyPatch(typeof(BuildMenus), nameof(BuildMenus.Rotate))]
    public static class RotatePatch
    {
        [HarmonyPrefix]
        public static void Prefix(ref float rotation)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Only affect 90º Rotations
                if (rotation == 90f)
                {
                    rotation = 15f;

                }
                else if (rotation == -90f)
                {
                    rotation = -15f;
                }
            }
        }
    }


    [HarmonyPatch(typeof(DevSettings), "DisableNewBuild", MethodType.Getter)]
    static class DevSettingsPatches
    {
        public static bool disableNewBuild = true;

        [HarmonyPostfix]
        static void Postfix(ref bool __result)
        {
            __result = disableNewBuild;
        }
    }

    // This is needed since the original method also checks for the enviorement being an editor
    [HarmonyPatch(typeof(HoldGrid), "Update")]
    public static class NewBuildSystemHook
    {


        [HarmonyPrefix]
        public static bool Prefix(HoldGrid __instance)
        {
            if (!DevSettings.DisableNewBuild)
            {
                // Reflection time
                Traverse.Create(__instance).Method("GetSnapPosition_New", new object[] { __instance.position }).GetValue();
            }

            // Do not let the original method run
            return false;
        }

        public static void ToggleNewBuild()
        {
            DevSettingsPatches.disableNewBuild = !DevSettingsPatches.disableNewBuild;
            MsgDrawer.main.Log("New build mode set to: " + (!DevSettingsPatches.disableNewBuild).ToString());
        }

    }

}


