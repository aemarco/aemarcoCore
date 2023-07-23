namespace aemarco.Crawler.Person.Model;
public record MeasurementDetails(int? Bust, string? Cup, int? Waist, int? Hip)
{
    public static MeasurementDetails Empty => new(null, null, null, null);

    public MeasurementDetails Combine(MeasurementDetails? other)
    {
        if (other is null)
            return this;

        return new MeasurementDetails(
            Bust ?? other.Bust,
            Cup ?? other.Cup,
            Waist ?? other.Waist,
            Hip ?? other.Hip);
    }

    /// <example>
    /// 86-58-81
    /// 96DDD-66-93
    /// </example>
    public override string ToString()
    {
        return Bust.HasValue && Waist.HasValue && Hip.HasValue
            ? $"{Bust}{Cup}-{Waist}-{Hip}"
            : string.IsNullOrWhiteSpace(Cup)
                ? string.Empty
                : Cup;
    }
}
