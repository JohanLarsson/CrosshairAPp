namespace CrosshairApp;

using System.Windows;
using System.Windows.Input;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        //var p = e.GetPosition(this);
        //var renderSize = this.RenderSize;
        //this.HorizontalLine.X1 = 0;
        //this.HorizontalLine.Y1 = p.Y;
        //this.HorizontalLine.X2 = renderSize.Width;
        //this.HorizontalLine.Y2 = p.Y;

        //this.VerticalLine.X1 = p.X;
        //this.VerticalLine.Y1 = 0;
        //this.VerticalLine.X2 = p.X;
        //this.VerticalLine.Y1 = renderSize.Height;
    }

    private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            Chart.Mode = Chart.Mode switch
            {
                Chart.Gfx => Chart.Direct,
                Chart.Direct => Chart.DrawingContext,
                _ => Chart.Gfx,
            };
        }
    }
}