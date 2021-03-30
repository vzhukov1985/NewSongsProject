using System;
using SPInfo.iOS.Renderers;
using SPInfo.UserControls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AutoSizeLabel), typeof(AutoSizeLabelRenderer))]
namespace SPInfo.iOS.Renderers
{
    public class AutoSizeLabelRenderer:LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control is UILabel label)
            {
                label.AdjustsFontSizeToFitWidth = true;
                label.Lines = 2;
                label.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
                label.LineBreakMode = UILineBreakMode.Clip;
            }
        }
    }
}
