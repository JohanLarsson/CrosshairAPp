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
                drawingContext.DrawLine(this.pen, new Point(i, i), new Point(i + 1, i + 1));
                drawingContext.DrawLine(this.pen, new Point(i, i + 10), new Point(i + 1, i + 11));
                drawingContext.DrawLine(this.pen, new Point(i, i + 20), new Point(i + 1, i + 21));
                drawingContext.DrawLine(this.pen, new Point(i, i + 30), new Point(i + 1, i + 31));
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