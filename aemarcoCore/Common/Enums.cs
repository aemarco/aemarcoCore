namespace aemarcoCore.Common
{

    //crawler
    public enum Category
    {
        Men,
        Girls,        
        Girls_Celebrities,
        Girls_CelebrityFakes,
        Girls_Cars,
        Girls_Bikes,
        Girls_Fantasy,
        Girls_Lingerie,
        Girls_Lesbians,
        Girls_Beaches,
        Girls_Asian,
        Girls_Holidays,
        Girls_Fetish,
        Girls_Blowjob,
        Girls_Hardcore,
        Girls_Amateur,
        Girls_Gloryhole,
        Girls_Selfies,

        Vehicle,
        Vehicle_Cars,
        Vehicle_Bikes,
        Vehicle_Planes,

        Media,
        Media_Movies,
        Media_TVSeries,
        Media_Games,
        Media_Music,

        Environment,
        Environment_Landscape,
        Environment_Space,
        Environment_City,
        Environment_Flowers,
        Environment_Macro,

        Fantasy,
        Fantasy_Abstract,
        Fantasy_Anime,
        Fantasy_3D,
        Fantasy_Vector,
        Fantasy_Art,

        Hobbies,
        Hobbies_Sport,
        Hobbies_Animals,
        Hobbies_Food,
        Hobbies_HiTech,

        Other,
        Other_Holidays,
        Other_Textures,
        Other_Brands,
        Other_Words


    }


    //wall setter
    public enum WallpaperMode
    {
        Fit,
        AllowFill,
        AllowFillForceCut,
        Fill
    }

    //wallsite
    public enum SourceSite
    {
        AdultWalls,
        Erowall,
        Ftop,
        Pornomass,
        Wallpaperscraft,
        Zoomgirls,
        Wallhaven,
        BabesUnivers,
        Pornpics,
        Mota

    }

    /// <summary>
    /// None... No caching
    /// BySize... Uses that given amount in Bytes (CacheSizeInBytes defaults to 250MB). Cleanup works with a 10% Threshold
    /// Auto... Cachesize may fluctuate according available diskspace (CacheSizePercentage default to 5%).
    /// </summary>
    public enum CacheMode
    {
        None,
        BySize,        
        Auto
    }




}
