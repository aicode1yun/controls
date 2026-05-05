namespace Sample.UITests;

public abstract class CalendarTests : PlatformTestBase
{
    async Task NavigateToCalendar()
    {
        await Driver.Navigate("//calendar");
        await Driver.WaitUntilExists("CalendarPage");
    }

    [Test]
    public async Task Calendar_PageLoads()
    {
        await NavigateToCalendar();

        var isVisible = await Driver.IsElementVisible("CalendarPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Calendar_Screenshot()
    {
        await NavigateToCalendar();

        await Driver.Screenshot("calendar-page.png");
    }
}
