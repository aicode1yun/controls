namespace Sample.UITests;

public abstract class SliderTests : PlatformTestBase
{
    async Task NavigateToSlider()
    {
        await Driver.Navigate("//slider");
        await Driver.WaitUntilExists("SliderPage");
    }

    [Test]
    public async Task Slider_PageLoads()
    {
        await NavigateToSlider();

        var isVisible = await Driver.IsElementVisible("SliderPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Slider_TemperatureSliderExists()
    {
        await NavigateToSlider();

        var isVisible = await Driver.IsElementVisible("TemperatureSlider");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Slider_TemperatureLabelDisplaysValue()
    {
        await NavigateToSlider();

        var isVisible = await Driver.IsElementVisible("TemperatureLabel");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Slider_IntensitySliderExists()
    {
        await NavigateToSlider();

        var isVisible = await Driver.IsElementVisible("IntensitySlider");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Slider_VolumeSliderExists()
    {
        await NavigateToSlider();

        var isVisible = await Driver.IsElementVisible("VolumeSlider");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Slider_InteractWithTemperature()
    {
        await NavigateToSlider();

        // Gesture swipe on the slider to change its value
        await Driver.Gesture("swipe", automationId: "TemperatureSlider", direction: "right", distance: 100);
        await Task.Delay(500);
        await Driver.Screenshot("slider-temperature-changed.png");
    }

    [Test]
    public async Task Slider_Screenshot()
    {
        await NavigateToSlider();

        await Driver.Screenshot("slider-page.png");
    }
}
