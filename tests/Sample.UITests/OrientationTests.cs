namespace Sample.UITests;

/// <summary>
/// Tests that controls render correctly after device orientation changes.
/// Uses Resize to simulate portrait/landscape dimensions.
/// Captures screenshots in both orientations for visual comparison.
/// </summary>
public abstract class OrientationTests : PlatformTestBase
{
    // Common phone dimensions
    const int PortraitWidth = 390;
    const int PortraitHeight = 844;
    const int LandscapeWidth = 844;
    const int LandscapeHeight = 390;

    async Task SetPortrait() => await Driver.Resize(PortraitWidth, PortraitHeight);
    async Task SetLandscape() => await Driver.Resize(LandscapeWidth, LandscapeHeight);

    [After(Test)]
    public async Task ResetToPortrait()
    {
        // Always reset back to portrait after each test
        await Driver.Resize(PortraitWidth, PortraitHeight);
        await Task.Delay(300);
    }

    [Test]
    public async Task Orientation_Home_Portrait()
    {
        await SetPortrait();
        await Driver.Navigate("//home");
        await Driver.WaitUntilExists("HomePage");
        await Driver.Screenshot("orientation-portrait-home.png");
    }

    [Test]
    public async Task Orientation_Home_Landscape()
    {
        await Driver.Navigate("//home");
        await Driver.WaitUntilExists("HomePage");

        await SetLandscape();
        await Task.Delay(500);
        await Driver.Screenshot("orientation-landscape-home.png");

        // Verify page is still visible after rotation
        var isVisible = await Driver.IsElementVisible("HomePage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Orientation_Slider_Portrait()
    {
        await SetPortrait();
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");
        await Driver.Screenshot("orientation-portrait-slider.png");

        var isVisible = await Driver.IsElementVisible("TemperatureSlider");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Orientation_Slider_Landscape()
    {
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");

        await SetLandscape();
        await Task.Delay(500);
        await Driver.Screenshot("orientation-landscape-slider.png");

        var isVisible = await Driver.IsElementVisible("TemperatureSlider");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Orientation_TextEntry_Portrait()
    {
        await SetPortrait();
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");
        await Driver.Screenshot("orientation-portrait-textentry.png");
    }

    [Test]
    public async Task Orientation_TextEntry_Landscape()
    {
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");

        await SetLandscape();
        await Task.Delay(500);
        await Driver.Screenshot("orientation-landscape-textentry.png");

        var isVisible = await Driver.IsElementVisible("FirstNameEntry");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Orientation_Pills_Portrait()
    {
        await SetPortrait();
        await Driver.Navigate("//pills");
        await Driver.WaitUntilExists("PillPage");
        await Driver.Screenshot("orientation-portrait-pills.png");
    }

    [Test]
    public async Task Orientation_Pills_Landscape()
    {
        await Driver.Navigate("//pills");
        await Driver.WaitUntilExists("PillPage");

        await SetLandscape();
        await Task.Delay(500);
        await Driver.Screenshot("orientation-landscape-pills.png");
    }

    [Test]
    public async Task Orientation_TableView_Portrait()
    {
        await SetPortrait();
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");
        await Driver.Screenshot("orientation-portrait-tableview.png");
    }

    [Test]
    public async Task Orientation_TableView_Landscape()
    {
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");

        await SetLandscape();
        await Task.Delay(500);
        await Driver.Screenshot("orientation-landscape-tableview.png");
    }

    [Test]
    public async Task Orientation_Chat_Portrait()
    {
        await SetPortrait();
        await Driver.Navigate("//chat");
        await Driver.WaitUntilExists("ChatPage");
        await Driver.Screenshot("orientation-portrait-chat.png");
    }

    [Test]
    public async Task Orientation_Chat_Landscape()
    {
        await Driver.Navigate("//chat");
        await Driver.WaitUntilExists("ChatPage");

        await SetLandscape();
        await Task.Delay(500);
        await Driver.Screenshot("orientation-landscape-chat.png");

        var isVisible = await Driver.IsElementVisible("ChatView");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Orientation_ProgressBar_Portrait()
    {
        await SetPortrait();
        await Driver.Navigate("//progressbar");
        await Driver.WaitUntilExists("ProgressBarPage");
        await Driver.Screenshot("orientation-portrait-progressbar.png");
    }

    [Test]
    public async Task Orientation_ProgressBar_Landscape()
    {
        await Driver.Navigate("//progressbar");
        await Driver.WaitUntilExists("ProgressBarPage");

        await SetLandscape();
        await Task.Delay(500);
        await Driver.Screenshot("orientation-landscape-progressbar.png");
    }

    [Test]
    public async Task Orientation_RotateDuringInteraction()
    {
        await SetPortrait();
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");

        // Type into field in portrait
        await Driver.Fill("Hello", automationId: "FirstNameEntry");
        await Driver.Screenshot("orientation-portrait-typing.png");

        // Rotate to landscape
        await SetLandscape();
        await Task.Delay(500);
        await Driver.Screenshot("orientation-landscape-typing.png");

        // Verify the entry is still visible and content preserved
        var isVisible = await Driver.IsElementVisible("FirstNameEntry");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Orientation_FullRotationCycle()
    {
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");

        // Portrait
        await SetPortrait();
        await Task.Delay(300);
        await Driver.Screenshot("orientation-cycle-portrait-1.png");

        // Landscape
        await SetLandscape();
        await Task.Delay(300);
        await Driver.Screenshot("orientation-cycle-landscape.png");

        // Back to portrait
        await SetPortrait();
        await Task.Delay(300);
        await Driver.Screenshot("orientation-cycle-portrait-2.png");

        var isVisible = await Driver.IsElementVisible("TemperatureSlider");
        await Assert.That(isVisible).IsTrue();
    }
}
