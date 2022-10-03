﻿using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishConArtist : AbstractSubclass
{
    internal const string Name = "RoguishConArtist";

    internal RoguishConArtist()
    {
        var abilityCheckAffinityConArtist = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityConArtist")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, RuleDefinitions.DieType.D8, 0,
                (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
                (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
                (AttributeDefinitions.Charisma, SkillDefinitions.Performance))
            .AddToDB();

        var castSpellConArtist = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellConArtist")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .AddRestrictedSchools(SchoolConjuration, SchoolTransmutation, SchoolEnchantment, SchoolIllusion)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetReplacedSpells(4, 1)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetKnownSpells(4, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .AddToDB();

        var conditionConArtistFeint = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionConArtistFeint")
            .SetGuiPresentation(Category.Feature,
                ConditionDefinitions.ConditionTrueStrike.GuiPresentation.SpriteReference)
            .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.Attacked)
            .SetAdditionalDamageData(RuleDefinitions.DieType.D8, 3, ConditionDefinition.DamageQuantity.Dice, true)
            .AddToDB();

        var powerConArtistFeint = FeatureDefinitionPowerBuilder
            .Create("PowerConArtistFeint")
            .SetGuiPresentation(Category.Feature)
            .Configure(
                0,
                RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.BonusAction,
                0,
                RuleDefinitions.RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Charisma,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Distance, 12,
                        RuleDefinitions.TargetType.Individuals, 1, 0)
                    .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .SetSavingThrowData(
                        true, false, AttributeDefinitions.Wisdom, true,
                        RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Charisma,
                        15)
                    .AddEffectForm(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(
                                conditionConArtistFeint, ConditionForm.ConditionOperation.Add, false, false)
                            .Build())
                    .Build())
            .AddToDB();

        var magicAffinityConArtistDc = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityConArtistDC")
            .SetGuiPresentation(Category.Feature)
            .SetCastingModifiers(0, RuleDefinitions.SpellParamsModifierType.None, 3)
            .AddToDB();

        var proficiencyConArtistMentalSavingThrows = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyConArtistMentalSavingThrows")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.SavingThrow,
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Wisdom)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, DomainInsight.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                abilityCheckAffinityConArtist,
                castSpellConArtist)
            .AddFeaturesAtLevel(9,
                powerConArtistFeint)
            .AddFeaturesAtLevel(13,
                magicAffinityConArtistDc)
            .AddFeaturesAtLevel(17,
                proficiencyConArtistMentalSavingThrows)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
}
