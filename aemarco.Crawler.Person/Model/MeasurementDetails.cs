namespace aemarco.Crawler.Person.Model;
public record MeasurementDetails
{

    public int? Bust { get; internal set; }
    public int? Waist { get; internal set; }
    public int? Hip { get; internal set; }
    public string? Cup { get; internal set; }

    /// <example>
    /// 86-58-81
    /// 96DDD-66-93
    /// </example>
    public override string ToString()
    {
        if (!Bust.HasValue || !Waist.HasValue || !Hip.HasValue)
            return string.Empty;

        return $"{Bust}{Cup}-{Waist}-{Hip}";
    }

    public void Merge(MeasurementDetails? info)
    {
        if (info is null)
            return;

        Bust ??= info.Bust;
        Waist ??= info.Waist;
        Hip ??= info.Hip;
        Cup ??= info.Cup;
    }
}
