using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
using System.CodeDom.Compiler;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(LootableContainerDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class LootableContainerDefinitionExtensions
    {
        public static T SetContentAlwaysKnown<T>(this T entity, System.Boolean value)
            where T : LootableContainerDefinition
        {
            entity.SetField("contentAlwaysKnown", value);
            return entity;
        }

        public static T SetForbidDrop<T>(this T entity, System.Boolean value)
            where T : LootableContainerDefinition
        {
            entity.SetField("forbidDrop", value);
            return entity;
        }

        public static T SetIgnoredByScavengers<T>(this T entity, System.Boolean value)
            where T : LootableContainerDefinition
        {
            entity.SetField("ignoredByScavengers", value);
            return entity;
        }

        public static T SetMinSlotsNumber<T>(this T entity, System.Int32 value)
            where T : LootableContainerDefinition
        {
            entity.SetField("minSlotsNumber", value);
            return entity;
        }
    }
}