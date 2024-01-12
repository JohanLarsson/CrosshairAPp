namespace CrosshairApp;

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public static class WriteableBitmapExtensions
{
    internal const int SizeOfArgb = 4;
    private const byte INSIDE = 0; // 0000
    private const byte LEFT = 1;   // 0001
    private const byte RIGHT = 2;  // 0010
    private const byte BOTTOM = 4; // 0100
    private const byte TOP = 8;    // 1000

    public static int IntColor(double opacity, System.Windows.Media.Color color)
    {
        if (opacity is < 0.0 or > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity), "Opacity must be between 0.0 and 1.0");
        }

        color.A = (byte)(color.A * opacity);

        return IntColor(color);
    }

    public static int IntColor(System.Windows.Media.Color color)
    {
        if (color.A == 0)
        {
            return 0;
        }

        var a = color.A + 1;
        return (color.A << 24)
               | ((byte)((color.R * a) >> 8) << 16)
               | ((byte)((color.G * a) >> 8) << 8)
               | ((byte)((color.B * a) >> 8));
    }

    public static void Clear(this WriteableBitmap bmp, int color)
    {
        unsafe
        {

            var pixels = new Span<int>((int*)bmp.BackBuffer, bmp.PixelWidth * bmp.PixelHeight);
            // Fill first line
            for (var i = 0; i < bmp.PixelWidth; i++)
            {
                pixels[i] = color;
            }

            // Copy first line
            var line = pixels.Slice(0, bmp.PixelWidth);
            for (var i = 0; i < bmp.PixelHeight; i++)
            {
                line.CopyTo(pixels.Slice(i * bmp.PixelWidth, line.Length));
            }
        }
    }

    public static void Clear(this WriteableBitmap bmp, Color color) => Clear(bmp, IntColor(color));

    /// <summary>
    /// Draws a filled rectangle with or without alpha blending (default = false).
    /// x2 has to be greater than x1 and y2 has to be greater than y1.
    /// </summary>
    /// <param name="bmp">The WriteableBitmap.</param>
    /// <param name="x1">The x-coordinate of the bounding rectangle's left side.</param>
    /// <param name="y1">The y-coordinate of the bounding rectangle's top side.</param>
    /// <param name="x2">The x-coordinate of the bounding rectangle's right side.</param>
    /// <param name="y2">The y-coordinate of the bounding rectangle's bottom side.</param>
    /// <param name="color">The color.</param>
    public static void DrawRectangle(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color)
    {
        if ((x1 < 0 && x2 < 0) ||
            (y1 < 0 && y2 < 0) ||
            (x1 >= bmp.PixelWidth && x2 >= bmp.PixelWidth) ||
            (y1 >= bmp.PixelHeight && y2 >= bmp.PixelHeight))
        {
            return;
        }

        x1 = Math.Clamp(x1, 0, bmp.PixelWidth - 1);
        x2 = Math.Clamp(x2, 0, bmp.PixelWidth - 1);
        y1 = Math.Clamp(y1, 0, bmp.PixelHeight - 1);
        y2 = Math.Clamp(y2, 0, bmp.PixelHeight - 1);
        var left = Math.Min(x1, x2);
        var width = Math.Abs(x1 - x2) + 1;
        var top = Math.Min(y1, y2);
        var bottom = Math.Max(y1, y2);
        unsafe
        {
            var pixels = new Span<int>((int*)bmp.BackBuffer, bmp.PixelWidth * bmp.PixelHeight);
            // Fill first line
            var start = top * bmp.PixelWidth + left;
            var end = start + width;
            for (var i = start; i < end; i++)
            {
                pixels[i] = color;
            }

            // Copy first line
            var line = pixels.Slice(start, width);
            var y = top + 1;
            while (y <= bottom)
            {
                line.CopyTo(pixels.Slice(y * bmp.PixelWidth + left, line.Length));
                y++;
            }
        }
    }

    public static void DrawRectangle(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color) => bmp.DrawRectangle(x1, y1, x2, y2, IntColor(color));

    public static void DrawRectangle(this WriteableBitmap bmp, System.Drawing.Point p1, System.Drawing.Point p2, Color color) => bmp.DrawRectangle(p1.X, p1.Y, p2.X, p2.Y, IntColor(color));

    public static void DrawHorizontalLine(this WriteableBitmap bmp, int x1, int x2, int y, int color)
    {
        if ((x1 < 0 && x2 < 0) ||
            y < 0 ||
            (x1 >= bmp.PixelWidth && x2 >= bmp.PixelWidth) ||
            y >= bmp.PixelHeight)
        {
            return;
        }

        x1 = Math.Clamp(x1, 0, bmp.PixelWidth - 1);
        x2 = Math.Clamp(x2, 0, bmp.PixelWidth - 1);
        var left = Math.Min(x1, x2);
        var width = Math.Abs(x1 - x2) + 1;
        unsafe
        {
            var pixels = new Span<int>((int*)bmp.BackBuffer, bmp.PixelWidth * bmp.PixelHeight);
            // Fill first line
            var start = y * bmp.PixelWidth + left;
            var end = start + width;
            for (var i = start; i < end; i++)
            {
                pixels[i] = color;
            }
        }
    }

    public static void DrawHorizontalLine(this WriteableBitmap bmp, int x1, int x2, int y, Color color)
        => DrawHorizontalLine(bmp, x1, x2, y, IntColor(color));

    public static void DrawVerticalLine(this WriteableBitmap bmp, int x, int y1, int y2, int color)
    {
        if (x < 0 ||
            x >= bmp.PixelWidth ||
            (y1 < 0 && y2 < 0) ||
            (y1 >= bmp.PixelHeight && y2 >= bmp.PixelHeight))
        {
            return;
        }

        y1 = Math.Clamp(y1, 0, bmp.PixelHeight - 1);
        y2 = Math.Clamp(y2, 0, bmp.PixelHeight - 1);
        var top = Math.Min(y1, y2);
        var bottom = Math.Max(y1, y2);
        unsafe
        {
            var pixels = new Span<int>((int*)bmp.BackBuffer, bmp.PixelWidth * bmp.PixelHeight);
            var y = top;
            while (y <= bottom)
            {
                pixels[y * bmp.PixelWidth + x] = color;
                y++;
            }
        }
    }

    public static void DrawVerticalLine(this WriteableBitmap bmp, int x, int y1, int y2, Color color) =>
        DrawVerticalLine(bmp, x, y1, y2, IntColor(color));

    public static void DrawLine(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color, Rect? clip = null)
    {
        unsafe
        {
            var clipRect = clip ?? new Rect(new Size(bmp.PixelWidth, bmp.PixelHeight));
            // Perform cohen-sutherland clipping if either point is out of the viewport
            if (!CohenSutherlandLineClip(clipRect, ref x1, ref y1, ref x2, ref y2))
            {
                return;
            }

            var pixels = (int*)bmp.BackBuffer;

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine slope (absolute value)
            int lenX, lenY;
            if (dy >= 0)
            {
                lenY = dy;
            }
            else
            {
                lenY = -dy;
            }

            if (dx >= 0)
            {
                lenX = dx;
            }
            else
            {
                lenX = -dx;
            }

            const int PRECISION_SHIFT = 8;
            if (lenX > lenY)
            { // x increases by +/- 1
                if (dx < 0)
                {
                    (x2, x1) = (x1, x2);
                    (y2, y1) = (y1, y2);
                }

                // Init steps and start
                int incy = (dy << PRECISION_SHIFT) / dx;

                int y1s = y1 << PRECISION_SHIFT;
                int y2s = y2 << PRECISION_SHIFT;
                int hs = bmp.PixelHeight << PRECISION_SHIFT;

                if (y1 < y2)
                {
                    if (y1 >= clipRect.Bottom || y2 < clipRect.Top)
                    {
                        return;
                    }
                    if (y1s < 0)
                    {
                        if (incy == 0)
                        {
                            return;
                        }
                        int oldy1s = y1s;
                        // Find lowest y1s that is greater or equal than 0.
                        y1s = incy - 1 + ((y1s + 1) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s >= hs)
                    {
                        if (incy != 0)
                        {
                            // Find highest y2s that is less or equal than ws - 1.
                            // y2s = y1s + n * incy. Find n.
                            y2s = hs - 1 - (hs - 1 - y1s) % incy;
                            x2 = x1 + (y2s - y1s) / incy;
                        }
                    }
                }
                else
                {
                    if (y2 > clipRect.Bottom || y1 < clipRect.Top)
                    {
                        return;
                    }
                    if (y1s >= hs)
                    {
                        if (incy == 0)
                        {
                            return;
                        }
                        int oldy1s = y1s;
                        // Find highest y1s that is less or equal than ws - 1.
                        // y1s = oldy1s + n * incy. Find n.
                        y1s = hs - 1 + (incy - (hs - 1 - oldy1s) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s < 0)
                    {
                        if (incy != 0)
                        {
                            // Find lowest y2s that is greater or equal than 0.
                            // y2s = y1s + n * incy. Find n.
                            y2s = y1s % incy;
                            x2 = x1 + (y2s - y1s) / incy;
                        }
                    }
                }

                if (x1 < 0)
                {
                    y1s -= incy * x1;
                    x1 = 0;
                }
                if (x2 >= bmp.PixelWidth)
                {
                    x2 = bmp.PixelWidth - 1;
                }

                int ys = y1s;

                // Walk the line!
                int y = ys >> PRECISION_SHIFT;
                int previousY = y;
                int index = x1 + y * bmp.PixelWidth;
                int k = incy < 0 ? 1 - bmp.PixelWidth : 1 + bmp.PixelWidth;
                for (int x = x1; x <= x2; ++x)
                {
                    pixels[index] = color;
                    ys += incy;
                    y = ys >> PRECISION_SHIFT;
                    if (y != previousY)
                    {
                        previousY = y;
                        index += k;
                    }
                    else
                    {
                        ++index;
                    }
                }
            }
            else
            {
                // Prevent division by zero
                if (lenY == 0)
                {
                    return;
                }
                if (dy < 0)
                {
                    int t = x1;
                    x1 = x2;
                    x2 = t;
                    t = y1;
                    y1 = y2;
                    y2 = t;
                }

                // Init steps and start
                int x1s = x1 << PRECISION_SHIFT;
                int x2s = x2 << PRECISION_SHIFT;
                int ws = bmp.PixelWidth << PRECISION_SHIFT;

                int incx = (dx << PRECISION_SHIFT) / dy;

                if (x1 < x2)
                {
                    if (x1 >= clipRect.Right || x2 < clipRect.Left)
                    {
                        return;
                    }
                    if (x1s < 0)
                    {
                        if (incx == 0)
                        {
                            return;
                        }
                        int oldx1s = x1s;
                        // Find lowest x1s that is greater or equal than 0.
                        x1s = incx - 1 + ((x1s + 1) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s >= ws)
                    {
                        if (incx != 0)
                        {
                            // Find highest x2s that is less or equal than ws - 1.
                            // x2s = x1s + n * incx. Find n.
                            x2s = ws - 1 - (ws - 1 - x1s) % incx;
                            y2 = y1 + (x2s - x1s) / incx;
                        }
                    }
                }
                else
                {
                    if (x2 >= clipRect.Right || x1 < clipRect.Left)
                    {
                        return;
                    }
                    if (x1s >= ws)
                    {
                        if (incx == 0)
                        {
                            return;
                        }
                        int oldx1s = x1s;
                        // Find highest x1s that is less or equal than ws - 1.
                        // x1s = oldx1s + n * incx. Find n.
                        x1s = ws - 1 + (incx - (ws - 1 - oldx1s) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s < 0)
                    {
                        if (incx != 0)
                        {
                            // Find lowest x2s that is greater or equal than 0.
                            // x2s = x1s + n * incx. Find n.
                            x2s = x1s % incx;
                            y2 = y1 + (x2s - x1s) / incx;
                        }
                    }
                }

                if (y1 < 0)
                {
                    x1s -= incx * y1;
                    y1 = 0;
                }
                if (y2 >= bmp.PixelHeight)
                {
                    y2 = bmp.PixelHeight - 1;
                }

                long index = x1s;
                int indexBaseValue = y1 * bmp.PixelWidth;

                // Walk the line!
                var inc = (bmp.PixelWidth << PRECISION_SHIFT) + incx;
                for (int y = y1; y <= y2; ++y)
                {
                    pixels[indexBaseValue + (index >> PRECISION_SHIFT)] = color;
                    index += inc;
                }
            }
        }
    }

    public static void DrawLine(this WriteableBitmap bmp, System.Drawing.Point p1, System.Drawing.Point p2, int color, Rect? clipRect = null)
        => DrawLine(bmp, p1.X, p1.Y, p2.X, p2.Y, color, clipRect);

    public static void DrawLine(this WriteableBitmap bmp, System.Drawing.Point p1, System.Drawing.Point p2, System.Windows.Media.Color color, Rect? clipRect = null)
        => DrawLine(bmp, p1.X, p1.Y, p2.X, p2.Y, IntColor(color), clipRect);

    internal static bool CohenSutherlandLineClip(Rect extents, ref int xi0, ref int yi0, ref int xi1, ref int yi1)
    {
        double x0 = xi0;
        double y0 = yi0;
        double x1 = xi1;
        double y1 = yi1;

        var isValid = CohenSutherlandLineClip(extents, ref x0, ref y0, ref x1, ref y1);

        // Update the clipped line
        xi0 = (int)x0;
        yi0 = (int)y0;
        xi1 = (int)x1;
        yi1 = (int)y1;

        return isValid;
    }

    /// <summary>
    /// Cohen–Sutherland clipping algorithm clips a line from
    /// P0 = (x0, y0) to P1 = (x1, y1) against a rectangle with 
    /// diagonal from (xmin, ymin) to (xmax, ymax).
    /// </summary>
    /// <remarks>See http://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm for details</remarks>
    /// <returns>a list of two points in the resulting clipped line, or zero</returns>
    internal static bool CohenSutherlandLineClip(Rect extents, ref double x0, ref double y0, ref double x1, ref double y1)
    {
        // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
        byte outcode0 = ComputeOutCode(extents, x0, y0);
        byte outcode1 = ComputeOutCode(extents, x1, y1);

        // No clipping if both points lie inside viewport
        if (outcode0 == INSIDE && outcode1 == INSIDE)
            return true;

        bool isValid = false;

        while (true)
        {
            // Bitwise OR is 0. Trivially accept and get out of loop
            if ((outcode0 | outcode1) == 0)
            {
                isValid = true;
                break;
            }
            // Bitwise AND is not 0. Trivially reject and get out of loop
            else if ((outcode0 & outcode1) != 0)
            {
                break;
            }
            else
            {
                // failed both tests, so calculate the line segment to clip
                // from an outside point to an intersection with clip edge
                double x, y;

                // At least one endpoint is outside the clip rectangle; pick it.
                byte outcodeOut = (outcode0 != 0) ? outcode0 : outcode1;

                // Now find the intersection point;
                // use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
                if ((outcodeOut & TOP) != 0)
                {   // point is above the clip rectangle
                    x = x0 + (x1 - x0) * (extents.Top - y0) / (y1 - y0);
                    y = extents.Top;
                }
                else if ((outcodeOut & BOTTOM) != 0)
                { // point is below the clip rectangle
                    x = x0 + (x1 - x0) * (extents.Bottom - y0) / (y1 - y0);
                    y = extents.Bottom;
                }
                else if ((outcodeOut & RIGHT) != 0)
                {  // point is to the right of clip rectangle
                    y = y0 + (y1 - y0) * (extents.Right - x0) / (x1 - x0);
                    x = extents.Right;
                }
                else if ((outcodeOut & LEFT) != 0)
                {   // point is to the left of clip rectangle
                    y = y0 + (y1 - y0) * (extents.Left - x0) / (x1 - x0);
                    x = extents.Left;
                }
                else
                {
                    x = double.NaN;
                    y = double.NaN;
                }

                // Now we move outside point to intersection point to clip
                // and get ready for next pass.
                if (outcodeOut == outcode0)
                {
                    x0 = x;
                    y0 = y;
                    outcode0 = ComputeOutCode(extents, x0, y0);
                }
                else
                {
                    x1 = x;
                    y1 = y;
                    outcode1 = ComputeOutCode(extents, x1, y1);
                }
            }
        }

        return isValid;
    }

    /// <summary>
    /// Compute the bit code for a point (x, y) using the clip rectangle
    /// bounded diagonally by (xmin, ymin), and (xmax, ymax)
    /// ASSUME THAT xmax , xmin , ymax and ymin are global constants.
    /// </summary>
    /// <param name="extents">The extents.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns></returns>
    private static byte ComputeOutCode(Rect extents, double x, double y)
    {
        // initialized as being inside of clip window
        byte code = INSIDE;

        if (x < extents.Left)           // to the left of clip window
            code |= LEFT;
        else if (x > extents.Right)     // to the right of clip window
            code |= RIGHT;
        if (y > extents.Bottom)         // below the clip window
            code |= BOTTOM;
        else if (y < extents.Top)       // above the clip window
            code |= TOP;

        return code;
    }
}
