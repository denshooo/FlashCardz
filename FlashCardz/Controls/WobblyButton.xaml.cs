namespace FlashCardz.Controls;

public partial class WobblyButton : ContentView
{
    private readonly SketchButtonDrawable _drawable = new();

    // Bindable text property
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(WobblyButton), "",
            propertyChanged: (b, _, n) =>
            {
                var btn = (WobblyButton)b;
                btn._drawable.Text = (string)n;
                btn.Canvas.Invalidate();
            });

    // Bindable IsDanger property
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

    // Tap event the page can subscribe to
    public event EventHandler? Clicked;

    public WobblyButton()
    {
        InitializeComponent();
        Canvas.Drawable = _drawable;
    }

    private async void OnTapped(object? sender, TappedEventArgs e)
    {
        // Wiggle animation on tap
        await Canvas.RotateTo(3, 60, Easing.SpringOut);
        await Canvas.RotateTo(-3, 60, Easing.SpringOut);
        await Canvas.RotateTo(0, 60, Easing.SpringOut);

        // Redraw border (makes it look redrawn each tap)
        Canvas.Invalidate();

        Clicked?.Invoke(this, EventArgs.Empty);
    }
}