namespace Sample.UITests;

public abstract class FloatingPanelTests : PlatformTestBase
{
    async Task NavigateToSheet()
    {
        await Driver.Navigate("//sheet");
        await Driver.WaitUntilExists("SheetPage");
    }

    [Test]
    public async Task FloatingPanel_PageLoads()
    {
        await NavigateToSheet();

        var isVisible = await Driver.IsElementVisible("SheetPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task FloatingPanel_OpenSheetButtonExists()
    {
        await NavigateToSheet();

        var isVisible = await Driver.IsElementVisible("OpenSheetButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task FloatingPanel_OpenSheet()
    {
        await NavigateToSheet();

        await Driver.Tap(automationId: "OpenSheetButton");
        await Task.Delay(1000);
        await Driver.Screenshot("floating-panel-sheet-open.png");

        // Close by tapping close button
        var closeButtons = await Driver.Query(text: "Close from inside");
        if (closeButtons.Count > 0)
            await Driver.Tap(elementId: closeButtons[0].Id);
        await Task.Delay(500);
    }

    [Test]
    public async Task FloatingPanel_Screenshot()
    {
        await NavigateToSheet();

        await Driver.Screenshot("floating-panel-page.png");
    }
}
