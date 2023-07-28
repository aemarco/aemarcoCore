namespace aemarco.Crawler.Person.Model;
public partial record MeasurementDetails(int? Bust, string? Cup, bool FakeTits, int? Waist, int? Hip)
    : IParsable<MeasurementDetails>
{

    public static MeasurementDetails Empty => new(null, null, false, null, null);
    public string? Cup { get; init; } = Cup?.ToUpper();
    public MeasurementDetails Combine(MeasurementDetails? other)
    {
        if (other is null)
            return this;

        return new MeasurementDetails(
            Bust ?? other.Bust,
            Cup ?? other.Cup,
            FakeTits | other.FakeTits,
            Waist ?? other.Waist,
            Hip ?? other.Hip);
    }

    /// <example>
    /// 86-58-81
    /// 96DDD-66-93
    /// 96DDD(fake)-66-93
    /// </example>
    public override string ToString()
    {
        var fakeText = FakeTits
            ? "(fake)"
            : null;

        return Bust.HasValue && Waist.HasValue && Hip.HasValue
            ? $"{Bust}{Cup}{fakeText}-{Waist}-{Hip}"
            : string.IsNullOrWhiteSpace(Cup)
                ? string.Empty
                : $"{Cup}{fakeText}";
    }


    #region IParseable

    [GeneratedRegex(@"(\d+)([a-dA-D]*)-(\d+)-(\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex MeasurementsRegex();

    [GeneratedRegex(@"[A-D]+", RegexOptions.IgnoreCase)]
    private static partial Regex CupRegex();



    public static MeasurementDetails Parse(string text, IFormatProvider? provider = null)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (TryParse(text, provider, out var result))
            return result;

        throw new FormatException();
    }

    public static bool TryParse([NotNullWhen(true)] string? text, out MeasurementDetails result) =>
        TryParse(text, null, out result);
    public static bool TryParse([NotNullWhen(true)] string? text, IFormatProvider? provider, out MeasurementDetails result)
    {
        result = Empty;
        if (text is null)
            return false;
        if (string.IsNullOrWhiteSpace(text))
            return true;

        text = text.Replace("/", "-");
        text = text.Replace(" -", "-");
        text = text.Replace("- ", "-");

        if (MeasurementsRegex().Match(text) is { Success: true } measureMatch &&
            int.TryParse(measureMatch.Groups[1].Value, out var bust) &&
            measureMatch.Groups[2].Value is { } cup &&
            int.TryParse(measureMatch.Groups[3].Value, out var waist) &&
            int.TryParse(measureMatch.Groups[4].Value, out var hip))
        {
            //measures
            if (provider is MeasurementFormatProvider { MeasurementSystem: MeasurementSystem.Imperial })
            {
                bust = (int)(bust * 2.54);
                waist = (int)(waist * 2.54);
                hip = (int)(hip * 2.54);
            }

            //tits
            cup = string.IsNullOrWhiteSpace(cup) ? null : cup.Trim();
            var fake = !string.IsNullOrWhiteSpace(cup) &&
                       text.Contains("fake", StringComparison.OrdinalIgnoreCase);
            result = new MeasurementDetails(bust, cup?.Trim(), fake, waist, hip);
            return true;
        }

        if (CupRegex().Match(text) is { Success: true } cupMatch)
        {
            result = new MeasurementDetails(
                null,
                cupMatch.Value,
                text.Contains("fake", StringComparison.OrdinalIgnoreCase),
                null,
                null);
            return true;
        }

        return false;
    }

    #endregion

}

#region IParseable


public class MeasurementFormatProvider : IFormatProvider
{
    public MeasurementFormatProvider(MeasurementSystem measurementSystem)
    {
        MeasurementSystem = measurementSystem;
    }

    public MeasurementSystem MeasurementSystem { get; }


    public object? GetFormat(Type? formatType)
    {
        if (formatType == typeof(ICustomFormatter))
        {
            return this;
        }
        return null;
    }
}

public enum MeasurementSystem
{
    Metric,
    Imperial
}

#endregion
