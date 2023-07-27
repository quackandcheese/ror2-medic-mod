using System;

namespace MedicMod.Modules
{
    internal static class StaticValues
    {
        internal static string descriptionText = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

        internal const float injectionDamageCoefficient = 5.0f;

        internal const float scalpelDamageCoefficient = 0.75f;

        internal const float bombDamageCoefficient = 16f;

        internal const float healCoefficient = 0.4f;
    }
}