﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.GadgetBlueprints;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameLocationManagerPatcher
{
    //PATCH: EnableSaveByLocation
    [HarmonyPatch(typeof(GameLocationManager), "LoadLocationAsync")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class LoadLocationAsync_Patch
    {
        public static void Prefix(GameLocationManager __instance,
            string locationDefinitionName, string userLocationName, string userCampaignName)
        {
            if (!Main.Settings.EnableSaveByLocation)
            {
                return;
            }

            Main.Log(
                $"LoadLocationAsync-Params: ld={locationDefinitionName}, ul={userLocationName}, uc={userCampaignName}");

            var sessionService = ServiceRepository.GetService<ISessionService>();

            if (sessionService is { Session: { } })
            {
                // Record which campaign/location the latest load game belongs to

#if DEBUG
            var session = sessionService.Session;

            Main.Log(
                $"Campaign-ss: Campaign={session.CampaignDefinitionName}, Location: {session.UserLocationName}");
#endif
                var selectedCampaignService = SaveByLocationContext.ServiceRepositoryEx.GetOrCreateService<SaveByLocationContext.SelectedCampaignService>();


                selectedCampaignService.SetCampaignLocation(userCampaignName, userLocationName);
            }

            __instance.StartCoroutine(ServiceRepository.GetService<IGameSerializationService>()?.EnumerateSavesGames());
        }
    }
    
    //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
    [HarmonyPatch(typeof(GameLocationManager), "ReadyLocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReadyLocation_Patch
    {
        internal static void SetTeleporterGadgetActiveAnimation(WorldGadget worldGadget, bool visibility = false)
        {
            if (worldGadget.UserGadget == null)
            {
                return;
            }

            if (worldGadget.UserGadget.GadgetBlueprint == TeleporterIndividual)
            {
                var visualEffect = worldGadget.transform.FindChildRecursive("Vfx_Teleporter_Individual_Idle_01");

                // NOTE: don't use visualEffect?. which bypasses Unity object lifetime check
                if (visualEffect)
                {
                    visualEffect.gameObject.SetActive(visibility);
                }
            }
            else if (worldGadget.UserGadget.GadgetBlueprint == TeleporterParty)
            {
                var visualEffect = worldGadget.transform.FindChildRecursive("Vfx_Teleporter_Party_Idle_01");

                // NOTE: don't use visualEffect?. which bypasses Unity object lifetime check
                if (visualEffect)
                {
                    visualEffect.gameObject.SetActive(visibility);
                }
            }
        }

        internal static void Postfix(GameLocationManager __instance)
        {
            if (!Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered || Gui.GameLocation.UserLocation == null)
            {
                return;
            }

            var worldGadgets = __instance.WorldLocation.WorldSectors.SelectMany(x => x.WorldGadgets);

            foreach (var worldGadget in worldGadgets)
            {
                SetTeleporterGadgetActiveAnimation(worldGadget);
            }
        }
    }
}
