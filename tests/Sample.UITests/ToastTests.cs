namespace Sample.UITests;

public abstract class ToastTests : PlatformTestBase
{
    async Task NavigateToToast()
    {
        await Driver.Navigate("//toast");
        await Driver.WaitUntilExists("ToastPage");
    }

    [Test]
    public async Task Toast_PageLoads()
    {
        await NavigateToToast();

        var isVisible = await Driver.IsElementVisible("ToastPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Toast_ShowToastButtonExists()
    {
        await NavigateToToast();

        var isVisible = await Driver.IsElementVisible("ShowToastButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Toast_ShowConfiguredToast()
    {
        await NavigateToToast();

        await Driver.Tap(automationId: "ShowToastButton");
        await Task.Delay(1000);
        await Driver.Screenshot("toast-shown.png");

        // Wait for toast to auto-dismiss
        await Task.Delay(3000);
    }

    [Test]
    public async Task Toast_ShowInfoToast()
    {
        await NavigateToToast();

        // Scroll down to themed types section
        await Driver.Scroll(dy: -400);
        await Task.Delay(300);

        await Driver.Tap(automationId: "InfoToastButton");
        await Task.Delay(1000);
        await Driver.Screenshot("toast-info.png");

        await Task.Delay(3000);
    }

    [Test]
    public async Task Toast_ShowSuccessToast()
    {
        await NavigateToToast();

        await Driver.Scroll(dy: -400);
        await Task.Delay(300);

        await Driver.Tap(automationId: "SuccessToastButton");
        await Task.Delay(1000);
        await Driver.Screenshot("toast-success.png");

        await Task.Delay(3000);
    }

    [Test]
    public async Task Toast_Screenshot()
    {
        await NavigateToToast();

        await Driver.Screenshot("toast-page.png");
    }
}
