using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace  StockFocus
{
    public class AnimationHelper
    {
        public static void RotateAnimate(double angle, UIElement target, double time, bool autoReverse = true)
        {
            target.RenderTransform = new RotateTransform() { CenterX = 10, CenterY = 10 };
            Duration d = new Duration(TimeSpan.FromSeconds(time));
            DoubleAnimation x = new DoubleAnimation(angle, d);

            x.AutoReverse = autoReverse;
            Storyboard.SetTarget(x, target);

            Storyboard.SetTargetProperty(x,
                        new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));



            Storyboard sb = new Storyboard();
            sb.Children.Add(x);

            sb.Begin();

        }

        public static void SlideAnimate(UIElement target, double time, double translateX, double translateY, bool autoReverse = true)
        {
            target.RenderTransform = new TranslateTransform();
            Duration d = new Duration(TimeSpan.FromSeconds(time));
            DoubleAnimation x = new DoubleAnimation(translateX, d);
            DoubleAnimation y = new DoubleAnimation(translateY, d);
            y.AutoReverse = x.AutoReverse = autoReverse;

            Storyboard.SetTarget(x, target);
            Storyboard.SetTarget(y, target);
            Storyboard.SetTargetProperty(x, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTargetProperty(y, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            Storyboard sb = new Storyboard();
            sb.Children.Add(x);
            sb.Children.Add(y);
            sb.Begin();

        }

        public static void SlideAnimate(UIElement target, double time, double translateX, double translateY,IEasingFunction ease,  bool autoReverse = true)
        {
            target.RenderTransform = new TranslateTransform();
            Duration d = new Duration(TimeSpan.FromSeconds(time));
            DoubleAnimation x = new DoubleAnimation(translateX, d);
            DoubleAnimation y = new DoubleAnimation(translateY, d);
            y.AutoReverse = x.AutoReverse = autoReverse;
            x.EasingFunction = y.EasingFunction = ease;

            Storyboard.SetTarget(x, target);
            Storyboard.SetTarget(y, target);
            Storyboard.SetTargetProperty(x, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTargetProperty(y, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            Storyboard sb = new Storyboard();
            sb.Children.Add(x);
            sb.Children.Add(y);
            sb.Begin();

        }

        public static void OpacitySlideAnimate(UIElement target, double time, double translateX, double translateY, double opacity, IEasingFunction ease, bool autoReverse = true)
        {
            target.RenderTransform = new TranslateTransform();
            Duration d = new Duration(TimeSpan.FromSeconds(time));
            DoubleAnimation x = new DoubleAnimation(translateX, d);
            DoubleAnimation y = new DoubleAnimation(translateY, d);
            DoubleAnimation op = new DoubleAnimation(opacity, d);

            op.EasingFunction = y.EasingFunction = x.EasingFunction = ease;
            op.AutoReverse = y.AutoReverse = x.AutoReverse = autoReverse;

            Storyboard.SetTarget(x, target);
            Storyboard.SetTarget(y, target);
            Storyboard.SetTarget(op, target);
            Storyboard.SetTargetProperty(x, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTargetProperty(y, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            Storyboard.SetTargetProperty(op, new PropertyPath("Opacity"));
            Storyboard sb = new Storyboard();
            sb.Children.Add(x);
            sb.Children.Add(y);
            sb.Children.Add(op);
            sb.Begin();

        }

        public static void OpacityAnimate(UIElement target, double time, double opacity, IEasingFunction ease, bool autoReverse = false)
        {
            target.RenderTransform = new TranslateTransform();
            Duration d = new Duration(TimeSpan.FromSeconds(time));

            DoubleAnimation op = new DoubleAnimation(opacity, d);

            op.EasingFunction = ease;
            op.AutoReverse = autoReverse;
            op.To = opacity;
            op.From = opacity < 1 ? 100 : 0;
              
            Storyboard.SetTarget(op, target);

            Storyboard.SetTargetProperty(op, new PropertyPath("Opacity"));
            Storyboard sb = new Storyboard();

            sb.Children.Add(op);
            sb.Begin();

        }



    }
}
