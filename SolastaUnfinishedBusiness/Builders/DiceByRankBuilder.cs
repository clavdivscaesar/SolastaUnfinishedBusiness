﻿using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Builders;

internal static class DiceByRankBuilder
{
    internal static DiceByRank BuildDiceByRank(int rank, int dice)
    {
        var diceByRank = new DiceByRank { rank = rank, diceNumber = dice };
        return diceByRank;
    }
}

internal static class DiceByRankMaker
{
    // internal static List<DiceByRank> Make(params (int, int)[] ranks)
    // {
    //     return ranks
    //         .Select(rank => new DiceByRank().SetRank(rank.Item1).SetDices(rank.Item2))
    //         .ToList();
    // }

    internal static List<DiceByRank> MakeBySteps(int start = 0, int increment = 1, int step = 0)
    {
        var result = new List<DiceByRank>();
        for (var i = 0; i < 20; i++)
        {
            result.Add(new DiceByRank().SetRank(i).SetDices(start + ((i + 1) / (step + 1) * increment)));
        }

        return result;
    }

    private static DiceByRank SetRank(this DiceByRank instance, int rank)
    {
        instance.rank = rank;
        return instance;
    }

    private static DiceByRank SetDices(this DiceByRank instance, int dices)
    {
        instance.diceNumber = dices;
        return instance;
    }
}
