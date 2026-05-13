using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}