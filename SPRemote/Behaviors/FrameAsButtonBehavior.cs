using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace SPRemote.Behaviors
{
    public class FrameAsButtonBehavior: Behavior<Frame>
    {
        private TapGestureRecognizer tap;

        public static readonly BindableProperty AttachBehaviorProperty =
            BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(FrameAsButtonBehavior), false, propertyChanged: OnAttachBehaviorChanged);

        public static bool GetAttachBehavior(BindableObject view)
        {
            return (bool)view.GetValue(AttachBehaviorProperty);
        }

        public static void SetAttachBehavior(BindableObject view, bool value)
        {
            view.SetValue(AttachBehaviorProperty, value);
        }

        static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            var frame = view as Frame;
            if (frame == null)
            {
                return;
            }

            bool attachBehavior = (bool)newValue;
            if (attachBehavior)
            {
                frame.Behaviors.Add(new FrameAsButtonBehavior());
            }
            else
            {
                var toRemove = frame.Behaviors.FirstOrDefault(b => b is FrameAsButtonBehavior);
                if (toRemove != null)
                {
                    frame.Behaviors.Remove(toRemove);
                }
            }
        }

        public FrameAsButtonBehavior()
        {
            tap = new TapGestureRecognizer();
            tap.Tapped += TapEffectAsync;
        }


        protected override void OnAttachedTo(Frame frame)
        {
            frame.GestureRecognizers.Add(tap);
            base.OnAttachedTo(frame);
        }

        protected override void OnDetachingFrom(Frame frame)
        {
            frame.GestureRecognizers.Remove(tap);
            base.OnDetachingFrom(frame);
        }

        private async void TapEffectAsync(Object sender, EventArgs e)
        {
            var frame = sender as Frame;
            if (frame.IsEnabled)
            {
                await frame.Content.FadeTo(0, 100);
                await frame.Content.FadeTo(1, 100);
            }
        }


    }
}
