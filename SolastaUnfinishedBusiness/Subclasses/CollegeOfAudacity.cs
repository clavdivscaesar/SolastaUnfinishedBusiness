﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfAudacity : AbstractSubclass
{
    private const string Name = "CollegeOfAudacity";
    private const string WhirlMarker = "Whirl";

    private const ActionDefinitions.Id AudaciousWhirlToggle = (ActionDefinitions.Id)ExtraActionId.AudaciousWhirlToggle;
    private const ActionDefinitions.Id MasterfulWhirlToggle = (ActionDefinitions.Id)ExtraActionId.MasterfulWhirlToggle;

    internal CollegeOfAudacity()
    {
        // LEVEL 03

        // Bonus Proficiencies

        var magicAffinityWeaponAsFocus = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetHandsFullCastingModifiers(false, false, true)
            .AddToDB();

        var proficiencyArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Armor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory)
            .AddToDB();

        var proficiencyScimitar = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Scimitar")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Weapon, ScimitarType.Name)
            .AddToDB();

        var featureSetBonusProficiencies = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BonusProficiencies")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(magicAffinityWeaponAsFocus, proficiencyArmor, proficiencyScimitar)
            .AddToDB();

        // Fighting Style

        var proficiencyFightingStyle = FeatureDefinitionFightingStyleChoiceBuilder
            .Create($"FightingStyleChoice{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetFightingStyles("Dueling", "TwoWeapon")
            .AddToDB();

        // AUDACIOUS WHIRL

        // Common

        var actionAffinityAudaciousWhirlToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityAudaciousWhirlToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.AudaciousWhirlToggle)
            .AddToDB();

        var movementAffinityAudaciousWhirl = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}AudaciousWhirl")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(2)
            .AddToDB();

        var conditionAudaciousWhirlExtraMovement = ConditionDefinitionBuilder
            .Create($"Condition{Name}AudaciousWhirlExtraMovement")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetFeatures(movementAffinityAudaciousWhirl)
            .AddToDB();

        // Defensive Whirl

        var attributeModifierDefensiveWhirl = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}DefensiveWhirl")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.ArmorClass)
            .AddToDB();

        var conditionDefensiveWhirl = ConditionDefinitionBuilder
            .Create($"Condition{Name}DefensiveWhirl")
            .SetGuiPresentation($"AttributeModifier{Name}DefensiveWhirl", Category.Feature, Gui.NoLocalization,
                ConditionDefinitions.ConditionMagicallyArmored.GuiPresentation.SpriteReference)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .SetFeatures(attributeModifierDefensiveWhirl)
            .AddToDB();

        var powerDefensiveWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DefensiveWhirl")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Slashing Whirl

        var powerSlashingWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SlashingWhirl")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Mobile Whirl

        var powerMobileWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MobileWhirl")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Audacious Whirl

        var powerAudaciousWhirl = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AudaciousWhirl")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnAttackHitMelee, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .Build())
            .AddToDB();

        powerAudaciousWhirl.SetCustomSubFeatures(
            new CustomBehaviorWhirl(
                conditionAudaciousWhirlExtraMovement,
                conditionDefensiveWhirl,
                powerDefensiveWhirl,
                powerSlashingWhirl,
                powerMobileWhirl),
            ReactionResourceBardicInspiration.Instance,
            new RestrictReactionAttackMode((_, character, _) =>
                character.OnceInMyTurnIsValid(WhirlMarker) &&
                (character.RulesetCharacter.IsToggleEnabled(AudaciousWhirlToggle) ||
                 character.RulesetCharacter.IsToggleEnabled(MasterfulWhirlToggle))));

        PowerBundle.RegisterPowerBundle(
            powerAudaciousWhirl,
            true,
            powerDefensiveWhirl,
            powerSlashingWhirl,
            powerMobileWhirl);

        var featureSetAudaciousWhirl = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AudaciousWhirl")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerDefensiveWhirl,
                powerSlashingWhirl,
                powerMobileWhirl,
                powerAudaciousWhirl,
                actionAffinityAudaciousWhirlToggle)
            .AddToDB();

        // LEVEL 14

        // Masterful Whirl

        var actionAffinityMasterfulWhirlToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityMasterfulWhirlToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.MasterfulWhirlToggle)
            .AddToDB();

        var featureSetMasterfulWhirl = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MasterfulWhirl")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(actionAffinityMasterfulWhirlToggle)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, featureSetBonusProficiencies, proficiencyFightingStyle, featureSetAudaciousWhirl)
            .AddFeaturesAtLevel(6, AttributeModifierCasterFightingExtraAttack, AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(14, featureSetMasterfulWhirl)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorWhirl :
        ISpendPowerFinishedByMe, IAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private readonly ConditionDefinition _conditionDefensiveWhirl;
        private readonly ConditionDefinition _conditionExtraMovement;
        private readonly FeatureDefinitionPower _powerDefensiveWhirl;
        private readonly FeatureDefinitionPower _powerMobileWhirl;
        private readonly FeatureDefinitionPower _powerSlashingWhirl;
        private bool _criticalHit;
        private string _damageType;

        public CustomBehaviorWhirl(
            ConditionDefinition conditionExtraMovement,
            ConditionDefinition conditionDefensiveWhirl,
            FeatureDefinitionPower powerDefensiveWhirl,
            FeatureDefinitionPower powerSlashingWhirl,
            FeatureDefinitionPower powerMobileWhirl)
        {
            _conditionExtraMovement = conditionExtraMovement;
            _conditionDefensiveWhirl = conditionDefensiveWhirl;
            _powerDefensiveWhirl = powerDefensiveWhirl;
            _powerSlashingWhirl = powerSlashingWhirl;
            _powerMobileWhirl = powerMobileWhirl;
        }

        // collect damage type
        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect != null)
            {
                _damageType = null;

                yield break;
            }

            var damageForm = attackMode.EffectDescription.FindFirstDamageForm();

            _damageType = damageForm.damageType;
            _criticalHit = criticalHit;
        }

        // add extra movement on any attack
        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetCharacter.HasAnyConditionOfType(_conditionExtraMovement.Name))
            {
                rulesetCharacter.InflictCondition(
                    _conditionExtraMovement.Name,
                    _conditionExtraMovement.DurationType,
                    _conditionExtraMovement.DurationParameter,
                    _conditionExtraMovement.TurnOccurence,
                    AttributeDefinitions.TagCombat,
                    attacker.RulesetCharacter.guid,
                    attacker.RulesetCharacter.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);
            }
        }

        public IEnumerator OnSpendPowerFinishedByMe(CharacterActionSpendPower action, FeatureDefinitionPower power)
        {
            if (_damageType == null)
            {
                yield break;
            }

            if (power != _powerDefensiveWhirl && power != _powerSlashingWhirl && power != _powerMobileWhirl)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            // targets
            var originalTarget = action.ActionParams.TargetCharacters[0];
            var targetCharacters = new List<GameLocationCharacter>();

            actingCharacter.UsedSpecialFeatures.TryAdd(WhirlMarker, 1);

            var dieType = rulesetCharacter.IsToggleEnabled(MasterfulWhirlToggle)
                ? DieType.D6
                : rulesetCharacter.GetBardicInspirationDieValue();
            var damageRoll = RollDie(dieType, AdvantageType.None, out _, out _);

            // add damage whirl condition and target
            if (power == _powerDefensiveWhirl)
            {
                targetCharacters.Add(originalTarget);

                rulesetCharacter.InflictCondition(
                    _conditionDefensiveWhirl.Name,
                    _conditionDefensiveWhirl.DurationType,
                    _conditionDefensiveWhirl.DurationParameter,
                    _conditionDefensiveWhirl.TurnOccurence,
                    AttributeDefinitions.TagCombat,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    null,
                    damageRoll,
                    0,
                    0);
            }

            // add mobile whirl condition and target
            else if (power == _powerMobileWhirl)
            {
                targetCharacters.Add(originalTarget);

                var conditionDisengaging = ConditionDefinitions.ConditionDisengaging;

                rulesetCharacter.InflictCondition(
                    conditionDisengaging.Name,
                    conditionDisengaging.DurationType,
                    conditionDisengaging.DurationParameter,
                    conditionDisengaging.TurnOccurence,
                    AttributeDefinitions.TagCombat,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);
            }

            // add slashing whirl targets
            else if (power == _powerSlashingWhirl)
            {
                targetCharacters.Add(originalTarget);

                var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

                if (gameLocationBattleService != null)
                {
                    targetCharacters.AddRange(gameLocationBattleService.Battle.EnemyContenders
                        .Where(x => gameLocationBattleService.IsWithin1Cell(actingCharacter, x))
                        .ToList());
                }
            }

            // prepare damage form
            var dices = new List<int> { damageRoll };
            var diceNumber = _criticalHit ? 2 : 1;

            if (diceNumber > 1)
            {
                var criticalDamageRoll = RollDie(dieType, AdvantageType.None, out _, out _);

                damageRoll += criticalDamageRoll;
                dices.Add(criticalDamageRoll);
            }

            // apply damage to targets
            foreach (var targetCharacter in targetCharacters)
            {
                var rulesetDefender = targetCharacter.RulesetCharacter;

                var damageForm = new DamageForm
                {
                    DamageType = _damageType, DieType = dieType, DiceNumber = diceNumber, BonusDamage = 0
                };

                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    _damageType,
                    new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                    rulesetDefender,
                    false,
                    rulesetCharacter.Guid,
                    false,
                    new List<string>(),
                    new RollInfo(dieType, dices, 0),
                    false,
                    out _);
            }

            // consume bardic inspiration if not a masterful whirl
            if (rulesetCharacter.IsToggleEnabled(MasterfulWhirlToggle))
            {
                yield break;
            }

            rulesetCharacter.UsedBardicInspiration++;
            rulesetCharacter.BardicInspirationAltered?.Invoke(
                rulesetCharacter, rulesetCharacter.RemainingBardicInspirations);
        }
    }
}
