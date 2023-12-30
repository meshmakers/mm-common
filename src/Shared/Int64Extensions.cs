namespace Meshmakers.Common.Shared;

public static class Int64Extensions
{
    /// <summary>
    /// Converts long value to an int array
    /// </summary>
    /// <param name="value">Long value (e. g. 1234)</param>
    /// <returns>Return array (e. g. {1, 2, 3, 4}</returns>
    public static int[] ToIntArray(this long value)
    {
        if (value == 0)
        {
            return new[] { 0 };
        }
        
        var result = new List<int>();
        while (value != 0)
        {
            var l = value % 10;
            result.Insert(0, (int)l);
            value /= 10;
        }
        return result.ToArray();
    }
}
