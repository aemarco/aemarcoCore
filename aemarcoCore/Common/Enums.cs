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
        Girls_Guns,
        Girls_Fantasy,
        Girls_Cosplay,
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

    //wallsite
    public enum SourceSite
    {
        //
        [SupportedCategories(Category.Girls, Category.Girls_Lingerie)]
        AdultWalls,

        //
        [SupportedCategories(Category.Girls, Category.Girls_Lesbians,
            Category.Girls_Lingerie, Category.Girls_Beaches, Category.Girls_Asian, Category.Girls_Fantasy)]
        Erowall,

        //
        [SupportedCategories(Category.Girls_Celebrities, Category.Girls_Beaches, Category.Girls_Cars,
            Category.Girls_Bikes, Category.Girls_Lingerie, Category.Girls_Asian, Category.Girls_Holidays,
            Category.Girls_Fantasy, Category.Girls_CelebrityFakes, Category.Girls_Fetish, Category.Girls)]
        Ftop,

        //
        [SupportedCategories(Category.Girls)]
        Pornomass,

        //
        [SupportedCategories(Category.Fantasy_3D, Category.Fantasy_Abstract, Category.Hobbies_Animals,
            Category.Fantasy_Anime, Category.Fantasy_Art, Category.Vehicle_Cars, Category.Environment_City,
            Category.Fantasy, Category.Environment_Flowers, Category.Hobbies_Food, Category.Media_Games,
            Category.Hobbies_HiTech, Category.Other_Holidays, Category.Environment_Macro,
            Category.Vehicle_Bikes, Category.Media_Movies, Category.Media_Music,
            Category.Environment_Landscape, Category.Other, Category.Environment_Space,
            Category.Hobbies_Sport, Category.Other_Textures, Category.Media_TVSeries,
            Category.Fantasy_Vector, Category.Other_Words)]
        Wallpaperscraft,

        //
        [SupportedCategories(Category.Girls)]
        Zoomgirls,

        //
        [SupportedCategories(Category.Girls_Fantasy, Category.Girls)]
        Wallhaven,

        //
        [SupportedCategories(Category.Girls_Lingerie, Category.Girls)]
        BabesUnivers,

        //
        [SupportedCategories(Category.Girls_Amateur, Category.Girls_Asian, Category.Girls_Fetish,
            Category.Girls_Holidays, Category.Girls_Cosplay, Category.Girls_Gloryhole, Category.Girls_Selfies,
            Category.Girls, Category.Girls_Hardcore, Category.Girls_Lesbians)]
        Pornpics,

        //
        [SupportedCategories(Category.Fantasy_Art, Category.Hobbies_Animals, Category.Girls_Fantasy,
            Category.Vehicle_Planes, Category.Vehicle_Cars, Category.Girls_Celebrities,
            Category.Hobbies_Food, Category.Media_Games, Category.Girls, Category.Other_Holidays,
            Category.Men, Category.Vehicle_Bikes, Category.Media_Movies, Category.Media_Music,
            Category.Environment, Category.Environment_Space, Category.Hobbies_Sport, Category.Other,
            Category.Environment_City)]
        Mota,

        //
        [SupportedCategories(Category.Girls_Asian, Category.Girls_Lingerie, Category.Girls)]
        Zoompussy,

        //
        [SupportedCategories(Category.Girls_Asian, Category.Girls_Lingerie, Category.Girls)]
        Moozpussy,


        [SupportedCategories(Category.Girls, Category.Vehicle, Category.Media_Games, Category.Media_Movies, Category.Media_TVSeries, Category.Media_Music, Category.Men)]
        Abyss,


        [SupportedCategories(Category.Girls, Category.Girls_Hardcore, Category.Girls_Lesbians, Category.Girls_Asian, Category.Girls_Selfies, Category.Girls_Amateur)]
        CoedCherry
    }


    //personsite
    public enum PersonSite
    {
        IStripper,
        Pornpics,
        Porngatherer,
        Nudevista,
        CoedCherry
    }


    internal enum CrawlMethod
    {
        Classic,
        API
    }

}
