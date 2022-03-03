using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ModLoader;
using SFS.Builds;
using SFS.Parts.Modules;
using UnityEngine;

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
        public static void DrawRegionalOutline(ref BuildManager __instance)
        {
            __instance.buildCamera.maxCameraDistance = 300;
            __instance.buildCamera.minCameraDistance = 0.1f;
        }
    }

}
