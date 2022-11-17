﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

public static class GuiFeatDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiFeatDefinition), "IsFeatMatchingPrerequisites")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class IsFeatMatchingPrerequisites_Patch
    {
        public static void Postfix(
            ref bool __result,
            FeatDefinition feat,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            //PATCH: Enforces Feats With PreRequisites
            if (feat is not FeatDefinitionWithPrerequisites featDefinitionWithPrerequisites
                || featDefinitionWithPrerequisites.Validators.Count == 0)
            {
                return;
            }

            var (result, output) = featDefinitionWithPrerequisites.Validate(featDefinitionWithPrerequisites, hero);

            __result = __result && result;
            prerequisiteOutput += '\n' + output;
        }

        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Replace call to RulesetCharacterHero.SpellRepertoires.Count with Count list of FeatureCastSpell
            //which are registered before feat selection at lvl 1
            return instructions
                .ReplaceCall(typeof(RulesetCharacter).GetMethod("get_SpellRepertoires"),
                    1,
                    1, "GuiFeatDefinition.IsFeatMatchingPrerequisites",
                    new CodeInstruction(OpCodes.Call,
                        new Func<RulesetCharacterHero, int>(CanCastSpells).Method))

                // PATCH: Remove asserts in DEBUG build
                .RemoveBoolAsserts();
        }

        private static int CanCastSpells([NotNull] RulesetCharacterHero hero)
        {
            return hero.EnumerateFeaturesToBrowse<FeatureDefinitionCastSpell>().Count;
        }
    }

    [HarmonyPatch(typeof(GuiFeatDefinition), "Subtitle", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Subtitle_Patch
    {
        public static bool Prefix(GuiFeatDefinition __instance, ref string __result)
        {
            //PATCH: use 'Feat Group' as subtitle for feats that are feat groups
            if (__instance.FeatDefinition.HasSubFeatureOfType<GroupedFeat>())
            {
                __result = "Tooltip/&FeatGroupTitle";
                return false;
            }

            return true;
        }
    }
}
