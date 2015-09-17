using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PullToRefreshListView
{
    public sealed class PullToRefreshListView : ListView
    {
        #region Constructor
        public PullToRefreshListView()
        {
            this.DefaultStyleKey = typeof(PullToRefreshListView);
            Loaded += PullToRefreshScrollViewer_Loaded;
        }
        #endregion

        #region Event
        public event EventHandler RefreshContent;
        public event EventHandler MoreContent;
        #endregion

        #region DependencyProperty
        public static readonly DependencyProperty PullPartTemplateProperty =
            DependencyProperty.Register("PullPartTemplate", typeof(string), typeof(PullToRefreshListView), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ReleasePartTemplateProperty =
            DependencyProperty.Register("ReleasePartTemplate", typeof(string), typeof(PullToRefreshListView), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty RefreshHeaderHeightProperty =
            DependencyProperty.Register("RefreshHeaderHeight", typeof(double), typeof(PullToRefreshListView), new PropertyMetadata(30D));
        private static readonly DependencyProperty arrowColorProperty =
            DependencyProperty.Register("ArrowColor", typeof(Brush), typeof(PullToRefreshListView), new PropertyMetadata(new SolidColorBrush(Colors.Red)));
        #endregion

        #region Property
        public string PullPartTemplate
        {
            get
            {
                return (string)base.GetValue(PullToRefreshListView.PullPartTemplateProperty);
            }
            set
            {
                base.SetValue(PullToRefreshListView.PullPartTemplateProperty, value);
            }
        }

        public string ReleasePartTemplate
        {
            get
            {
                return (string)base.GetValue(PullToRefreshListView.ReleasePartTemplateProperty);
            }
            set
            {
                base.SetValue(PullToRefreshListView.ReleasePartTemplateProperty, value);
            }
        }

        public double RefreshHeaderHeight
        {
            get
            {
                return (double)base.GetValue(PullToRefreshListView.RefreshHeaderHeightProperty);
            }
            set
            {
                base.SetValue(PullToRefreshListView.RefreshHeaderHeightProperty, value);
            }
        }

        public Brush ArrowColor
        {
            get
            {
                return (Brush)base.GetValue(PullToRefreshListView.ArrowColorProperty);
            }
            set
            {
                base.SetValue(PullToRefreshListView.ArrowColorProperty, value);
            }
        }

        public static DependencyProperty ArrowColorProperty
        {
            get
            {
                return arrowColorProperty;
            }
        }
        #endregion

        #region Field
        private double OffsetTreshhold = 40;
        private DispatcherTimer CompressionTimer;
        private ScrollViewer RootScrollViewer;
        private DispatcherTimer Timer;
        private Grid ContainerGrid;
        private Border PullToRefreshIndicator;
        private bool IsCompressionTimerRunning;
        private bool IsReadyToRefresh;
        private bool IsCompressedEnough;
        #endregion

        #region Override Method
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RootScrollViewer = base.GetTemplateChild("ScrollViewer") as ScrollViewer;
            RootScrollViewer.ViewChanging += ScrollViewer_ViewChanging;
            RootScrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            RootScrollViewer.Margin = new Thickness(0, 0, 0, -RefreshHeaderHeight);
            RootScrollViewer.RenderTransform = new CompositeTransform() { TranslateY = -RefreshHeaderHeight };

            ContainerGrid = base.GetTemplateChild("ContainerGrid") as Grid;

            PullToRefreshIndicator = GetTemplateChild("PullToRefreshIndicator") as Border;
            SizeChanged += OnSizeChanged;
        }
        #endregion

        #region Routed Event
        private void PullToRefreshScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += Timer_Tick;

            CompressionTimer = new DispatcherTimer();
            CompressionTimer.Interval = TimeSpan.FromSeconds(1);
            CompressionTimer.Tick += CompressionTimer_Tick;

            Timer.Start();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (e.NextView.VerticalOffset == 0)
            {
                Timer.Start();
            }
            else
            {
                if (Timer != null)
                {
                    Timer.Stop();
                }

                if (CompressionTimer != null)
                {
                    CompressionTimer.Stop();
                }

                IsCompressionTimerRunning = false;
                IsCompressedEnough = false;
                IsReadyToRefresh = false;

                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            if (ContainerGrid != null)
            {
                Rect elementBounds = PullToRefreshIndicator.TransformToVisual(ContainerGrid).TransformBounds(new Rect(0.0, 0.0, PullToRefreshIndicator.Height, RefreshHeaderHeight));
                var compressionOffset = elementBounds.Bottom;

                if (compressionOffset > OffsetTreshhold)
                {
                    if (IsCompressionTimerRunning == false)
                    {
                        IsCompressionTimerRunning = true;
                        CompressionTimer.Start();
                    }

                    IsCompressedEnough = true;
                }
                else if (compressionOffset == 0 && IsReadyToRefresh == true)
                {
                    InvokeRefresh();
                }
                else
                {
                    IsCompressedEnough = false;
                    IsCompressionTimerRunning = false;
                }
            }
        }

        private void CompressionTimer_Tick(object sender, object e)
        {
            if (IsCompressedEnough)
            {
                VisualStateManager.GoToState(this, "ReadyToRefresh", true);
                IsReadyToRefresh = true;
            }
            else
            {
                IsCompressedEnough = false;
                CompressionTimer.Stop();
            }
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (RootScrollViewer.VerticalOffset == RootScrollViewer.ScrollableHeight)
            {
                InvokeMore();
            }
        }
        #endregion

        #region Misc
        private void InvokeRefresh()
        {
            IsReadyToRefresh = false;
            VisualStateManager.GoToState(this, "Normal", true);

            if (RefreshContent != null)
            {
                RefreshContent(this, EventArgs.Empty);
            }
        }

        private void InvokeMore()
        {
            if (MoreContent != null)
            {
                MoreContent(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
