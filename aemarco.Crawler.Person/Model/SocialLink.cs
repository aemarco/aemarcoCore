﻿namespace aemarco.Crawler.Person.Model;

public record SocialLink(SocialLinkKind Kind, string Url) : IComparable<SocialLink>
{
    public static SocialLink FromUri(Uri uri)
    {
        var kind = uri.Host switch
        {
            "twitter.com" => SocialLinkKind.Twitter,
            "instagram.com" => SocialLinkKind.Instagram,
            "www.instagram.com" => SocialLinkKind.Instagram,
            "facebook.com" => SocialLinkKind.Facebook,
            "youtube.com" => SocialLinkKind.YouTube,
            "www.youtube.com" => SocialLinkKind.YouTube,
            _ => SocialLinkKind.Unknown
        };
        return new SocialLink(kind, uri.AbsoluteUri);
    }

    public int CompareTo(SocialLink? other)
    {
        if (other is null)
            return -1;

        return Kind - other.Kind;
    }
}

public enum SocialLinkKind
{
    Unknown,
    Official,
    Twitter,
    Instagram,
    Facebook,
    YouTube
}
