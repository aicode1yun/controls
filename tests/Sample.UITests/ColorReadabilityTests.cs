namespace Sample.UITests;

/// <summary>
/// Tests that verify controls remain visually readable across themes by
/// checking that key elements exist and taking comparison screenshots.
/// These screenshots enable manual or automated visual diff review to
/// catch contrast or color readability regressions.
/// </summary>
public abstract class ColorReadabilityTests : PlatformTestBase
{
    async Task SetTheme(string theme)
    {
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");
        await Driver.Scroll(automationId: "SettingsTableView", dy: -300);
        await Task.Delay(300);

        var themeOptions = await Driver.Query(text: theme);
        if (themeOptions.Count > 0)
            await Driver.Tap(elementId: themeOptions[0].Id);
        await Task.Delay(500);
    }

    [Test]
    public async Task Readability_LightMode_PillsHaveVisibleText()
    {
        await SetTheme("Light");
        await Driver.Navigate("//pills");
        await Driver.WaitUntilExists("PillPage");

        // Verify all preset pill types are visible (text is readable)
        var pillTexts = new[] { "None", "Success", "Info", "Warning", "Caution", "Critical" };
        foreach (var text in pillTexts)
        {
            var elements = await Driver.Query(text: text);
            await Assert.That(elements.Count).IsGreaterThanOrEqualTo(1);
        }

        await Driver.Screenshot("readability-light-pills.png");
    }

    [Test]
    public async Task Readability_DarkMode_PillsHaveVisibleText()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//pills");
        await Driver.WaitUntilExists("PillPage");

        var pillTexts = new[] { "None", "Success", "Info", "Warning", "Caution", "Critical" };
        foreach (var text in pillTexts)
        {
            var elements = await Driver.Query(text: text);
            await Assert.That(elements.Count).IsGreaterThanOrEqualTo(1);
        }

        await Driver.Screenshot("readability-dark-pills.png");
    }

    [Test]
    public async Task Readability_LightMode_TableViewCellsReadable()
    {
        await SetTheme("Light");
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");

        // Verify cell titles are visible
        var wifiCells = await Driver.Query(text: "Wi-Fi");
        await Assert.That(wifiCells.Count).IsGreaterThanOrEqualTo(1);

        var versionCells = await Driver.Query(text: "Version");
        await Assert.That(versionCells.Count).IsGreaterThanOrEqualTo(1);

        await Driver.Screenshot("readability-light-tableview.png");
    }

    [Test]
    public async Task Readability_DarkMode_TableViewCellsReadable()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");

        var wifiCells = await Driver.Query(text: "Wi-Fi");
        await Assert.That(wifiCells.Count).IsGreaterThanOrEqualTo(1);

        var versionCells = await Driver.Query(text: "Version");
        await Assert.That(versionCells.Count).IsGreaterThanOrEqualTo(1);

        await Driver.Screenshot("readability-dark-tableview.png");
    }

    [Test]
    public async Task Readability_LightMode_SliderLabelsReadable()
    {
        await SetTheme("Light");
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");

        var tempLabel = await Driver.Query(text: "Temperature");
        await Assert.That(tempLabel.Count).IsGreaterThanOrEqualTo(1);

        var customLabel = await Driver.Query(text: "Custom Colors (Green → Yellow)");
        await Assert.That(customLabel.Count).IsGreaterThanOrEqualTo(1);

        await Driver.Screenshot("readability-light-slider.png");
    }

    [Test]
    public async Task Readability_DarkMode_SliderLabelsReadable()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");

        var tempLabel = await Driver.Query(text: "Temperature");
        await Assert.That(tempLabel.Count).IsGreaterThanOrEqualTo(1);

        await Driver.Screenshot("readability-dark-slider.png");
    }

    [Test]
    public async Task Readability_LightMode_TextEntryPlaceholdersVisible()
    {
        await SetTheme("Light");
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");

        // Text entries should be visible
        var isVisible = await Driver.IsElementVisible("FirstNameEntry");
        await Assert.That(isVisible).IsTrue();

        isVisible = await Driver.IsElementVisible("LastNameEntry");
        await Assert.That(isVisible).IsTrue();

        await Driver.Screenshot("readability-light-textentry.png");
    }

    [Test]
    public async Task Readability_DarkMode_TextEntryPlaceholdersVisible()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");

        var isVisible = await Driver.IsElementVisible("FirstNameEntry");
        await Assert.That(isVisible).IsTrue();

        isVisible = await Driver.IsElementVisible("LastNameEntry");
        await Assert.That(isVisible).IsTrue();

        await Driver.Screenshot("readability-dark-textentry.png");
    }

    [Test]
    public async Task Readability_LightMode_ToastButtonsReadable()
    {
        await SetTheme("Light");
        await Driver.Navigate("//toast");
        await Driver.WaitUntilExists("ToastPage");

        var isVisible = await Driver.IsElementVisible("ShowToastButton");
        await Assert.That(isVisible).IsTrue();

        await Driver.Screenshot("readability-light-toast.png");
    }

    [Test]
    public async Task Readability_DarkMode_ToastButtonsReadable()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//toast");
        await Driver.WaitUntilExists("ToastPage");

        var isVisible = await Driver.IsElementVisible("ShowToastButton");
        await Assert.That(isVisible).IsTrue();

        await Driver.Screenshot("readability-dark-toast.png");
    }

    [Test]
    public async Task Readability_LightMode_ProgressBarControlsVisible()
    {
        await SetTheme("Light");
        await Driver.Navigate("//progressbar");
        await Driver.WaitUntilExists("ProgressBarPage");

        var isVisible = await Driver.IsElementVisible("BasicProgressBar");
        await Assert.That(isVisible).IsTrue();

        await Driver.Screenshot("readability-light-progressbar.png");
    }

    [Test]
    public async Task Readability_DarkMode_ProgressBarControlsVisible()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//progressbar");
        await Driver.WaitUntilExists("ProgressBarPage");

        var isVisible = await Driver.IsElementVisible("BasicProgressBar");
        await Assert.That(isVisible).IsTrue();

        await Driver.Screenshot("readability-dark-progressbar.png");
    }

    [Test]
    public async Task Readability_LightMode_SecurityPinCellsVisible()
    {
        await SetTheme("Light");
        await Driver.Navigate("//securitypin");
        await Driver.WaitUntilExists("SecurityPinPage");

        var isVisible = await Driver.IsElementVisible("HiddenPin");
        await Assert.That(isVisible).IsTrue();

        await Driver.Screenshot("readability-light-securitypin.png");
    }

    [Test]
    public async Task Readability_DarkMode_SecurityPinCellsVisible()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//securitypin");
        await Driver.WaitUntilExists("SecurityPinPage");

        var isVisible = await Driver.IsElementVisible("HiddenPin");
        await Assert.That(isVisible).IsTrue();

        await Driver.Screenshot("readability-dark-securitypin.png");
    }

    [Test]
    public async Task Readability_ResetThemeToSystem()
    {
        // Reset to system theme after all readability tests
        await SetTheme("System");
    }
}
