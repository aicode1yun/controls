namespace Sample.UITests;

public abstract class SignaturePadTests : PlatformTestBase
{
    async Task NavigateToSignaturePad()
    {
        await Driver.Navigate("//signaturepad");
        await Driver.WaitUntilExists("SignaturePadPage");
    }

    [Test]
    public async Task SignaturePad_PageLoads()
    {
        await NavigateToSignaturePad();

        var isVisible = await Driver.IsElementVisible("SignaturePadPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task SignaturePad_CaptureButtonExists()
    {
        await NavigateToSignaturePad();

        var isVisible = await Driver.IsElementVisible("CaptureSignatureButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task SignaturePad_OpenSignaturePanel()
    {
        await NavigateToSignaturePad();

        await Driver.Tap(automationId: "CaptureSignatureButton");
        await Task.Delay(1000);
        await Driver.Screenshot("signature-pad-opened.png");

        // Cancel the signature
        var cancelButtons = await Driver.Query(text: "Cancel");
        if (cancelButtons.Count > 0)
            await Driver.Tap(elementId: cancelButtons[0].Id);
        await Task.Delay(500);
    }

    [Test]
    public async Task SignaturePad_Screenshot()
    {
        await NavigateToSignaturePad();

        await Driver.Screenshot("signature-pad-page.png");
    }
}
