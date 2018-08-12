using System;
using Android.Content;
using Android.Graphics.Drawables;
using FloraEjemplo.Customs;
using FloraEjemplo.Droid.Customs;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryAndroid))]
namespace FloraEjemplo.Droid.Customs
{
    public class CustomEntryAndroid : EntryRenderer
    {

        public CustomEntryAndroid(Context context)
            : base(context)
        {
        }

        public CustomEntryAndroid()
            : base(null)
        {
            // Default constructor needed for Xamarin Forms bug?
            throw new Exception("This constructor should not actually ever be used");
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                //Control.SetBackgroundResource(Resource.Layout.rounded_shape);
                var gradientDrawable = new GradientDrawable();
                //gradientDrawable.SetCornerRadius(15f);
                gradientDrawable.SetStroke(3, Android.Graphics.Color.Rgb(160, 160, 155));
                gradientDrawable.SetColor(Android.Graphics.Color.Rgb(244, 244, 242));
                Control.SetBackground(gradientDrawable);
                Control.SetPadding(10, Control.PaddingTop, Control.PaddingRight,
                    Control.PaddingBottom);
            }
        }
    }
}