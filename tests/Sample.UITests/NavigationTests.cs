namespace Sample.UITests;

public abstract class NavigationTests : PlatformTestBase
{
    [Test]
    public async Task Navigate_ToHome()
    {
        await Driver.Navigate("//home");
        await Driver.WaitUntilExists("HomePage");

        var isVisible = await Driver.IsElementVisible("HomePage");
        await Assert.That(isVisible).IsTrue();
        await Driver.Screenshot("nav-home.png");
    }

    [Test]
    public async Task Navigate_ToSlider()
    {
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");

        var isVisible = await Driver.IsElementVisible("SliderPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToPills()
    {
        await Driver.Navigate("//pills");
        await Driver.WaitUntilExists("PillPage");

        var isVisible = await Driver.IsElementVisible("PillPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToSecurityPin()
    {
        await Driver.Navigate("//securitypin");
        await Driver.WaitUntilExists("SecurityPinPage");

        var isVisible = await Driver.IsElementVisible("SecurityPinPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToColorPicker()
    {
        await Driver.Navigate("//colorpicker");
        await Driver.WaitUntilExists("ColorPickerPage");

        var isVisible = await Driver.IsElementVisible("ColorPickerPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToTextEntry()
    {
        await Driver.Navigate("//textentry");
        await Driver.WaitUntilExists("TextEntryPage");

        var isVisible = await Driver.IsElementVisible("TextEntryPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToProgressBar()
    {
        await Driver.Navigate("//progressbar");
        await Driver.WaitUntilExists("ProgressBarPage");

        var isVisible = await Driver.IsElementVisible("ProgressBarPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToOverlay()
    {
        await Driver.Navigate("//overlay");
        await Driver.WaitUntilExists("OverlayPage");

        var isVisible = await Driver.IsElementVisible("OverlayPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToFab()
    {
        await Driver.Navigate("//fab");
        await Driver.WaitUntilExists("FabPage");

        var isVisible = await Driver.IsElementVisible("FabPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToToast()
    {
        await Driver.Navigate("//toast");
        await Driver.WaitUntilExists("ToastPage");

        var isVisible = await Driver.IsElementVisible("ToastPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToTableView()
    {
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");

        var isVisible = await Driver.IsElementVisible("BasicSettingsPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToSignaturePad()
    {
        await Driver.Navigate("//signaturepad");
        await Driver.WaitUntilExists("SignaturePadPage");

        var isVisible = await Driver.IsElementVisible("SignaturePadPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToChat()
    {
        await Driver.Navigate("//chat");
        await Driver.WaitUntilExists("ChatPage");

        var isVisible = await Driver.IsElementVisible("ChatPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToCalendar()
    {
        await Driver.Navigate("//calendar");
        await Driver.WaitUntilExists("CalendarPage");

        var isVisible = await Driver.IsElementVisible("CalendarPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToSheet()
    {
        await Driver.Navigate("//sheet");
        await Driver.WaitUntilExists("SheetPage");

        var isVisible = await Driver.IsElementVisible("SheetPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToAutoComplete()
    {
        await Driver.Navigate("//autocomplete");
        await Driver.WaitUntilExists("AutoCompletePage");

        var isVisible = await Driver.IsElementVisible("AutoCompletePage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToImageViewer()
    {
        await Driver.Navigate("//imageviewer");
        await Driver.WaitUntilExists("ImageViewerPage");

        var isVisible = await Driver.IsElementVisible("ImageViewerPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToFrostedGlass()
    {
        await Driver.Navigate("//frostedglass");
        await Driver.WaitUntilExists("FrostedGlassPage");

        var isVisible = await Driver.IsElementVisible("FrostedGlassPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_ToMarkdownView()
    {
        await Driver.Navigate("//markdownview");
        await Driver.WaitUntilExists("MarkdownViewPage");

        var isVisible = await Driver.IsElementVisible("MarkdownViewPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Navigate_AllControlPages_Sequentially()
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
            ("//overlay", "OverlayPage"),
            ("//fab", "FabPage"),
            ("//toast", "ToastPage"),
            ("//basic", "BasicSettingsPage"),
            ("//signaturepad", "SignaturePadPage"),
            ("//chat", "ChatPage"),
            ("//calendar", "CalendarPage"),
            ("//sheet", "SheetPage"),
            ("//autocomplete", "AutoCompletePage"),
            ("//imageviewer", "ImageViewerPage"),
            ("//frostedglass", "FrostedGlassPage"),
            ("//markdownview", "MarkdownViewPage")
        };

        foreach (var (route, pageId) in pages)
        {
            await Driver.Navigate(route);
            await Driver.WaitUntilExists(pageId);

            var isVisible = await Driver.IsElementVisible(pageId);
            await Assert.That(isVisible).IsTrue();
        }
    }
}
