namespace FlashCardz.Controls;

public partial class WobblyButton : ContentView
{
    private readonly SketchButtonDrawable _drawable = new();

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(WobblyButton), "",
            propertyChanged: (b, _, n) =>
            {
                var btn = (WobblyButton)b;
                btn._drawable.Text = (string)n;
                btn.UpdateSize();
                btn.Canvas.Invalidate();
            });

    public static readonly BindableProperty IsDangerProperty =
        BindableProperty.Create(nameof(IsDanger), typeof(bool), typeof(WobblyButton), false,
            propertyChanged: (b, _, n) =>
            {
                var btn = (WobblyButton)b;
                btn._drawable.IsDanger = (bool)n;
                btn.Canvas.Invalidate();
            });

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsDanger
    {
        get => (bool)GetValue(IsDangerProperty);
        set => SetValue(IsDangerProperty, value);
    }

    public event EventHandler? Clicked;

    public WobblyButton()
    {
        InitializeComponent();
        Canvas.Drawable = _drawable;
    }

    private void UpdateSize()
    {
        // Measure text length and set size accordingly
        var textLength = Text?.Length ?? 0;
        var calculatedWidth = Math.Max(80, textLength * 11 + 32);
        var calculatedHeight = HeightRequest > 0 ? HeightRequest : 44;

        Canvas.WidthRequest = WidthRequest > 0 ? WidthRequest : calculatedWidth;
        Canvas.HeightRequest = calculatedHeight;
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(WidthRequest) ||
            propertyName == nameof(HeightRequest))
        {
            UpdateSize();
            Canvas.Invalidate();
        }
    }

    private async void OnTapped(object? sender, TappedEventArgs e)
    {
        await Canvas.RotateTo(3, 60, Easing.SpringOut);
        await Canvas.RotateTo(-3, 60, Easing.SpringOut);
        await Canvas.RotateTo(0, 60, Easing.SpringOut);
        Canvas.Invalidate();
        Clicked?.Invoke(this, EventArgs.Empty);
    }
}