﻿using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MajaMobile.Controls.DroidEntry), typeof(MajaMobile.Droid.Renderers.DroidEntryRenderer))]
namespace MajaMobile.Droid.Renderers
{
    public class DroidEntryRenderer : EntryRenderer
    {
        public DroidEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                //https://github.com/facebook/react-native/issues/6096
                var hor = (Control.PaddingLeft + Control.PaddingRight) / 2;
                var ver = (Control.PaddingTop + Control.PaddingBottom) / 2;
                Control.SetPadding(hor, ver, hor, ver);

                Control.SetBackgroundColor(Utilities.ColorScheme.EntryBackgroundColor.ToAndroid());
                //Control.SetSingleLine(true);
            }
        }
    }
}