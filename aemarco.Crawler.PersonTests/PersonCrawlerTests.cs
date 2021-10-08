using aemarco.Crawler.Core.Extensions;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.PersonCrawlerTests
{
    public class PersonCrawlerTests
    {

        [Test]
        public void GetAvailableCrawlers_DeliversCorrectly()
        {
            var types = PersonCrawler.PersonCrawler.GetCrawlerTypes();
            var infos = types.Select(x => x.ToCrawlerInfo()).ToList();

            var crawler = new PersonCrawler.PersonCrawler("none");
            var available = crawler.GetAvailableCrawlers().ToList();


            foreach (var info in infos)
            {
                if (info.IsEnabled)
                    available.Should().Contain(info.FriendlyName);
                else
                    available.Should().NotContain(info.FriendlyName);
            }
        }

        [Test]
        public async Task StartAsync_DoesNotCrawlDisabledCrawlers()
        {
            var crawler = new PersonCrawler.PersonCrawler("Foxi Di");
            await crawler.StartAsync(CancellationToken.None);
        }

        [Test]
        public async Task StartAsync_MergesResults()
        {
            var crawler = new PersonCrawler.PersonCrawler("Foxi Di");
            var result = await crawler.StartAsync(CancellationToken.None);

            result.ProfilePictures.Count.Should().Be(5);
            result.Aliases.Count.Should().Be(19);
            result.Piercings.Should().Be("Navel");

        }


    }
}
