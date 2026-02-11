using System.Text;

namespace Shared.Kernel.Extensions;

public static class StampExtensions
{
    private static readonly Random _random = new();
    private static readonly object _lock = new();

    public static string GenerateStamp(this int size, bool lowerCase = false)
    {
        lock (_lock)
        {
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return builder.ToString();
        }
    }

    public static string GenerateStamp()
    {
        return DateTime.UtcNow.ToString("yyyyMMddHHmmss") + 8.GenerateStamp();
    }

    public static string GenerateStamp(this int randomSize)
    {
        return DateTime.UtcNow.ToString("yyyyMMddHHmmss") + randomSize.GenerateStamp();
    }

    public static string GenerateRequestId(string prefix = "PHCAPI")
    {
        var uuid = Guid.NewGuid().ToString();
        return $"{prefix}{uuid[..21]}";
    }

    public static string GenerateCustomStamp(string prefix, int randomSize = 8, bool includeTimestamp = true)
    {
        var stamp = new StringBuilder();

        if (!string.IsNullOrEmpty(prefix))
            stamp.Append(prefix);

        if (includeTimestamp)
            stamp.Append(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

        if (randomSize > 0)
            stamp.Append(randomSize.GenerateStamp());

        return stamp.ToString();
    }
}
