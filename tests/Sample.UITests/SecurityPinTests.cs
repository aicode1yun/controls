namespace Sample.UITests;

public abstract class SecurityPinTests : PlatformTestBase
{
    async Task NavigateToSecurityPin()
    {
        await Driver.Navigate("//securitypin");
        await Driver.WaitUntilExists("SecurityPinPage");
    }

    [Test]
    public async Task SecurityPin_PageLoads()
    {
        await NavigateToSecurityPin();

        var isVisible = await Driver.IsElementVisible("SecurityPinPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task SecurityPin_HiddenPinExists()
    {
        await NavigateToSecurityPin();

        var isVisible = await Driver.IsElementVisible("HiddenPin");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task SecurityPin_VisiblePinExists()
    {
        await NavigateToSecurityPin();

        var isVisible = await Driver.IsElementVisible("VisiblePin");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task SecurityPin_TapHiddenPin()
    {
        await NavigateToSecurityPin();

        // Tap the hidden pin to focus it
        await Driver.Tap(automationId: "HiddenPin");
        await Task.Delay(500);
        await Driver.Screenshot("security-pin-focused.png");
    }

    [Test]
    public async Task SecurityPin_Screenshot()
    {
        await NavigateToSecurityPin();

        await Driver.Screenshot("security-pin-page.png");
    }
}
