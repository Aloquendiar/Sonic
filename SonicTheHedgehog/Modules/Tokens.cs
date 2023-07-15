﻿using R2API;
using System;

namespace SonicTheHedgehog.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region SonicTheHedgehog
            string prefix = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_";

            string desc = "Sonic is a fast melee fighter who specializes in movement and single target damage.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Melee is your go-to tool for just about any situation. Use Sonic's close range attacks for a consistent rush of damage. Use homing attacks to close the gap or quickly take down weak enemies." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sonic boom is a fast barrage of projectiles. All charges refill at the same time, so you can quickly keep attacking." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Boost lets you move significantly faster than normal. Use it to dodge attacks or traverse the map." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Grand Slam is a powerful attack that closes in on an enemy and does major single target damage." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for finished flavor text.";
            string outroFailure = "..and so he vanished, forever without finished flavor text.";

            LanguageAPI.Add(prefix + "NAME", "Sonic");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Fastest Thing Alive");
            LanguageAPI.Add(prefix + "LORE", "maybe one day there will be a logbook entry");
            //lore idea: something about ancients getting scooped up by providence before their planet got blown up, drew chaos emeralds to ror planet
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            string superSonicColor = "<color=#ffee00>";
            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Sonic passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "I got plans for super forms and chaos emeralds and stuff that'll be a part of Sonic's passive, but I probably won't get to that for a long time :)");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Melee");
            string meleeDescription = $"Melee nearby enemies dealing <style=cIsDamage>{100f * StaticValues.meleeDamageCoefficient}% damage</style>. Every 5th hit deals <style=cIsDamage>{100f * StaticValues.finalMeleeDamageCoefficient}% damage</style>. Homing attack distant enemies to deal <style=cIsDamage>{100f * StaticValues.homingAttackDamageCoefficient}% damage</style>.";
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", meleeDescription);
            #endregion

            #region Super Primary
            LanguageAPI.Add(prefix + "SUPER_PRIMARY_SLASH_NAME", "Super Melee");
            LanguageAPI.Add(prefix + "SUPER_PRIMARY_SLASH_DESCRIPTION", meleeDescription+$" {superSonicColor}Every close range attack fires a projectile dealing {(100f * StaticValues.superMeleeExtraDamagePercent)}% base damage.</color>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SONIC_BOOM_NAME", "Sonic Boom");
            LanguageAPI.Add(prefix + "SECONDARY_SONIC_BOOM_DESCRIPTION", $"Fire shockwaves dealing <style=cIsDamage>{Modules.StaticValues.sonicBoomCount}x{100f * StaticValues.sonicBoomDamageCoefficient}% damage</style>.");
            #endregion

            #region Super Secondary
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_SONIC_BOOM_NAME", "Cross Slash");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_SONIC_BOOM_DESCRIPTION", $"Fire shockwaves dealing <style=cIsDamage>{Modules.StaticValues.sonicBoomCount}x{superSonicColor}{100f * StaticValues.superSonicBoomDamageCoefficient}</color><style=cIsDamage>% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_BOOST_NAME", "Boost");
            LanguageAPI.Add(prefix + "UTILITY_BOOST_DESCRIPTION", $"Spend boost meter to move at speeds <style=cIsUtility>{(100f * StaticValues.boostSpeedCoefficient)-100}%</style> faster than normal. If health is near full, sprint <style=cIsUtility>{(100f * StaticValues.powerBoostSpeedCoefficient) - 100}%</style> faster instead. Can be used in the air for a short mid-air dash.");
            #endregion

            #region Super Utility
            LanguageAPI.Add(prefix + "SUPER_UTILITY_BOOST_NAME", "Super Boost");
            LanguageAPI.Add(prefix + "SUPER_UTILITY_BOOST_DESCRIPTION", $"Move at speeds {superSonicColor}{(100f * StaticValues.superBoostSpeedCoefficient) - 100}%</color> faster than normal.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Grand Slam");
            string grandSlamDescription = $"Dash forward into an enemy to attack with <style=cIsDamage>{100f * StaticValues.grandSlamDashDamageCoefficient}% damage</style> repeatedly before unleashing a powerful attack from above dealing <style=cIsDamage>{100f * StaticValues.grandSlamFinalDamageCoefficient}% damage</style>.";
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", grandSlamDescription);
            #endregion

            #region Super Special
            LanguageAPI.Add(prefix + "SUPER_SPECIAL_BOMB_NAME", "Super Grand Slam");
            LanguageAPI.Add(prefix + "SUPER_SPECIAL_BOMB_DESCRIPTION", grandSlamDescription + $"{superSonicColor} Create afterimages that rain down for {100f * (Modules.StaticValues.superGrandSlamDOTDamage*2)}% damage per second.</color>");
            #endregion

            #region Special #2
            LanguageAPI.Add(prefix + "SPECIAL_SUPER_TRANSFORMATION_NAME", $"{superSonicColor}Super Sonic</color>");
            LanguageAPI.Add(prefix + "SPECIAL_SUPER_TRANSFORMATION_DESCRIPTION", $"Transform into {superSonicColor}Super Sonic</color> for {Modules.StaticValues.superSonicDuration} seconds. {superSonicColor}Upgrades all of your skills</color>. Increases <style=cIsDamage>damage</style> by <style=cIsDamage>+{(100f * StaticValues.superSonicBaseDamage) - 100}%</style>. Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>+{(100f * StaticValues.superSonicAttackSpeed) - 100}%</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>+{(100f * StaticValues.superSonicMovementSpeed) - 100}%</style>. Increases <style=cIsUtility>jump height</style> by <style=cIsUtility>+{(100f * StaticValues.superSonicJumpHeight) - 100}%</style>. Gain <style=cIsHealing>complete invincibility</style>." + Environment.NewLine + Environment.NewLine + "This can only be used once per stage.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Sonic: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Sonic, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Sonic: Mastery");
            #endregion
            #endregion
        }
    }
}