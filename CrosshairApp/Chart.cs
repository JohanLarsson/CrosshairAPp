namespace CrosshairApp;

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using PixelFormat = System.Drawing.Imaging.PixelFormat;

public sealed class Chart : FrameworkElement
{
    public static readonly DependencyProperty UseBitmapProperty = DependencyProperty.Register(
        nameof(UseBitmap),
        typeof(bool),
        typeof(Chart),
        new FrameworkPropertyMetadata(
            true,
            FrameworkPropertyMetadataOptions.AffectsRender));

    private readonly Crosshair crosshair = new();
    private Pen? pen;

    public Chart()
    {
        this.AddVisualChild(this.crosshair);
    }

    public bool UseBitmap
    {
        get => (bool)GetValue(UseBitmapProperty);
        set => SetValue(UseBitmapProperty, value);
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index) => index switch
    {
        0 => this.crosshair,
        _ => throw new InvalidOperationException(),
    };

    protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) => new PointHitTestResult(this, hitTestParameters.HitPoint);

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        this.crosshair.Position(e.GetPosition(this), this.RenderSize);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawRectangle(UseBitmap ? Brushes.Red : Brushes.Blue, null, new Rect(new Point(200,0), new Size(100,100)));
        const int n = 1_000;
        if (UseBitmap)
        {
            var bmp = new WriteableBitmap(1100, 1100, 96.0, 96.0, PixelFormats.Pbgra32, null);
            bmp.Lock();
            var  useGfx = true;
            if (useGfx)
            {
                using var bitmap = new System.Drawing.Bitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.BackBufferStride, PixelFormat.Format32bppPArgb, bmp.BackBuffer);
                using var gfx = System.Drawing.Graphics.FromImage(bitmap);
                using var pen = new System.Drawing.Pen(System.Drawing.Color.Black, 0.5f);
                for (var i = 0; i < n; i++)
                {
                    gfx.DrawLine(pen, new System.Drawing.Point(i, i), new System.Drawing.Point(i + 1, i + 1));
                    gfx.DrawLine(pen, new System.Drawing.Point(i, i + 10), new System.Drawing.Point(i + 1, i + 11));
                    gfx.DrawLine(pen, new System.Drawing.Point(i, i + 20), new System.Drawing.Point(i + 1, i + 21));
                    gfx.DrawLine(pen, new System.Drawing.Point(i, i + 30), new System.Drawing.Point(i + 1, i + 31));
                    gfx.DrawLine(pen, new System.Drawing.Point(i, i + 40), new System.Drawing.Point(i + 1, i + 41));
                    gfx.DrawLine(pen, new System.Drawing.Point(i, i + 50), new System.Drawing.Point(i + 1, i + 51));
                }
            }
            else
            {
                using var pen = new System.Drawing.Pen(System.Drawing.Color.Black, 0.5f);
                for (var i = 0; i < n; i++)
                {
                    var color = WriteableBitmapExtensions.ConvertColor(Colors.Black);
                    bmp.DrawLine(new System.Drawing.Point(i, i), new System.Drawing.Point(i + 1, i + 1), color);
                    bmp.DrawLine(new System.Drawing.Point(i, i + 10), new System.Drawing.Point(i + 1, i + 11), color);
                    bmp.DrawLine(new System.Drawing.Point(i, i + 20), new System.Drawing.Point(i + 1, i + 21), color);
                    bmp.DrawLine(new System.Drawing.Point(i, i + 30), new System.Drawing.Point(i + 1, i + 31), color);
                    bmp.DrawLine(new System.Drawing.Point(i, i + 40), new System.Drawing.Point(i + 1, i + 41), color);
                    bmp.DrawLine(new System.Drawing.Point(i, i + 50), new System.Drawing.Point(i + 1, i + 51), color);
                }
            }

            bmp.AddDirtyRect(new Int32Rect(0, 0, 1100, 1100));
            bmp.Unlock();
            drawingContext.DrawImage(bmp, new Rect(new Size(bmp.Width, bmp.Height)));
        }
        else
        {
            this.pen ??= CreatePen();
            for (var i = 0; i < n; i++)
            {
                drawingContext.DrawLine(this.pen, new Point(i, i), new Point(i + 1, i + 1));
                drawingContext.DrawLine(this.pen, new Point(i, i + 10), new Point(i + 1, i + 11));
                drawingContext.DrawLine(this.pen, new Point(i, i + 20), new Point(i + 1, i + 21));
                drawingContext.DrawLine(this.pen, new Point(i, i + 30), new Point(i + 1, i + 31));
                drawingContext.DrawLine(this.pen, new Point(i, i + 40), new Point(i + 1, i + 41));
                drawingContext.DrawLine(this.pen, new Point(i, i + 50), new Point(i + 1, i + 51));
            }

            static Pen CreatePen()
            {
                var pen = new Pen(Brushes.Black, 0.5);
                pen.Freeze();
                return pen;
            }
        }
    }
}