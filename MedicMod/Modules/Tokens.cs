using R2API;
using System;

namespace MedicMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Medic
            string prefix = MedicPlugin.DEVELOPER_PREFIX + "_MEDIC_BODY_";

            string desc = "Medic is a highly skilled combatant who utilizes a variety of medical tools to support her allies and dispatch her enemies.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Scalpels are precise and deadly, allowing the Medic to dispatch foes with accuracy." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Lethal Injection is a close-range attack that injects a potent drug into enemies, dealing substantial damage over time." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Gauze Grapple enables the Medic to throw a gauze at targets, allowing them to swiftly close the distance or reposition strategically." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > First Aid activates a gradual healing effect, replenishing 100 HP over time for the Medic or nearby allies." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, seeking new lives to save.";
            string outroFailure = "..and so she vanished, forever haunted by the lives she couldn't save.";


            LanguageAPI.Add(prefix + "NAME", "Medic");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Healer");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Henry passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SCALPEL_NAME", "Scalpels");
            LanguageAPI.Add(prefix + "PRIMARY_SCALPEL_DESCRIPTION", Helpers.agilePrefix + $"Unleash a burst of four scalpels with pinpoint accuracy, dealing <style=cIsDamage>{100f * StaticValues.scalpelDamageCoefficient}% damage</style> per scalpel.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_INJECTION_NAME", "Lethal Injection");
            LanguageAPI.Add(prefix + "SECONDARY_INJECTION_DESCRIPTION", Helpers.agilePrefix + $"Deliver a powerful melee attack by injecting a lethal drug, dealing <style=cIsDamage>{100f * StaticValues.gunDamageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_GRAPPLE_NAME", "Gauze Grapple");
            LanguageAPI.Add(prefix + "UTILITY_GRAPPLE_DESCRIPTION", "Throw a gauze at an enemy or ally to pull yourself towards them. <style=cIsUtility>Unlimited range.</style>");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_HEAL_NAME", "First Aid");
            LanguageAPI.Add(prefix + "SPECIAL_HEAL_DESCRIPTION", $"Apply first aid to yourself or an ally, <style=cIsHealing>healing 100 HP</style>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Medic: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Medic, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Medic: Mastery");
            #endregion
            #endregion
        }
    }
}