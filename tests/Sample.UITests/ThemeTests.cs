namespace Sample.UITests;

/// <summary>
/// Tests for switching between light and dark mode across multiple control pages.
/// Captures screenshots in each mode so visual regressions are detectable.
/// </summary>
public abstract class ThemeTests : PlatformTestBase
{
    async Task SetTheme(string theme)
    {
        // Navigate to the TableView basic settings page where the Theme radio section lives
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");

        // Scroll to the theme section
        await Driver.Scroll(automationId: "SettingsTableView", dy: -300);
        await Task.Delay(300);

        // Tap the desired theme radio cell
        var themeOptions = await Driver.Query(text: theme);
        if (themeOptions.Count > 0)
            await Driver.Tap(elementId: themeOptions[0].Id);

        await Task.Delay(500);
    }

    [Test]
    public async Task Theme_SwitchToLight_HomePageScreenshot()
    {
        await SetTheme("Light");
        await Driver.Navigate("//home");
        await Driver.WaitUntilExists("HomePage");
        await Driver.Screenshot("theme-light-home.png");
    }

    [Test]
    public async Task Theme_SwitchToDark_HomePageScreenshot()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//home");
        await Driver.WaitUntilExists("HomePage");
        await Driver.Screenshot("theme-dark-home.png");
    }

    [Test]
    public async Task Theme_Light_SliderPage()
    {
        await SetTheme("Light");
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");
        await Driver.Screenshot("theme-light-slider.png");
    }

    [Test]
    public async Task Theme_Dark_SliderPage()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");
        await Driver.Screenshot("theme-dark-slider.png");
    }

    [Test]
    public async Task Theme_Light_PillsPage()
    {
        await SetTheme("Light");
        await Driver.Navigate("//pills");
        await Driver.WaitUntilExists("PillPage");
        await Driver.Screenshot("theme-light-pills.png");
    }

    [Test]
    public async Task Theme_Dark_PillsPage()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//pills");
        await Driver.WaitUntilExists("PillPage");
        await Driver.Screenshot("theme-dark-pills.png");
    }

    [Test]
    public async Task Theme_Light_TextEntryPage()
    {
        await SetTheme("Light");
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");
        await Driver.Screenshot("theme-light-textentry.png");
    }

    [Test]
    public async Task Theme_Dark_TextEntryPage()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");
        await Driver.Screenshot("theme-dark-textentry.png");
    }

    [Test]
    public async Task Theme_Light_ProgressBarPage()
    {
        await SetTheme("Light");
        await Driver.Navigate("//progressbar");
        await Driver.WaitUntilExists("ProgressBarPage");
        await Driver.Screenshot("theme-light-progressbar.png");
    }

    [Test]
    public async Task Theme_Dark_ProgressBarPage()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//progressbar");
        await Driver.WaitUntilExists("ProgressBarPage");
        await Driver.Screenshot("theme-dark-progressbar.png");
    }

    [Test]
    public async Task Theme_Light_TableViewPage()
    {
        await SetTheme("Light");
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");
        await Driver.Screenshot("theme-light-tableview.png");
    }

    [Test]
    public async Task Theme_Dark_TableViewPage()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");
        await Driver.Screenshot("theme-dark-tableview.png");
    }

    [Test]
    public async Task Theme_Light_ChatPage()
    {
        await SetTheme("Light");
        await Driver.Navigate("//chat");
        await Driver.WaitUntilExists("ChatPage");
        await Driver.Screenshot("theme-light-chat.png");
    }

    [Test]
    public async Task Theme_Dark_ChatPage()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//chat");
        await Driver.WaitUntilExists("ChatPage");
        await Driver.Screenshot("theme-dark-chat.png");
    }

    [Test]
    public async Task Theme_Light_SecurityPinPage()
    {
        await SetTheme("Light");
        await Driver.Navigate("//securitypin");
        await Driver.WaitUntilExists("SecurityPinPage");
        await Driver.Screenshot("theme-light-securitypin.png");
    }

    [Test]
    public async Task Theme_Dark_SecurityPinPage()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//securitypin");
        await Driver.WaitUntilExists("SecurityPinPage");
        await Driver.Screenshot("theme-dark-securitypin.png");
    }

    [Test]
    public async Task Theme_Light_ToastPage()
    {
        await SetTheme("Light");
        await Driver.Navigate("//toast");
        await Driver.WaitUntilExists("ToastPage");
        await Driver.Screenshot("theme-light-toast.png");
    }

    [Test]
    public async Task Theme_Dark_ToastPage()
    {
        await SetTheme("Dark");
        await Driver.Navigate("//toast");
        await Driver.WaitUntilExists("ToastPage");
        await Driver.Screenshot("theme-dark-toast.png");
    }

    [Test]
    public async Task Theme_SwitchBetweenDarkAndLight_AllPages()
    {
        var pages = new[]
        {
            ("//home", "HomePage"),
            ("//slider", "SliderPage"),
            ("//pills", "PillPage"),
            ("//securitypin", "SecurityPinPage"),
            ("//colorpicker", "ColorPickerPage"),
            ("//textentry", "TextEntryPage"),
            ("//progressbar", "ProgressBarPage"),
            ("//toast", "ToastPage"),
            ("//basic", "BasicSettingsPage"),
            ("//chat", "ChatPage"),
            ("//fab", "FabPage"),
            ("//overlay", "OverlayPage")
        };

        // Dark mode screenshots
        await SetTheme("Dark");
        foreach (var (route, pageId) in pages)
        {
            await Driver.Navigate(route);
            await Driver.WaitUntilExists(pageId);
            await Driver.Screenshot($"theme-dark-{pageId.ToLowerInvariant()}.png");
        }

        // Light mode screenshots
        await SetTheme("Light");
        foreach (var (route, pageId) in pages)
        {
            await Driver.Navigate(route);
            await Driver.WaitUntilExists(pageId);
            await Driver.Screenshot($"theme-light-{pageId.ToLowerInvariant()}.png");
        }

        // Reset to system theme
        await SetTheme("System");
    }
}
