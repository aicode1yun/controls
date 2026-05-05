namespace Sample.UITests;

public abstract class ChatTests : PlatformTestBase
{
    async Task NavigateToChat()
    {
        await Driver.Navigate("//chat");
        await Driver.WaitUntilExists("ChatPage");
    }

    [Test]
    public async Task Chat_PageLoads()
    {
        await NavigateToChat();

        var isVisible = await Driver.IsElementVisible("ChatPage");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Chat_ChatViewExists()
    {
        await NavigateToChat();

        var isVisible = await Driver.IsElementVisible("ChatView");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Chat_SimulateIncomingButtonExists()
    {
        await NavigateToChat();

        var isVisible = await Driver.IsElementVisible("SimulateIncomingButton");
        await Assert.That(isVisible).IsTrue();
    }

    [Test]
    public async Task Chat_SimulateIncomingMessage()
    {
        await NavigateToChat();

        await Driver.Tap(automationId: "SimulateIncomingButton");
        await Task.Delay(1000);
        await Driver.Screenshot("chat-incoming-message.png");
    }

    [Test]
    public async Task Chat_MultipleIncomingMessages()
    {
        await NavigateToChat();

        for (var i = 0; i < 3; i++)
        {
            await Driver.Tap(automationId: "SimulateIncomingButton");
            await Task.Delay(500);
        }

        await Driver.Screenshot("chat-multiple-messages.png");
    }

    [Test]
    public async Task Chat_Screenshot()
    {
        await NavigateToChat();

        await Driver.Screenshot("chat-page.png");
    }
}
