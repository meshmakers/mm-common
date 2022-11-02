using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Meshmakers.Common.Shared;

/// <summary>
///     Implementation more independent Random numbers
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
// ReSharper disable once UnusedType.Global
public static class RandomGenerator
{
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
    private static readonly object Locker = new();

    /// <summary>
    ///     Generate new integer number between minValue and maxValue
    /// </summary>
    /// <param name="minValue">Min limit of random number</param>
    /// <param name="maxValue">Max limit of random number</param>
    /// <returns>New random number</returns>
    public static int NextRandom(int minValue, int maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue));
        }

        if (minValue == maxValue)
        {
            return minValue;
        }

        lock (Locker)
        {
            var data = new byte[4];
            Rng.GetBytes(data);

            var generatedValue = Math.Abs(BitConverter.ToInt32(data, 0));

            var range = maxValue - minValue;
            var mod = generatedValue % range;
            var normalizedNumber = minValue + mod;

            return normalizedNumber;
        }
    }
}