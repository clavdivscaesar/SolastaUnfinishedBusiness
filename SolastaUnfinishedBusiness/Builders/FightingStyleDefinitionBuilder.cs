﻿using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders;

internal abstract class
    FightingStyleDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FightingStyleDefinition
    where TBuilder : FightingStyleDefinitionBuilder<TDefinition, TBuilder>
{
    internal TBuilder SetFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.SetRange(features.OrderBy(f => f.Name));
        Definition.Features.Sort(Sorting.Compare);
        return This();
    }

    #region Constructors

    protected FightingStyleDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FightingStyleDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FightingStyleDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected FightingStyleDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
