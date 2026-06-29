using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Dopamine.Behaviors
{
    public class MouseIdleBehavior : Behavior<FrameworkElement>
    {
        #region Dependency Properties

        public static readonly DependencyProperty HideAnimationProperty =
            DependencyProperty.Register(nameof(HideAnimation), typeof(Storyboard),
                typeof(MouseIdleBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty ShowAnimationProperty =
            DependencyProperty.Register(nameof(ShowAnimation), typeof(Storyboard),
                typeof(MouseIdleBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty TargetElementProperty =
            DependencyProperty.Register(nameof(TargetElement), typeof(FrameworkElement),
                typeof(MouseIdleBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty IdleThresholdMsProperty =
            DependencyProperty.Register(nameof(IdleThresholdMs), typeof(int),
                typeof(MouseIdleBehavior), new PropertyMetadata(3000));

        public Storyboard HideAnimation
        {
            get => (Storyboard)GetValue(HideAnimationProperty);
            set => SetValue(HideAnimationProperty, value);
        }

        public Storyboard ShowAnimation
        {
            get => (Storyboard)GetValue(ShowAnimationProperty);
            set => SetValue(ShowAnimationProperty, value);
        }

        public FrameworkElement TargetElement
        {
            get => (FrameworkElement)GetValue(TargetElementProperty);
            set => SetValue(TargetElementProperty, value);
        }

        public int IdleThresholdMs
        {
            get => (int)GetValue(IdleThresholdMsProperty);
            set => SetValue(IdleThresholdMsProperty, value);
        }

        #endregion

        #region Private Fields

        private Storyboard _hideAnimation;
        private Storyboard _showAnimation;
        private DispatcherTimer _timer;
        private DateTime _lastMouseMoveTime;
        private Point _lastMousePosition;
        private bool _isHideCompleted;

        #endregion

        #region Behavior Lifecycle

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeave -= OnMouseLeave;
            StopTimer();
        }

        #endregion

        #region Event Handlers

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (TargetElement == null || HideAnimation == null || ShowAnimation == null)
                return;

            _hideAnimation = HideAnimation.Clone();
            _showAnimation = ShowAnimation.Clone();

            Storyboard.SetTarget(_hideAnimation, TargetElement);
            Storyboard.SetTarget(_showAnimation, TargetElement);

            _hideAnimation.Completed += (es, ee) =>
            {
                TargetElement.Visibility = Visibility.Hidden;
                _isHideCompleted = true;
            };
            _showAnimation.Completed += (es, ee) =>
            {
                _isHideCompleted = false;
            };

            _lastMouseMoveTime = DateTime.UtcNow;

            _timer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(500),
                IsEnabled = true
            };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window == null) return;

            var position = e.GetPosition(window);
            if (position != _lastMousePosition)
                _lastMouseMoveTime = DateTime.UtcNow;

            _lastMousePosition = position;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _lastMouseMoveTime = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(10));
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (TargetElement == null) return;

            var elapsed = DateTime.UtcNow.Subtract(_lastMouseMoveTime);
            var isIdle = elapsed.TotalMilliseconds >= IdleThresholdMs;

            if (isIdle && TargetElement.IsMouseOver == false)
            {
                if (_isHideCompleted) return;
                AssociatedObject.Cursor = Cursors.None;
                _hideAnimation?.Begin();
                _isHideCompleted = false;
            }
            else
            {
                AssociatedObject.Cursor = Cursors.Arrow;
                TargetElement.Visibility = Visibility.Visible;
                _showAnimation?.Begin();
            }
        }

        #endregion

        #region Helpers

        private void StopTimer()
        {
            if (_timer == null) return;
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }

        #endregion
    }
}