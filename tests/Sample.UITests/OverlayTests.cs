namespace Sample.UITests;

public abstract class OverlayTests : PlatformTestBase
{
    async Task NavigateToOverlay()
    {
        await Driver.Navigate("//overlay");
        await Driver.WaitUntilExists("OverlayPage");
    }

    [Test]
    public async Task Overlay_PageLoads()
    {
        await NavigateToOverlay();

        var isVisible = await Driver.IsElementVisible("OverlayPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Overlay_CustomOverlayButtonExists()
    {
        await NavigateToOverlay();

        var isVisible = await Driver.IsElementVisible("ShowCustomOverlayButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Overlay_SpinnerButtonExists()
    {
        await NavigateToOverlay();

        var isVisible = await Driver.IsElementVisible("ShowSpinnerButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Overlay_ProgressButtonExists()
    {
        await NavigateToOverlay();

        var isVisible = await Driver.IsElementVisible("ShowProgressButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Overlay_ShowCustomOverlay()
    {
        await NavigateToOverlay();

        await Driver.Tap(automationId: "ShowCustomOverlayButton");
        await Task.Delay(1000);
        await Driver.Screenshot("overlay-custom-shown.png");

        // Dismiss by tapping the dismiss button inside the overlay
        var dismissButtons = await Driver.Query(text: "Dismiss");
        if (dismissButtons.Count > 0)
            await Driver.Tap(elementId: dismissButtons[0].Id);
        await Task.Delay(500);
    }

    [Test]
    public async Task Overlay_ShowSpinnerOverlay()
    {
        await NavigateToOverlay();

        await Driver.Tap(automationId: "ShowSpinnerButton");
        await Task.Delay(1000);
        await Driver.Screenshot("overlay-spinner-shown.png");

        // Wait for it to dismiss automatically or timeout
        await Task.Delay(3000);
    }

    [Test]
    public async Task Overlay_Screenshot()
    {
        await NavigateToOverlay();

        await Driver.Screenshot("overlay-page.png");
    }
}
