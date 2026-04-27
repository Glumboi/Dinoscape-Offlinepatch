using HarmonyLib;
using MelonLoader;
using UnityEngine.Networking;
using Il2CppInterop.Runtime;
using System;
using System.Reflection;
using UnityEngineInternal;
using Il2CppPlayFab;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using System.Text;

namespace DinoScapeOffline.Patches
{
    //// Patching UnityWebRequest
    //[HarmonyPatch(typeof(UnityWebRequest))]
    //class UnityWebRequestPatches
    //{
    //    [HarmonyPatch("set_url", new[] { typeof(string) })]
    //    [HarmonyPrefix]
    //    private static bool Prefix_set_url(UnityWebRequest __instance, ref string value)
    //    {

    //        try
    //        {
    //            var originalUri = new System.Uri(value);
    //            MelonLogger.Msg($"Original URI: {value}");

    //            var builder = new System.UriBuilder(originalUri)
    //            {
    //                Scheme = "http",
    //                Host = "localhost",
    //                Port = 8000
    //            };

    //            value = builder.Uri.ToString();
    //            MelonLogger.Msg($"Modified URL: {value}");
    //        }
    //        catch (Exception ex)
    //        {
    //            MelonLogger.Error($"Failed to modify URL: {ex.Message}");
    //        }

    //        return false;
    //    }

    //    [HarmonyPatch("SendWebRequest")]
    //    [HarmonyPostfix]
    //    private static void Postfix_SendWebRequest(UnityWebRequest __instance)
    //    {
    //        MelonLogger.Msg($"SendWebRequest: {__instance.responseCode}");
    //    }


    //}

    //[HarmonyPatch(typeof(WebRequestUtils))]
    //class WebRequestUtilsPatches
    //{
    //    [HarmonyPatch("MakeInitialUrl", new[] { typeof(string), typeof(string) })]
    //    [HarmonyPostfix]
    //    private static void Postfix_MakeInitialUrl(string targetUrl, string localUrl)
    //    {
    //        // Log
    //        MelonLogger.Msg($"MakeInitialUrl: {targetUrl}");
    //        MelonLogger.Msg($"MakeInitialUrl: {localUrl}");
    //    }
    //}


    [HarmonyPatch(typeof(PlayFabApiSettings))]
    class PlayFabApiSettingsPatches
    {
        static StringBuilder requestBuilder = new StringBuilder();
       
        [HarmonyPatch("GetFullUrl", new[] { typeof(string), typeof(Il2CppSystem.Collections.Generic.Dictionary<string, string>) })]
        [HarmonyPrefix]
        private static bool Prefix_GetFullUrl(ref string __result, string apiCall, Il2CppSystem.Collections.Generic.Dictionary<string, string> getParams)
        {
            MelonLogger.Msg($"apiCall: {apiCall}");
            MelonLogger.Msg($"getParams:");
            foreach (var kvp in getParams)
            {
                MelonLogger.Msg($"{kvp.key} : {kvp.value}");
            }
            requestBuilder.Append(DinoScapeOffline.AddressInjector.NewServerAddress);
            requestBuilder.Append(apiCall);
            __result = requestBuilder.ToString();
            requestBuilder.Clear();
            MelonLogger.Msg($"Modifed URL: {__result}");
            return false;
        }
    }
}
