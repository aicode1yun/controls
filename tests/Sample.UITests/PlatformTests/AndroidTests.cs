namespace Sample.UITests.PlatformTests;

[InheritsTests]
public class AndroidNavigationTests : NavigationTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidSliderTests : SliderTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidPillTests : PillTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidSecurityPinTests : SecurityPinTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidColorPickerTests : ColorPickerTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidTextEntryTests : TextEntryTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidProgressBarTests : ProgressBarTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidOverlayTests : OverlayTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidFabTests : FabTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidToastTests : ToastTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidTableViewTests : TableViewTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidSignaturePadTests : SignaturePadTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidChatTests : ChatTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidFloatingPanelTests : FloatingPanelTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidCalendarTests : CalendarTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidAutoCompleteTests : AutoCompleteTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidThemeTests : ThemeTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidOrientationTests : OrientationTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}

[InheritsTests]
public class AndroidColorReadabilityTests : ColorReadabilityTests
{
    [ClassDataSource<AndroidAppFixture>(Shared = SharedType.PerTestSession)]
    public required AndroidAppFixture AndroidFixture { get; init; }

    protected override PlatformFixture Fixture => AndroidFixture;
}
