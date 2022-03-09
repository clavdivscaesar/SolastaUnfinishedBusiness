﻿using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionSummoningAffinityBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionSummoningAffinity, FeatureDefinitionSummoningAffinityBuilder>
    {
        protected FeatureDefinitionSummoningAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        protected FeatureDefinitionSummoningAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        protected FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionSummoningAffinityBuilder ClearEffectForms()
        {
            Definition.ClearEffectForms();
            return this;
        }

        public FeatureDefinitionSummoningAffinityBuilder SetRequiredMonsterTag(string tag)
        {
            Definition.SetRequiredMonsterTag(tag);
            return this;
        }

        public FeatureDefinitionSummoningAffinityBuilder SetAddedConditions(params ConditionDefinition[] value)
        {
            SetAddedConditions(value.AsEnumerable());
            return this;
        }

        public FeatureDefinitionSummoningAffinityBuilder SetAddedConditions(IEnumerable<ConditionDefinition> value)
        {
            Definition.SetAddedConditions(value);
            return this;
        }
    }
}
