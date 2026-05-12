namespace FlashCardz.Controls;

public class SketchButtonDrawable : IDrawable
{
    public Color InkColor { get; set; } = Color.FromArgb("#1A1A1A");
    public Color DangerColor { get; set; } = Color.FromArgb("#E53935");
    public bool IsDanger { get; set; } = false;
    public string Text { get; set; } = "";

    private static readonly Random Rng = new();

    // Generates a slightly wobbly point offset
    private static float W(float max = 2.5f) =>
        (float)(Rng.NextDouble() * max * 2 - max);

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var color = IsDanger ? DangerColor : InkColor;
        canvas.StrokeColor = color;
        canvas.StrokeSize = 1.8f;
        canvas.FontColor = color;
        canvas.FontSize = 15;

        float x = dirtyRect.X + 4;
        float y = dirtyRect.Y + 4;
        float w = dirtyRect.Width - 8;
        float h = dirtyRect.Height - 8;

        // Draw wobbly rectangle using a path
        var path = new PathF();

        // Top-left corner (slightly off)
        path.MoveTo(x + W(), y + W());

        // Top edge (wobbly line segments)
        int steps = 6;
        for (int i = 1; i <= steps; i++)
        {
            float px = x + (w / steps) * i + W();
            float py = y + W(1.5f);
            path.LineTo(px, py);
        }

        // Top-right corner
        path.LineTo(x + w + W(), y + W());

        // Right edge
        for (int i = 1; i <= steps; i++)
        {
            float px = x + w + W(1.5f);
            float py = y + (h / steps) * i + W();
            path.LineTo(px, py);
        }

        // Bottom-right corner
        path.LineTo(x + w + W(), y + h + W());

        // Bottom edge
        for (int i = steps - 1; i >= 0; i--)
        {
            float px = x + (w / steps) * i + W();
            float py = y + h + W(1.5f);
            path.LineTo(px, py);
        }

        // Bottom-left corner
        path.LineTo(x + W(), y + h + W());

        // Left edge
        for (int i = steps - 1; i >= 0; i--)
        {
            float px = x + W(1.5f);
            float py = y + (h / steps) * i + W();
            path.LineTo(px, py);
        }

        path.Close();
        canvas.DrawPath(path);

        // Draw text centered
        canvas.DrawString(
            Text,
            dirtyRect.X, dirtyRect.Y,
            dirtyRect.Width, dirtyRect.Height,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}