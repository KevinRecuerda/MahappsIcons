namespace MahappsIcons
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int ToggleButtonSize = 42;
        private const int RectangleSize = 18;
        private const int MarginOnToggleButton = 2;
        private const int PaddingOnStackPanel = 10;

        private Dictionary<string, FrameworkElement> iconsView;

        private CancellationTokenSource cancellationTokenSource;

        public MainWindow()
        {
            this.InitializeComponent();

            this.Loaded += (obj, e) => this.Fill();
            this.Filter.TextChanged += (obj, e) => this.UpdateSelection(obj as TextBox);
        }

        private void UpdateSelection(TextBox textBox)
        {
            if (this.cancellationTokenSource != null)
                this.cancellationTokenSource.Cancel();
            
            var filter = textBox.Text;

            this.cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = this.cancellationTokenSource.Token;

            Task.Delay(300, cancellationToken)
                .ContinueWith(
                    t => 
                    { 
                        if (!cancellationToken.IsCancellationRequested)
                            this.FilterIcons(filter); 
                    }, 
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void FilterIcons(string filter)
        {
            this.CustomPanel.Children.Clear();
            this.iconsView.Where(x => x.Key.Contains(filter) || string.IsNullOrEmpty(filter))
                          .ToList()
                          .ForEach(x => this.CustomPanel.Children.Add(x.Value));
        }

        private void Fill()
        {
            var icons = Application.Current.Resources.MergedDictionaries[6].Keys;

            this.AddIcons(icons.OfType<string>());
        }

        private void AddIcons(IEnumerable<string> icons)
        {
            this.iconsView = new Dictionary<string, FrameworkElement>();

            foreach (var icon in icons.OrderBy(s => s))
            {
                var rectangle = CreateRectangle(icon);
                var toggleButton = CreateToggleButton(rectangle);

                var textBlock = CreateTextBlock(icon);

                var stackPanel = CreateStackPanel(toggleButton, textBlock);
                var border = CreateBorder(stackPanel);

                this.CustomPanel.Children.Add(border);
                this.iconsView.Add(icon, border);
            }
        }

        private static Border CreateBorder(StackPanel stackPanel)
        {
            return new Border
                             {
                                 Padding = new Thickness(PaddingOnStackPanel),
                                 Child = stackPanel
                             };
        }

        private static StackPanel CreateStackPanel(ToggleButton toggleButton, TextBlock textBlock)
        {
            return new StackPanel
                                 {
                                     Orientation = Orientation.Vertical,
                                     Margin = new Thickness(PaddingOnStackPanel),
                                     Children = { toggleButton, textBlock }
                                 };
        }

        private static TextBlock CreateTextBlock(string icon)
        {
            return new TextBlock
                                {
                                    Text = icon,
                                    TextAlignment = TextAlignment.Center,
                                    Foreground = (Brush)Application.Current.Resources["Orange"]
                                };
        }

        private static ToggleButton CreateToggleButton(Rectangle rectangle)
        {
            return new ToggleButton
                                   {
                                       Width = ToggleButtonSize,
                                       Height = ToggleButtonSize,
                                       Margin = new Thickness(MarginOnToggleButton),
                                       Style = (Style)Application.Current.Resources["MetroCircleToggleButtonStyle"],
                                       Content = rectangle,
                                   };
        }

        private static Rectangle CreateRectangle(string icon)
        {
            return new Rectangle
                                {
                                    Width = RectangleSize,
                                    Height = RectangleSize,
                                    OpacityMask =
                                        new VisualBrush
                                            {
                                                Stretch = Stretch.Fill,
                                                Visual = (Canvas)Application.Current.Resources[icon]
                                            },
                                    Style = (Style)Application.Current.Resources["RectangleInsideToggleButtonStyle"]
                                };
        }
    }
}
