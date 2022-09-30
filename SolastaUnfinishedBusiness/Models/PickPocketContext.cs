﻿using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static FeatureDefinitionAbilityCheckAffinity;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.LootPackDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.TreasureTableDefinitions;

namespace SolastaUnfinishedBusiness.Models;

public static class PickPocketContext
{
    private static bool _initialized;

    internal static void CreateFeats([NotNull] ICollection<FeatDefinition> feats)
    {
        var abilityCheckAffinityFeatPickPocket = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityFeatLockbreaker,
                "AbilityCheckAffinityFeatPickPocket")
            .SetGuiPresentation("FeatPickPocket", Category.Feat)
            .AddToDB();

        var pickpocketAbilityCheckAffinityGroup = new AbilityCheckAffinityGroup
        {
            abilityScoreName = AttributeDefinitions.Dexterity,
            proficiencyName = SkillDefinitions.SleightOfHand,
            affinity = CharacterAbilityCheckAffinity.Advantage
        };

        abilityCheckAffinityFeatPickPocket.AffinityGroups.SetRange(pickpocketAbilityCheckAffinityGroup);

        var proficiencyFeatPickPocket = FeatureDefinitionProficiencyBuilder
            .Create(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFeatLockbreaker,
                "ProficiencyFeatPickPocket")
            .SetGuiPresentation("FeatPickPocket", Category.Feat)
            .AddToDB();

        proficiencyFeatPickPocket.proficiencyType = ProficiencyType.SkillOrExpertise;
        proficiencyFeatPickPocket.Proficiencies.Clear();
        proficiencyFeatPickPocket.Proficiencies.Add(SkillDefinitions.SleightOfHand);

        var featPickPocket = FeatDefinitionBuilder
            .Create(DatabaseHelper.FeatDefinitions.Lockbreaker, "FeatPickPocket")
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        featPickPocket.Features.SetRange(abilityCheckAffinityFeatPickPocket, proficiencyFeatPickPocket);

        feats.Add(featPickPocket);
    }

    internal static void Load()
    {
        if (!Main.Settings.AddPickPocketableLoot || _initialized)
        {
            return;
        }

        _initialized = true;

        var sleightOfHand = DatabaseHelper.SkillDefinitions.SleightOfHand;
        sleightOfHand.GuiPresentation.unusedInSolastaCOTM = false;

        var pickpocketTableLow = TreasureTableDefinitionBuilder
            .Create(RandomTreasureTableE2_Mundane_Ingredients, "PickPocketTableLow")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
            .AddToDB();

        var pickpocketTableMed = TreasureTableDefinitionBuilder
            .Create(RandomTreasureTableE_Ingredients, "PickPocketTableMed")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(RandomTreasureTableB_Consumables.TreasureOptions)
            .AddTreasureOptions(RandomTreasureTableA_Gem.TreasureOptions)
            .SetGuiPresentationNoContent()
            .AddToDB();

        var pickpocketTableUndead = TreasureTableDefinitionBuilder
            .Create("PickPocketTableUndead")
            .SetGuiPresentationNoContent()
            .AddTreasureOptions(RandomTreasureTableE_Ingredients.TreasureOptions[3])
            .AddTreasureOptions(RandomTreasureTableE_Ingredients.TreasureOptions[9])
            .AddTreasureOptions(RandomTreasureTableE_Ingredients.TreasureOptions[16])
            .AddToDB();

        var lootPickpocketTableLow = new ItemOccurence
        {
            itemMode = ItemOccurence.SelectionMode.TreasureTable,
            treasureTableDefinition = pickpocketTableLow,
            diceNumber = 1,
            diceType = DieType.D1,
            additiveModifier = 0
        };

        var lootPickpocketTableMed = new ItemOccurence
        {
            itemMode = ItemOccurence.SelectionMode.TreasureTable,
            treasureTableDefinition = pickpocketTableMed,
            diceNumber = 1,
            diceType = DieType.D1,
            additiveModifier = 0
        };

        var lootPickpocketTableUndead = new ItemOccurence
        {
            itemMode = ItemOccurence.SelectionMode.TreasureTable,
            treasureTableDefinition = pickpocketTableUndead,
            diceNumber = 1,
            diceType = DieType.D1,
            additiveModifier = 0
        };

        var pickPocketableLootA = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_LowMoney, "CE_PickpocketableLoot_A")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow)
            .AddToDB();

        var pickPocketableLootB = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_B")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow)
            .AddToDB();

        var pickPocketableLootC = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_C")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableMed)
            .AddToDB();

        var pickPocketableLootD = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_MedMoney, "CE_PickpocketableLoot_D")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableLow, lootPickpocketTableMed)
            .AddToDB();

        var pickPocketableLootUndead = LootPackDefinitionBuilder
            .Create(Pickpocket_generic_loot_LowMoney, "CE_PickpocketableLoot_Undead")
            .SetGuiPresentationNoContent()
            .SetItemOccurrencesList(lootPickpocketTableUndead)
            .AddToDB();

        foreach (var monster in DatabaseRepository.GetDatabase<MonsterDefinition>())
        {
            switch (monster.CharacterFamily)
            {
                case "Humanoid" when monster.DefaultFaction == "HostileMonsters" &&
                                     monster.StealableLootDefinition == null:
                {
                    if (monster.ChallengeRating < 1.0)
                    {
                        monster.stealableLootDefinition = Pickpocket_generic_loot_LowMoney;
                    }

                    if (monster.ChallengeRating > 0.9 &&
                        monster.ChallengeRating < 2.0)
                    {
                        monster.stealableLootDefinition = pickPocketableLootA;
                    }

                    if (monster.ChallengeRating > 1.9 &&
                        monster.ChallengeRating < 3.0)
                    {
                        monster.stealableLootDefinition = pickPocketableLootB;
                    }

                    if (monster.ChallengeRating > 2.9 &&
                        monster.ChallengeRating < 5.0)
                    {
                        monster.stealableLootDefinition = pickPocketableLootC;
                    }

                    if (monster.ChallengeRating > 4.9)
                    {
                        monster.stealableLootDefinition = pickPocketableLootD;
                    }

                    break;
                }
                case "Undead" when monster.DefaultFaction.Contains("HostileMonsters") &&
                                   monster.StealableLootDefinition == null && !monster.Name.Contains("Ghost") &&
                                   !monster.Name.Contains("Spectral") && !monster.Name.Contains("Servant"):
                    monster.stealableLootDefinition = pickPocketableLootUndead;
                    break;
            }
        }
    }
}
