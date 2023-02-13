﻿using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class
    MetamagicOptionDefinitionBuilder : DefinitionBuilder<MetamagicOptionDefinition, MetamagicOptionDefinitionBuilder>
{
    protected override void Initialise()
    {
        base.Initialise();
        Definition.metamagicType =
            (RuleDefinitions.MetamagicType)9000; // use a dummy value to avoid conflicts with vanilla
    }

    internal MetamagicOptionDefinitionBuilder SetCost(
        RuleDefinitions.MetamagicCostMethod costMethod = RuleDefinitions.MetamagicCostMethod.FixedValue,
        int sorceryPointsCost = 1)
    {
        Definition.costMethod = costMethod;
        Definition.sorceryPointsCost = sorceryPointsCost;
        return this;
    }

    #region Constructors

    protected MetamagicOptionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected MetamagicOptionDefinitionBuilder(MetamagicOptionDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
