namespace Sample.UITests.PlatformTests;

[InheritsTests]
public class IosNavigationTests : NavigationTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosSliderTests : SliderTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosPillTests : PillTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosSecurityPinTests : SecurityPinTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosColorPickerTests : ColorPickerTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosTextEntryTests : TextEntryTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosProgressBarTests : ProgressBarTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosOverlayTests : OverlayTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosFabTests : FabTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosToastTests : ToastTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosTableViewTests : TableViewTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosSignaturePadTests : SignaturePadTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosChatTests : ChatTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosFloatingPanelTests : FloatingPanelTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosCalendarTests : CalendarTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosAutoCompleteTests : AutoCompleteTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosThemeTests : ThemeTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosOrientationTests : OrientationTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}

[InheritsTests]
public class IosColorReadabilityTests : ColorReadabilityTests
{
    [ClassDataSource<IosAppFixture>(Shared = SharedType.PerTestSession)]
    public required IosAppFixture IosFixture { get; init; }

    protected override PlatformFixture Fixture => IosFixture;
}
