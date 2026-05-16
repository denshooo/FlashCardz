namespace FlashCardz.Controls;

public partial class WobblyBorder : ContentView
{
    private readonly WobblyBorderDrawable _drawable = new();

    public static readonly BindableProperty WobblyFillColorProperty =
        BindableProperty.Create(nameof(WobblyFillColor), typeof(Color),
            typeof(WobblyBorder), Color.FromArgb("#FAFAFA"),
            propertyChanged: (b, _, n) =>
            {
                var wb = (WobblyBorder)b;
                wb._drawable.FillColor = (Color)n;
                wb.BorderCanvas.Invalidate();
            });

    public static readonly BindableProperty WobblyStrokeColorProperty =
        BindableProperty.Create(nameof(WobblyStrokeColor), typeof(Color),
            typeof(WobblyBorder), Color.FromArgb("#1A1A1A"),
            propertyChanged: (b, _, n) =>
            {
                var wb = (WobblyBorder)b;
                wb._drawable.StrokeColor = (Color)n;
                wb.BorderCanvas.Invalidate();
            });

    public Color WobblyFillColor
    {
        get => (Color)GetValue(WobblyFillColorProperty);
        set => SetValue(WobblyFillColorProperty, value);
    }

    public Color WobblyStrokeColor
    {
        get => (Color)GetValue(WobblyStrokeColorProperty);
        set => SetValue(WobblyStrokeColorProperty, value);
    }

    // Restored: needed so XAML child content is routed into InnerContent
    // instead of ContentView's own Content slot (which would clash with
    // the Grid we have in the XAML).
    public new View? Content
    {
        get => InnerContent.Content;
        set => InnerContent.Content = value;
    }

    public WobblyBorder()
    {
        InitializeComponent();
        BorderCanvas.Drawable = _drawable;
    }

    // Fires after every layout pass — redraws the border at the correct size.
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (width > 0 && height > 0)
            BorderCanvas.Invalidate();
    }

    // Forces the inner Grid (and everything inside it) to be arranged
    // at the full allocated bounds, fixing vertical centering on Android.
    [Obsolete("Obsolete")]
    protected override Size ArrangeOverride(Rect bounds)
    {
        var result = base.ArrangeOverride(bounds);

        // Explicitly arrange the root Grid child to fill the entire bounds
        if (Children.Count > 0 && Children[0] is Grid grid)
        {
            grid.Arrange(new Rect(0, 0, bounds.Width, bounds.Height));
            BorderCanvas.Invalidate();
        }

        return result;
    }
}