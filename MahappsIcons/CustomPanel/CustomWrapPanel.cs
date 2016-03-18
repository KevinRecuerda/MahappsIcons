namespace MahappsIcons.CustomPanel
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class CustomWrapPanel : Canvas
    {
        private double contentsMaxHeight;

        private double contentsMaxWidth;

        private const int MinPadding = 0;

        protected override Size MeasureOverride(Size availableSize)
        {
            this.contentsMaxHeight = this.contentsMaxWidth = 0;
            
            foreach (FrameworkElement frameworkElement in this.InternalChildren)
            {
                frameworkElement.Measure(availableSize);
                this.contentsMaxHeight = Math.Max(this.contentsMaxHeight, frameworkElement.DesiredSize.Height);
                this.contentsMaxWidth = Math.Max(this.contentsMaxWidth, frameworkElement.DesiredSize.Width);
            }

            var n = this.InternalChildren.Count;
            if (availableSize.Width < Double.PositiveInfinity && n > 0)
            {
                var nbPerLine = Convert.ToInt32(Math.Floor(availableSize.Width / this.contentsMaxWidth));
                var nbLines = Convert.ToInt32(Math.Ceiling((double)n / nbPerLine));
                return new Size(0, nbLines * (this.contentsMaxHeight + MinPadding) + MinPadding);
            }

            return Size.Empty;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var n = this.InternalChildren.Count;
            if (n == 0)
            {
                return finalSize;
            }

            var nbPerLine = Convert.ToInt32(Math.Floor(finalSize.Width / this.contentsMaxWidth));

            var stillPaddingWidth = finalSize.Width - nbPerLine * this.contentsMaxWidth;
            var widthPadding = stillPaddingWidth / (nbPerLine + 1);

            double x = 0;
            double y = MinPadding;
            for (var i = 0; i < n; i++)
            {
                if (i % nbPerLine == 0)
                {
                    x = widthPadding;
                    y += i > 0 ? this.contentsMaxHeight + MinPadding : 0;
                }
                else
                {
                    x += this.contentsMaxWidth + widthPadding;
                }
                this.InternalChildren[i].Arrange(new Rect(x, y, this.contentsMaxWidth, this.contentsMaxHeight));

                // Update background color
                var brushName = ((i % nbPerLine) % 2 == 1) ? "DefaultColor" : "AlternativColor";
                var border = this.InternalChildren[i] as Border;
                if (border != null)
                {
                    border.Background = (Brush)Application.Current.Resources[brushName];
                }
            }
            return new Size(finalSize.Width, y + this.contentsMaxHeight + MinPadding);
        }
    }
}
