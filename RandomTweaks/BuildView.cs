using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ModLoader;
using SFS.Builds;
using SFS.Input;
using SFS.Parts;
using SFS.Parts.Modules;
using UnityEngine;
using static SFS.Input.KeybindingsPC;

namespace RandomTweaks
{

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

    [HarmonyPatch(typeof(BuildMenus), "Start")]
    public static class AddBuildKeybinds
    {
        [HarmonyPrefix]
        public static void Prefix(ref BuildMenus __instance)
        {
            KeysNode keysNode = BuildManager.main.build_Input.keysNode;
            keysNode.AddOnKeyDown(Key.Ctrl_(KeyCode.UpArrow), MoveSelectedPartsUp);
            keysNode.AddOnKeyDown(Key.Ctrl_(KeyCode.LeftArrow), MoveSelectedPartsLeft);
            keysNode.AddOnKeyDown(Key.Ctrl_(KeyCode.DownArrow), MoveSelectedPartsDown);
            keysNode.AddOnKeyDown(Key.Ctrl_(KeyCode.RightArrow), MoveSelectedPartsRight);
        }

        public static void MoveSelectedPartsLeft()
        {
            Part[] parts = BuildManager.main.holdGrid.selector.selected.ToArray();

            Part_Utility.OffsetPartPosition(new Vector2(-0.5f, 0), true, parts);
        }

        public static void MoveSelectedPartsRight()
        {
            Part[] parts = BuildManager.main.holdGrid.selector.selected.ToArray();

            Part_Utility.OffsetPartPosition(new Vector2(0.5f, 0), true, parts);
        }

        public static void MoveSelectedPartsUp()
        {
            Part[] parts = BuildManager.main.holdGrid.selector.selected.ToArray();

            Part_Utility.OffsetPartPosition(new Vector2(0, 0.5f), true, parts);
        }

        public static void MoveSelectedPartsDown()
        {
            Part[] parts = BuildManager.main.holdGrid.selector.selected.ToArray();

            Part_Utility.OffsetPartPosition(new Vector2(0, -0.5f), true, parts);
        }
    }

}
