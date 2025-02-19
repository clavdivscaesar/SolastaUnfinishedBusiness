﻿using System.Collections;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPhysicalAttackFinishedByMe
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackFinishedByMe(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        RollOutcome rollOutcome,
        int damageAmount);
}
