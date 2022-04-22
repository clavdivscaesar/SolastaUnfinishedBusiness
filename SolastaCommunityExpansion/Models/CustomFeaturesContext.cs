﻿using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    internal static class ActivePlayerCharacter
    {
        private static GameLocationCharacter _character = null;

        internal static void Set(GameLocationCharacter character = null)
        {
            _character = character;
        }

        internal static GameLocationCharacter Get()
        {
            return _character;
        }
    }

    internal static class CustomFeaturesContext
    {
        internal static CharacterAction CurrentAction { get; set; }

        internal static void RecursiveGrantCustomFeatures(RulesetCharacterHero hero, List<FeatureDefinition> features)
        {
            foreach (FeatureDefinition grantedFeature in features)
            {
                if (grantedFeature is FeatureDefinitionFeatureSet set && set.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                {
                    RecursiveGrantCustomFeatures(hero, set.FeatureSet);
                }
                if (grantedFeature is FeatureDefinitionCustomCode customFeature)
                {
                    customFeature.ApplyFeature(hero);
                }
                if (!(grantedFeature is FeatureDefinitionProficiency featureDefinitionProficiency))
                {
                    continue;
                }
                if (featureDefinitionProficiency.ProficiencyType != RuleDefinitions.ProficiencyType.FightingStyle)
                {
                    continue;
                }
                featureDefinitionProficiency.Proficiencies.ForEach(prof => hero.TrainedFightingStyles.Add(DatabaseRepository.GetDatabase<FightingStyleDefinition>().GetElement(prof, false)));
            }
        }

        internal static void RechargeLinkedPowers(RulesetCharacter character, RuleDefinitions.RestType restType)
        {
            List<FeatureDefinitionPower> pointPoolPowerDefinitions = new List<FeatureDefinitionPower>();
            foreach (RulesetUsablePower usablePower in character.UsablePowers)
            {
                if (usablePower.PowerDefinition is IPowerSharedPool pool)
                {
                    var pointPoolPower = pool.GetUsagePoolPower();

                    // Only add to recharge here if it (recharges on a short rest and this is a short or long rest) or 
                    // it recharges on a long rest and this is a long rest.
                    if (!pointPoolPowerDefinitions.Contains(pointPoolPower)
                        && ((pointPoolPower.RechargeRate == RuleDefinitions.RechargeRate.ShortRest &&
                            (restType == RuleDefinitions.RestType.ShortRest || restType == RuleDefinitions.RestType.LongRest)) ||
                            (pointPoolPower.RechargeRate == RuleDefinitions.RechargeRate.LongRest && restType == RuleDefinitions.RestType.LongRest)))
                    {
                        pointPoolPowerDefinitions.Add(pointPoolPower);
                    }
                }
            }

            // Find the UsablePower of the point pool powers.
            foreach (RulesetUsablePower poolPower in character.UsablePowers)
            {
                if (pointPoolPowerDefinitions.Contains(poolPower.PowerDefinition))
                {
                    int poolSize = GetMaxUsesForPool(poolPower, character);
                    poolPower.SetRemainingUses(poolSize);

                    AssignUsesToSharedPowersForPool(character, poolPower, poolSize, poolSize);
                }
            }
        }

        internal static void AssignUsesToSharedPowersForPool(RulesetCharacter character, RulesetUsablePower poolPower, int remainingUses, int totalUses)
        {
            // Find powers that rely on this pool
            foreach (RulesetUsablePower usablePower in character.UsablePowers)
            {
                if (usablePower.PowerDefinition is IPowerSharedPool pool)
                {
                    FeatureDefinitionPower pointPoolPower = pool.GetUsagePoolPower();
                    if (pointPoolPower == poolPower.PowerDefinition)
                    {
                        usablePower.SetMaxUses(totalUses / usablePower.PowerDefinition.CostPerUse);
                        usablePower.SetRemainingUses(remainingUses / usablePower.PowerDefinition.CostPerUse);
                    }
                }
            }
        }

        internal static int GetMaxUsesForPool(RulesetUsablePower poolPower, RulesetCharacter character)
        {
            int totalPoolSize = poolPower.MaxUses;

            foreach (RulesetUsablePower modifierPower in character.UsablePowers)
            {
                if (modifierPower.PowerDefinition is IPowerPoolModifier modifier && modifier.GetUsagePoolPower() == poolPower.PowerDefinition)
                {
                    totalPoolSize += modifierPower.MaxUses;
                }
            }

            return totalPoolSize;
        }

        internal static void UpdateUsageForPowerPool(this RulesetCharacter character, RulesetUsablePower modifiedPower, int poolUsage)
        {
            if (modifiedPower.PowerDefinition is not IPowerSharedPool sharedPoolPower)
            {
                return;
            }

            var pointPoolPower = sharedPoolPower.GetUsagePoolPower();

            foreach (RulesetUsablePower poolPower in character.UsablePowers)
            {
                if (poolPower.PowerDefinition == pointPoolPower)
                {
                    int maxUses = GetMaxUsesForPool(poolPower, character);
                    int remainingUses = Mathf.Clamp(poolPower.RemainingUses - poolUsage, 0, maxUses);
                    poolPower.SetRemainingUses(remainingUses);
                    AssignUsesToSharedPowersForPool(character, poolPower, remainingUses, maxUses);
                    return;
                }
            }
        }

        public static List<T> FeaturesByType<T>(RulesetActor actor) where T : class
        {
            var list = new List<FeatureDefinition>();
            actor.EnumerateFeaturesToBrowse<T>(list);
            return list.Select(s => s as T).ToList();
        }

        public static EffectDescription ModifySpellEffect(EffectDescription original, RulesetEffectSpell spell)
        {
            var result = original;
            var baseDefinition = spell.SpellDefinition as ICustomMagicEffectBasedOnCaster;
            var caster = spell.Caster;

            if (baseDefinition != null && caster != null)
            {
                result = baseDefinition.GetCustomEffect(caster);
            }

            //TODO: find a way to cache result, so it works faster
            return FeaturesByType<IModifySpellEffect>(caster)
                .Aggregate(result, (current, f) => f.ModifyEffect(spell, current));
        }

        /**Modifies spell description for GUI purposes. Uses only modifiers based on ICustomMagicEffectBasedOnCaster*/
        public static EffectDescription ModifySpellEffectGui(EffectDescription original, GuiSpellDefinition spell)
        {
            var result = original;
            var baseDefinition = spell.SpellDefinition as ICustomMagicEffectBasedOnCaster;
            var caster = ActivePlayerCharacter.Get()?.RulesetCharacter;

            if (baseDefinition != null && caster != null)
            {
                result = baseDefinition.GetCustomEffect(caster);
            }

            return result;
        }
        
        public static EffectDescription AddEffectForms(EffectDescription baseEffect, params EffectForm[] effectForms)
        {
            var newEffect = new EffectDescription();
            newEffect.Copy(baseEffect);
            newEffect.EffectForms.AddRange(effectForms);

            return newEffect;
        }
    }
}
