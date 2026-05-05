namespace Sample.UITests;

public abstract class TextEntryTests : PlatformTestBase
{
    async Task NavigateToTextEntry()
    {
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");
    }

    [Test]
    public async Task TextEntry_PageLoads()
    {
        await NavigateToTextEntry();

        var isVisible = await Driver.IsElementVisible("TextEntryPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task TextEntry_FirstNameEntryExists()
    {
        await NavigateToTextEntry();

        var isVisible = await Driver.IsElementVisible("FirstNameEntry");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task TextEntry_LastNameEntryExists()
    {
        await NavigateToTextEntry();

        var isVisible = await Driver.IsElementVisible("LastNameEntry");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task TextEntry_SearchEntryExists()
    {
        await NavigateToTextEntry();

        var isVisible = await Driver.IsElementVisible("SearchEntry");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task TextEntry_TypeIntoFirstName()
    {
        await NavigateToTextEntry();

        await Driver.Tap(automationId: "FirstNameEntry");
        await Driver.Fill("John", automationId: "FirstNameEntry");
        await Task.Delay(500);
        await Driver.Screenshot("text-entry-filled.png");
    }

    [Test]
    public async Task TextEntry_TypeIntoLastName()
    {
        await NavigateToTextEntry();

        await Driver.Tap(automationId: "LastNameEntry");
        await Driver.Fill("Doe", automationId: "LastNameEntry");
        await Task.Delay(500);
        await Driver.Screenshot("text-entry-lastname-filled.png");
    }

    [Test]
    public async Task TextEntry_ClearSearch()
    {
        await NavigateToTextEntry();

        await Driver.Fill("Test Query", automationId: "SearchEntry");
        await Task.Delay(300);
        await Driver.Screenshot("text-entry-search-filled.png");

        await Driver.Clear(automationId: "SearchEntry");
        await Task.Delay(300);
        await Driver.Screenshot("text-entry-search-cleared.png");
    }

    [Test]
    public async Task TextEntry_Screenshot()
    {
        await NavigateToTextEntry();

        await Driver.Screenshot("text-entry-page.png");
    }
}
