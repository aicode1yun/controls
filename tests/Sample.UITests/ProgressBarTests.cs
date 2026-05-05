namespace Sample.UITests;

public abstract class ProgressBarTests : PlatformTestBase
{
    async Task NavigateToProgressBar()
    {
        await Driver.Navigate("//progressbar");
        await Driver.WaitUntilExists("ProgressBarPage");
    }

    [Test]
    public async Task ProgressBar_PageLoads()
    {
        await NavigateToProgressBar();

        var isVisible = await Driver.IsElementVisible("ProgressBarPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task ProgressBar_BasicProgressBarExists()
    {
        await NavigateToProgressBar();

        var isVisible = await Driver.IsElementVisible("BasicProgressBar");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task ProgressBar_IncrementButtonExists()
    {
        await NavigateToProgressBar();

        // Scroll to bottom to see the controls section
        await Driver.Scroll(dy: -500);
        await Task.Delay(300);

        var isVisible = await Driver.IsElementVisible("IncrementButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task ProgressBar_Increment()
    {
        await NavigateToProgressBar();

        await Driver.Scroll(dy: -500);
        await Task.Delay(300);

        await Driver.Tap(automationId: "IncrementButton");
        await Task.Delay(500);
        await Driver.Screenshot("progressbar-after-increment.png");
    }

    [Test]
    public async Task ProgressBar_Decrement()
    {
        await NavigateToProgressBar();

        await Driver.Scroll(dy: -500);
        await Task.Delay(300);

        await Driver.Tap(automationId: "DecrementButton");
        await Task.Delay(500);
        await Driver.Screenshot("progressbar-after-decrement.png");
    }

    [Test]
    public async Task ProgressBar_ValueLabelUpdatesOnIncrement()
    {
        await NavigateToProgressBar();

        await Driver.Scroll(dy: -500);
        await Task.Delay(300);

        var isVisible = await Driver.IsElementVisible("ProgressValueLabel");
        await Assert.That(isVisible).IsTrue();

        // Tap increment and check label is still visible
        await Driver.Tap(automationId: "IncrementButton");
        await Task.Delay(300);

        isVisible = await Driver.IsElementVisible("ProgressValueLabel");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task ProgressBar_Screenshot()
    {
        await NavigateToProgressBar();

        await Driver.Screenshot("progressbar-page.png");
    }
}
