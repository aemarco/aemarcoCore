using aemarco.Crawler.Model;

namespace aemarco.CrawlerTests.Model;

internal class TagTests
{

    [Test]
    public void Equals_SameContentOtherInstances_EqualWithSameHash()
    {
        var first = new Tag("Tsuki (Billlie)", "Musicians", ["Fukutomi Tsuki", "TARU"]);
        var second = new Tag("Tsuki (Billlie)", "Musicians", ["Fukutomi Tsuki", "TARU"]);

        var result = first == second;

        result.ShouldBeTrue();
        second.GetHashCode().ShouldBe(first.GetHashCode());
    }

    [Test]
    public void Equals_NoAliasesOrEmptyAliases_Equal()
    {
        var first = new Tag("redhead");
        var second = new Tag("redhead", null, []);

        var result = first == second;

        result.ShouldBeTrue();
    }

    [TestCase("other", "Musicians", "a")]
    [TestCase("text", "Other", "a")]
    [TestCase("text", null, "a")]
    [TestCase("text", "Musicians", "b")]
    [TestCase("text", "Musicians", "a,b")]
    public void Equals_DifferentContent_NotEqual(string text, string? category, string aliases)
    {
        var first = new Tag("text", "Musicians", ["a"]);
        var second = new Tag(text, category, [.. aliases.Split(',')]);

        var result = first == second;

        result.ShouldBeFalse();
    }

    [Test]
    public void Equals_OtherAliasOrder_NotEqual()
    {
        var first = new Tag("text", null, ["a", "b"]);
        var second = new Tag("text", null, ["b", "a"]);

        var result = first == second;

        result.ShouldBeFalse();
    }

    [Test]
    public void HashSet_TwoIdenticalTags_HoldsOne()
    {
        var tags = new HashSet<Tag>
        {
            new("text", "Musicians", ["a"]),
            new("text", "Musicians", ["a"])
        };

        tags.Count.ShouldBe(1);
    }

}
