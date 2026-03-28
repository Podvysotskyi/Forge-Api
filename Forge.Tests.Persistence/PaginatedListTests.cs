using Forge.Contracts;
using Forge.Persistence;

namespace Forge.Tests.Persistence;

public class PaginatedListTests
{
    [Test]
    public void Constructor_ShouldImplementInterfaceAndStorePagingFields()
    {
        var items = new[] { "first", "second" };

        IPaginatedList<string> paginatedList = PaginatedList<string>.Create(items, 2, 10, 25);

        Assert.Multiple(() =>
        {
            Assert.That(paginatedList.Items, Is.EqualTo(items));
            Assert.That(paginatedList.Page, Is.EqualTo(2));
            Assert.That(paginatedList.PageSize, Is.EqualTo(10));
            Assert.That(paginatedList.Total, Is.EqualTo(25));
        });
    }

    [Test]
    public void Constructor_ShouldReturnPaginatedListWithProvidedValues()
    {
        var items = new[] { "first", "second" };

        var paginatedList = PaginatedList<string>.Create(items, 3, 5, 12);

        Assert.Multiple(() =>
        {
            Assert.That(paginatedList.Items, Is.EqualTo(items));
            Assert.That(paginatedList.Page, Is.EqualTo(3));
            Assert.That(paginatedList.PageSize, Is.EqualTo(5));
            Assert.That(paginatedList.Total, Is.EqualTo(12));
        });
    }
}
