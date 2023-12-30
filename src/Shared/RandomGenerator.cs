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

    /// <summary>
    /// Creates a short unique string based on the current time
    /// </summary>
    /// <returns></returns>
    public static string GenerateUniqueString()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var time = DateTime.Now.ToUnixEpochInMilliSecondsTime() - new DateTime(2022, 11, 15).ToUnixEpochInMilliSecondsTime();
        var part1 = time.ToIntArray().Select(v => chars[v]);
        
        var random = new Random();
        var part2 = Enumerable.Repeat(chars, 4)
            .Select(s => s[random.Next(s.Length)]);
        
        var result = new string(part1.Concat(part2).ToArray());
        return result;
    }
}
