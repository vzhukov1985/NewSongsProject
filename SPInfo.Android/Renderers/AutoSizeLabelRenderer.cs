using System;
using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Widget;
using SPInfo.Droid.Renderers;
using SPInfo.UserControls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AutoSizeLabel), typeof(AutoSizeLabelRenderer))]
namespace SPInfo.Droid.Renderers
{
    public class AutoSizeLabelRenderer:LabelRenderer
    {
        public AutoSizeLabelRenderer(Context context):base(context)
        {

        }

        protected override bool ManageNativeControlLifetime => false;

        protected override void Dispose(bool disposing)
        {
            Control.RemoveFromParent();
            base.Dispose(disposing);
        }

        private AppCompatTextView appCompatTextView;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null || !(e.NewElement is AutoSizeLabel autoLabel) || Control == null) { return; }

            //v8 and above supported natively, no need for the extra stuff below.
            if (DeviceInfo.Version.Major >= 8)
            {
                Control?.SetAutoSizeTextTypeUniformWithConfiguration(5,
                    200, 1,
                    (int)ComplexUnitType.Sp);
                return;
            }

            appCompatTextView = new AppCompatTextView(Context);
            appCompatTextView.SetTextColor(Element.TextColor.ToAndroid());
            appCompatTextView.SetMaxLines(1);
            appCompatTextView.Gravity = GravityFlags.Center;
            appCompatTextView.SetBindingContext(autoLabel.BindingContext);
            appCompatTextView.SetBinding("Text", new Binding(autoLabel.Text));
            SetNativeControl(appCompatTextView);


            TextViewCompat.SetAutoSizeTextTypeUniformWithConfiguration(Control, 5, 200, 1, (int)ComplexUnitType.Sp);
        }
    }
}
