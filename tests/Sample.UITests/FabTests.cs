namespace Sample.UITests;

public abstract class FabTests : PlatformTestBase
{
    async Task NavigateToFab()
    {
        await Driver.Navigate("//fab");
        await Driver.WaitUntilExists("FabPage");
    }

    [Test]
    public async Task Fab_PageLoads()
    {
        await NavigateToFab();

        var isVisible = await Driver.IsElementVisible("FabPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Fab_StatusMessageExists()
    {
        await NavigateToFab();

        var labels = await Driver.Query(text: "Floating Action Button");
        await Assert.That(labels.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Fab_InlineFabsSectionVisible()
    {
        await NavigateToFab();

        var inlineLabel = await Driver.Query(text: "Inline Fab (icon only)");
        await Assert.That(inlineLabel.Count).IsGreaterThanOrEqualTo(1);

        var extendedLabel = await Driver.Query(text: "Extended Fab (icon + text)");
        await Assert.That(extendedLabel.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Fab_Screenshot()
    {
        await NavigateToFab();

        await Driver.Screenshot("fab-page.png");
    }
}
