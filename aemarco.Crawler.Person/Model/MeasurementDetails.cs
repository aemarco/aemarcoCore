namespace aemarco.Crawler.Person.Model;
public class MeasurementDetails
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



    //TODO: merge



}
