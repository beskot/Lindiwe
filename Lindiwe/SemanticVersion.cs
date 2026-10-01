namespace Lindiwe;

public readonly record struct SemanticVersion(
    int Major,
    int Minor,
    int Patch,
    bool IsPrerelease,
    string Prerelease) : IComparable<SemanticVersion>
{
    public static SemanticVersion Default() => new(1, 0, 0, false, "");

    public static bool TryParse(string? value, out SemanticVersion version)
    {
        version = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.TrimStart('v', 'V');

        var prereleaseSeparator = value.IndexOf('-');

        var core = prereleaseSeparator < 0
            ? value
            : value[..prereleaseSeparator];

        var numbers = core.Split('.');

        if (numbers.Length is < 2 or > 3 ||
            !int.TryParse(numbers[0], out var major) ||
            !int.TryParse(numbers[1], out var minor))
        {
            return false;
        }

        var patch = 0;

        if (numbers.Length == 3 &&
            !int.TryParse(numbers[2], out patch))
        {
            return false;
        }

        var prerelease = prereleaseSeparator < 0
            ? string.Empty
            : value[(prereleaseSeparator + 1)..];

        version = new SemanticVersion(
            major,
            minor,
            patch,
            prerelease.Length > 0,
            prerelease);

        return true;
    }

    public int CompareTo(SemanticVersion other)
    {
        var result = Major.CompareTo(other.Major);

        if (result != 0)
        {
            return result;
        }

        result = Minor.CompareTo(other.Minor);

        if (result != 0)
        {
            return result;
        }

        result = Patch.CompareTo(other.Patch);

        if (result != 0)
        {
            return result;
        }

        if (!IsPrerelease)
        {
            return other.IsPrerelease ? 1 : 0;
        }

        if (!other.IsPrerelease)
        {
            return -1;
        }

        return ComparePrerelease(Prerelease, other.Prerelease);
    }

    private static int ComparePrerelease(string left, string right)
    {
        var leftParts = left.Split('.');
        var rightParts = right.Split('.');
        var length = Math.Max(leftParts.Length, rightParts.Length);

        for (var i = 0; i < length; i++)
        {
            var leftPart = i < leftParts.Length ? leftParts[i] : null;
            var rightPart = i < rightParts.Length ? rightParts[i] : null;

            if (leftPart == rightPart)
            {
                continue;
            }

            if (leftPart is null)
            {
                return -1;
            }

            if (rightPart is null)
            {
                return 1;
            }

            if (int.TryParse(leftPart, out var leftNumber) &&
                int.TryParse(rightPart, out var rightNumber))
            {
                return leftNumber.CompareTo(rightNumber);
            }

            return string.CompareOrdinal(leftPart, rightPart);
        }

        return 0;
    }

    public override string ToString()
    {
        var core = $"{Major}.{Minor}.{Patch}";
        return IsPrerelease ? $"{core}-{Prerelease}" : core;
    }
}