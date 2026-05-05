using System.Text.Json;
using Microsoft.Maui.DevFlow.Driver;

namespace Sample.UITests;

public class MauiDevFlowDriver : IAsyncDisposable
{
    readonly string screenshotDir;
    AgentClient? client;

    public Platform? TargetPlatform { get; init; }
    public int Port { get; init; } = 9223;

    public MauiDevFlowDriver(string? screenshotDir = null)
    {
        this.screenshotDir = screenshotDir ?? Path.Combine(
            AppContext.BaseDirectory,
            "screenshots"
        );
        Directory.CreateDirectory(this.screenshotDir);
    }

    public AgentClient Client => client ?? throw new InvalidOperationException("Driver not connected. Call WaitForAgent first.");

    public ValueTask DisposeAsync()
    {
        client?.Dispose();
        return ValueTask.CompletedTask;
    }

    public async Task WaitForAgent(int timeoutSeconds = 120)
    {
        client = new AgentClient("localhost", Port);
        var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
        Exception? lastException = null;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                await client.GetStatusAsync();
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                await Task.Delay(1000);
            }
        }

        throw new TimeoutException(
            $"DevFlow agent did not respond within {timeoutSeconds}s",
            lastException
        );
    }

    public Task Navigate(string route)
        => Client.NavigateAsync(route);

    public async Task<List<ElementInfo>> Tree(int depth = 15)
        => await Client.GetTreeAsync(maxDepth: depth);

    public async Task<List<ElementInfo>> Query(
        string? automationId = null,
        string? type = null,
        string? text = null)
        => await Client.QueryAsync(type: type, automationId: automationId, text: text);

    async Task<ElementInfo> FindElementAsync(string automationId, int timeoutSeconds = 30)
    {
        var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
        Exception? lastException = null;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var results = await Client.QueryAsync(automationId: automationId);
                if (results.Count > 0)
                    return results[0];
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"Element with AutomationId '{automationId}' not found within {timeoutSeconds}s",
            lastException
        );
    }

    public async Task<List<ElementInfo>> WaitUntilExists(string automationId, int timeoutSeconds = 30)
    {
        var element = await FindElementAsync(automationId, timeoutSeconds);
        return [element];
    }

    public async Task WaitUntilGone(string automationId, int timeoutSeconds = 30)
    {
        var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var results = await Client.QueryAsync(automationId: automationId);
                if (results.Count == 0)
                    return;
            }
            catch
            {
                return;
            }
            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"Element with AutomationId '{automationId}' still present after {timeoutSeconds}s"
        );
    }

    public async Task Tap(string? automationId = null, string? elementId = null, string? type = null, int? index = null)
    {
        var id = elementId;
        if (id == null && automationId != null)
        {
            var results = await Client.QueryAsync(automationId: automationId);
            var target = index != null && index < results.Count ? results[index.Value] : results[0];
            id = target.Id;
        }
        else if (id == null && type != null)
        {
            var results = await Client.QueryAsync(type: type);
            var target = index != null && index < results.Count ? results[index.Value] : results[0];
            id = target.Id;
        }

        if (id == null)
            throw new InvalidOperationException("No element identifier provided for Tap");

        await Client.TapAsync(id);
    }

    public async Task Fill(string text, string? automationId = null, string? elementId = null)
    {
        var id = elementId;
        if (id == null && automationId != null)
        {
            var element = await FindElementAsync(automationId, 10);
            id = element.Id;
        }

        if (id == null)
            throw new InvalidOperationException("No element identifier provided for Fill");

        await Client.FillAsync(id, text);
    }

    public async Task Clear(string? automationId = null, string? elementId = null)
    {
        var id = elementId;
        if (id == null && automationId != null)
        {
            var element = await FindElementAsync(automationId, 10);
            id = element.Id;
        }

        if (id == null)
            throw new InvalidOperationException("No element identifier provided for Clear");

        await Client.ClearAsync(id);
    }

    public async Task<string?> GetProperty(string property, string? automationId = null, string? elementId = null)
    {
        var id = elementId;
        if (id == null && automationId != null)
        {
            var element = await FindElementAsync(automationId, 10);
            id = element.Id;
        }

        if (id == null)
            throw new InvalidOperationException("No element identifier provided for GetProperty");

        return await Client.GetPropertyAsync(id, property);
    }

    public async Task SetProperty(string property, string value, string? automationId = null, string? elementId = null)
    {
        var id = elementId;
        if (id == null && automationId != null)
        {
            var element = await FindElementAsync(automationId, 10);
            id = element.Id;
        }

        if (id == null)
            throw new InvalidOperationException("No element identifier provided for SetProperty");

        await Client.SetPropertyAsync(id, property, value);
    }

    public async Task Screenshot(string filename, string? elementAutomationId = null)
    {
        string? elId = null;
        if (elementAutomationId != null)
        {
            var element = await FindElementAsync(elementAutomationId, 10);
            elId = element.Id;
        }

        var bytes = await Client.ScreenshotAsync(elementId: elId)
            ?? throw new InvalidOperationException("Screenshot returned no data");
        var path = Path.Combine(screenshotDir, filename);
        await File.WriteAllBytesAsync(path, bytes);
    }

    public async Task Scroll(string? automationId = null, string? elementId = null, int? dy = null, int? itemIndex = null)
    {
        var id = elementId;
        if (id == null && automationId != null)
        {
            var element = await FindElementAsync(automationId, 10);
            id = element.Id;
        }

        await Client.ScrollAsync(
            elementId: id,
            deltaX: 0,
            deltaY: dy ?? 0,
            animated: true,
            itemIndex: itemIndex
        );
    }

    public async Task Resize(int width, int height)
        => await Client.ResizeAsync(width, height);

    public async Task<bool> IsElementVisible(string automationId)
    {
        try
        {
            var results = await Client.QueryAsync(automationId: automationId);
            return results.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task Back()
        => await Client.BackAsync();

    public async Task Gesture(string gestureType, string? automationId = null, string? direction = null, double? distance = null, int? durationMs = null)
    {
        string? elId = null;
        if (automationId != null)
        {
            var element = await FindElementAsync(automationId, 10);
            elId = element.Id;
        }

        await Client.GestureAsync(gestureType, elementId: elId, direction: direction, distance: distance, durationMs: durationMs);
    }
}
