namespace CrosshairApp
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    public sealed class Chart : FrameworkElement
    {
        private readonly Crosshair crosshair = new();
        private Pen? pen;

        public Chart()
        {
            this.AddVisualChild(this.crosshair);
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
            this.pen ??= CreatePen();
            for (var i = 0; i < 1000; i++)
            {
                drawingContext.DrawRectangle(Brushes.Red, null, new Rect(new Point(i, i), new Size(1, 1)));
                drawingContext.DrawRectangle(Brushes.Blue, null, new Rect(new Point(i + 2, i + 2), new Size(2, 2)));
                drawingContext.DrawRectangle(Brushes.Yellow, null, new Rect(new Point(i + 3, i + 3), new Size(3, 3)));
                drawingContext.DrawRectangle(Brushes.HotPink, null, new Rect(new Point(i + 4, i + 4), new Size(4, 4)));
                drawingContext.DrawLine(this.pen, new Point(i, i + 1), new Point(i - 1, i));
                drawingContext.DrawLine(this.pen, new Point(i, i + 2), new Point(i - 1, i));
                drawingContext.DrawLine(this.pen, new Point(i, i + 3), new Point(i - 1, i));
                drawingContext.DrawLine(this.pen, new Point(i, i + 4), new Point(i - 1, i));
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