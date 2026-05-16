namespace FlashCardz.Controls;

public class WobblyBorderDrawable : IDrawable
{
    public Color StrokeColor { get; set; } = Color.FromArgb("#1A1A1A");
    public Color FillColor { get; set; } = Color.FromArgb("#FAFAFA");
    public float StrokeWidth { get; set; } = 1.8f;

    private static readonly Random Rng = new();
    private static float W(float max = 2f) =>
        (float)(Rng.NextDouble() * max * 2 - max);

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float x = dirtyRect.X + 4;
        float y = dirtyRect.Y + 4;
        float w = dirtyRect.Width - 8;
        float h = dirtyRect.Height - 8;

        int steps = 8;

        var path = new PathF();
        path.MoveTo(x + W(), y + W());
        for (int i = 1; i <= steps; i++)
            path.LineTo(x + (w / steps) * i + W(), y + W(1.5f));

        path.LineTo(x + w + W(), y + W());
        for (int i = 1; i <= steps; i++)
            path.LineTo(x + w + W(1.5f), y + (h / steps) * i + W());

        path.LineTo(x + w + W(), y + h + W());
        for (int i = steps - 1; i >= 0; i--)
            path.LineTo(x + (w / steps) * i + W(), y + h + W(1.5f));

        path.LineTo(x + W(), y + h + W());
        for (int i = steps - 1; i >= 0; i--)
            path.LineTo(x + W(1.5f), y + (h / steps) * i + W());

        path.Close();

        // Fill FIRST then stroke on top
        canvas.FillColor = FillColor;
        canvas.FillPath(path);

        canvas.StrokeColor = StrokeColor;
        canvas.StrokeSize = StrokeWidth;
        canvas.DrawPath(path);
    }
}