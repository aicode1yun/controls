namespace Sample.UITests;

public abstract class TableViewTests : PlatformTestBase
{
    async Task NavigateToTableView()
    {
        await Driver.Navigate("//basic");
        await Driver.WaitUntilExists("BasicSettingsPage");
    }

    [Test]
    public async Task TableView_PageLoads()
    {
        await NavigateToTableView();

        var isVisible = await Driver.IsElementVisible("BasicSettingsPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task TableView_SettingsTableViewExists()
    {
        await NavigateToTableView();

        var isVisible = await Driver.IsElementVisible("SettingsTableView");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task TableView_ToggleSectionsExist()
    {
        await NavigateToTableView();

        // Check for section header and cell titles
        var wifiCells = await Driver.Query(text: "Wi-Fi");
        await Assert.That(wifiCells.Count).IsGreaterThanOrEqualTo(1);

        var bluetoothCells = await Driver.Query(text: "Bluetooth");
        await Assert.That(bluetoothCells.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task TableView_LabelsAndCommandsSectionExists()
    {
        await NavigateToTableView();

        var versionCells = await Driver.Query(text: "Version");
        await Assert.That(versionCells.Count).IsGreaterThanOrEqualTo(1);

        var aboutCells = await Driver.Query(text: "About");
        await Assert.That(aboutCells.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task TableView_ThemeRadioSectionExists()
    {
        await NavigateToTableView();

        // Scroll to theme section
        await Driver.Scroll(automationId: "SettingsTableView", dy: -300);
        await Task.Delay(300);

        var lightRadio = await Driver.Query(text: "Light");
        await Assert.That(lightRadio.Count).IsGreaterThanOrEqualTo(1);

        var darkRadio = await Driver.Query(text: "Dark");
        await Assert.That(darkRadio.Count).IsGreaterThanOrEqualTo(1);

        var systemRadio = await Driver.Query(text: "System");
        await Assert.That(systemRadio.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task TableView_TapWifiSwitch()
    {
        await NavigateToTableView();

        var wifiCells = await Driver.Query(text: "Wi-Fi");
        if (wifiCells.Count > 0)
        {
            await Driver.Tap(elementId: wifiCells[0].Id);
            await Task.Delay(500);
            await Driver.Screenshot("tableview-wifi-toggled.png");
        }
    }

    [Test]
    public async Task TableView_ScrollThroughAllSections()
    {
        await NavigateToTableView();

        await Driver.Screenshot("tableview-top.png");

        // Scroll through the table
        await Driver.Scroll(automationId: "SettingsTableView", dy: -300);
        await Task.Delay(300);
        await Driver.Screenshot("tableview-middle.png");

        await Driver.Scroll(automationId: "SettingsTableView", dy: -300);
        await Task.Delay(300);
        await Driver.Screenshot("tableview-bottom.png");
    }

    [Test]
    public async Task TableView_Screenshot()
    {
        await NavigateToTableView();

        await Driver.Screenshot("tableview-page.png");
    }
}
