namespace Sample.UITests;

public abstract class PillTests : PlatformTestBase
{
    async Task NavigateToPills()
    {
        await Driver.Navigate("//pills");
        await Driver.WaitUntilExists("PillPage");
    }

    [Test]
    public async Task Pills_PageLoads()
    {
        await NavigateToPills();

        var isVisible = await Driver.IsElementVisible("PillPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Pills_PresetPillsAreVisible()
    {
        await NavigateToPills();

        // Check for pill text elements by querying text content
        var successPills = await Driver.Query(text: "Success");
        await Assert.That(successPills.Count).IsGreaterThanOrEqualTo(1);

        var infoPills = await Driver.Query(text: "Info");
        await Assert.That(infoPills.Count).IsGreaterThanOrEqualTo(1);

        var warningPills = await Driver.Query(text: "Warning");
        await Assert.That(warningPills.Count).IsGreaterThanOrEqualTo(1);

        var criticalPills = await Driver.Query(text: "Critical");
        await Assert.That(criticalPills.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Pills_CustomColorPillsExist()
    {
        await NavigateToPills();

        var purplePills = await Driver.Query(text: "Purple");
        await Assert.That(purplePills.Count).IsGreaterThanOrEqualTo(1);

        var tealPills = await Driver.Query(text: "Teal");
        await Assert.That(tealPills.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Pills_SizesExist()
    {
        await NavigateToPills();

        // Scroll to bottom to see sizes section
        await Driver.Scroll(dy: -300);
        await Task.Delay(300);

        var smallPills = await Driver.Query(text: "Small");
        await Assert.That(smallPills.Count).IsGreaterThanOrEqualTo(1);

        var largePills = await Driver.Query(text: "Large");
        await Assert.That(largePills.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Pills_Screenshot()
    {
        await NavigateToPills();

        await Driver.Screenshot("pills-page.png");
    }
}
