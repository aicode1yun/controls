namespace Sample.UITests;

public abstract class ColorPickerTests : PlatformTestBase
{
    async Task NavigateToColorPicker()
    {
        await Driver.Navigate("//colorpicker");
        await Driver.WaitUntilExists("ColorPickerPage");
    }

    [Test]
    public async Task ColorPicker_PageLoads()
    {
        await NavigateToColorPicker();

        var isVisible = await Driver.IsElementVisible("ColorPickerPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task ColorPicker_ButtonExists()
    {
        await NavigateToColorPicker();

        var isVisible = await Driver.IsElementVisible("ColorPickerButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task ColorPicker_PreviewExists()
    {
        await NavigateToColorPicker();

        var isVisible = await Driver.IsElementVisible("ColorPreview");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task ColorPicker_TapOpensPickerDialog()
    {
        await NavigateToColorPicker();

        await Driver.Tap(automationId: "ColorPickerButton");
        await Task.Delay(1000);
        await Driver.Screenshot("color-picker-opened.png");

        // Navigate away to dismiss
        await Driver.Back();
        await Task.Delay(500);
    }

    [Test]
    public async Task ColorPicker_Screenshot()
    {
        await NavigateToColorPicker();

        await Driver.Screenshot("color-picker-page.png");
    }
}
