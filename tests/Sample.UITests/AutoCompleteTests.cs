namespace Sample.UITests;

public abstract class AutoCompleteTests : PlatformTestBase
{
    async Task NavigateToAutoComplete()
    {
        await Driver.Navigate("//autocomplete");
        await Driver.WaitUntilExists("AutoCompletePage");
    }

    [Test]
    public async Task AutoComplete_PageLoads()
    {
        await NavigateToAutoComplete();

        var isVisible = await Driver.IsElementVisible("AutoCompletePage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task AutoComplete_FruitsSectionExists()
    {
        await NavigateToAutoComplete();

        var labels = await Driver.Query(text: "Fruits");
        await Assert.That(labels.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task AutoComplete_LanguagesSectionExists()
    {
        await NavigateToAutoComplete();

        var labels = await Driver.Query(text: "Languages");
        await Assert.That(labels.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task AutoComplete_Screenshot()
    {
        await NavigateToAutoComplete();

        await Driver.Screenshot("autocomplete-page.png");
    }
}
