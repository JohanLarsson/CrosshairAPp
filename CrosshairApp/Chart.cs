namespace CrosshairApp
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    public sealed class Chart : FrameworkElement
    {
        private readonly Crosshair crosshair = new();

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
    }
}