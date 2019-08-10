namespace aemarcoCore.Crawlers.Types
{

    public class AbyssCategoryList
    {
        public bool success { get; set; }
        public AbyssCategory[] categories { get; set; }
    }

    public class AbyssCategory
    {
        public string name { get; set; }
        public int id { get; set; }
        public int count { get; set; }
        public string url { get; set; }
    }





    public class AbyssQueryCount
    {
        public bool success { get; set; }
        public AbyssCounts counts { get; set; }
    }

    public class AbyssCounts
    {
        public int month_count { get; set; }
        public int month_price { get; set; }
        public int last_month_count { get; set; }
        public int last_month_price { get; set; }
    }



    public class AbyssWallpaperPage
    {
        public bool success { get; set; }
        public AbyssWallpaper[] wallpapers { get; set; }
        public bool is_last { get; set; }
    }

    public class AbyssWallpaper
    {
        public string id { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string file_type { get; set; }
        public string file_size { get; set; }
        public string url_image { get; set; }
        public string url_thumb { get; set; }
        public string url_page { get; set; }
    }




    public class AbyssWallpaperInfoResult
    {
        public bool success { get; set; }
        public AbyssWallpaperInfo wallpaper { get; set; }
        public AbyssTag[] tags { get; set; }
    }

    public class AbyssWallpaperInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public object featured { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string file_type { get; set; }
        public string file_size { get; set; }
        public string url_image { get; set; }
        public string url_thumb { get; set; }
        public string url_page { get; set; }
        public string category { get; set; }
        public string category_id { get; set; }
        public string sub_category { get; set; }
        public string sub_category_id { get; set; }
        public string user_name { get; set; }
        public string user_id { get; set; }
        public object collection { get; set; }
        public int collection_id { get; set; }
        public object group { get; set; }
        public int group_id { get; set; }
    }

    public class AbyssTag
    {
        public string id { get; set; }
        public string name { get; set; }
    }

}
