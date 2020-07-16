using System;

namespace avalonia_animation
{
    /// <summary>
    /// Exposes some auxiliary functionality.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="rad">The radian value.</param>
        /// <returns>The converted degree value.</returns>
        public static double RadToDeg(double rad) => rad * (180.0 / Math.PI);
    }
}