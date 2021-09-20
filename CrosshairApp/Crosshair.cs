namespace CrosshairApp
{
    using System.Windows;
    using System.Windows.Media;



    public sealed class Crosshair : DrawingVisual
    {
        private Pen? pen;

        public void Position(Point? position, Size renderSize)
        {
            using var drawingContext = this.RenderOpen();

            if (position is { } p)
            {
                this.pen ??= CreatePen();
                drawingContext.DrawLine( this.pen, new Point(0, p.Y), new Point(renderSize.Width, p.Y));
                drawingContext.DrawLine(this.pen, new Point(p.X, 0), new Point(p.X, renderSize.Height));

                static Pen CreatePen()
                {
                    var pen = new Pen(Brushes.Black, 0.5);
                    pen.Freeze();
                    return pen;
                }
            }
        }

        protected override GeometryHitTestResult? HitTestCore(GeometryHitTestParameters hitTestParameters) => null;

        protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters) => null;
    }
}