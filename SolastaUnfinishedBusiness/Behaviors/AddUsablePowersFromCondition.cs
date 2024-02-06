﻿using System.Linq;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Behaviors;

/**
 * Adds all powers from condition definition's feature list to character's usable powers when condition is applied
 * and removes them when condition is removed.
 */
public class AddUsablePowersFromCondition : IOnConditionAddedOrRemoved
{
    internal static AddUsablePowersFromCondition Marker { get; } = new();

    public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
    {
        foreach (var power in rulesetCondition.ConditionDefinition.Features
                     .OfType<FeatureDefinitionPower>())
        {
            if (target.UsablePowers.Any(u => u.PowerDefinition == power))
            {
                continue;
            }

            target.UsablePowers.Add(PowerProvider.Get(power, target));
        }
    }

    public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
    {
        var powers = rulesetCondition.ConditionDefinition.Features
            .OfType<FeatureDefinitionPower>()
            .ToList();

        target.UsablePowers.RemoveAll(usablePower => powers.Contains(usablePower.PowerDefinition));
    }
}
