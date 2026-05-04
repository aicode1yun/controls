namespace Shiny.Maui.Controls.Infrastructure;

public class MauiControlFeedbackIntegrator(IReadOnlyList<IControlFeedbackHook> hooks) : IMauiInitializeService
{
    IFeedbackService? feedback;
    bool attached;

    public void Initialize(IServiceProvider services)
    {
        feedback = services.GetService<IFeedbackService>();
        if (feedback == null)
            return;

        // Cannot resolve IApplication during IMauiInitializeService.Initialize()
        // because the platform (iOS) may not be ready yet. Defer until the first
        // page is created, which guarantees the Application is fully initialized.
        Microsoft.Maui.Handlers.PageHandler.Mapper.AppendToMapping("FeedbackIntegrator", (handler, view) =>
        {
            if (attached)
                return;

            if (Application.Current is Application application)
            {
                attached = true;
                Attach(application);
            }
        });
    }

    void Attach(Application application)
    {
        foreach (var descendant in application.GetVisualTreeDescendants().Skip(1))
        {
            if (descendant is VisualElement element)
                BindElement(element);
        }

        application.DescendantAdded += OnDescendantAdded;
        application.DescendantRemoved += OnDescendantRemoved;
    }

    void OnDescendantAdded(object? sender, ElementEventArgs e)
    {
        if (e.Element is VisualElement element)
            BindElement(element);
    }

    void OnDescendantRemoved(object? sender, ElementEventArgs e)
    {
        if (e.Element is VisualElement element)
            UnbindElement(element);
    }

    void BindElement(VisualElement element)
    {
        foreach (var hook in hooks)
        {
            if (hook.TryBind(element, feedback!))
                break;
        }
    }

    void UnbindElement(VisualElement element)
    {
        foreach (var hook in hooks)
        {
            if (hook.TryUnbind(element))
                break;
        }
    }
}
