﻿using BiExcellence.OpenBi.Api.Commands.Users;
using MajaMobile.Models;
using MajaMobile.Utilities;
using MajaMobile.ViewModels;
using Plugin.Media;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace MajaMobile.Pages
{
    public partial class UserProfilePage : CancelBackContentPage
    {
        public UserProfilePage(IUser user, SessionHandler sessionHandler)
        {
            InitializeComponent();
            BindingContext = ViewModel = new UserProfileViewModel(user, sessionHandler);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Unsubscribe<ImageEditorPage, byte[]>(this, ImageEditorPage.ImageSavedMessage);
        }

        private bool _discarded;

        protected override bool OnBackButtonPressed()
        {
            if (!_discarded && ((UserProfileViewModel)ViewModel).DataChanged())
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await DisplayAlert("Profil bearbeiten", "Änderungen verwerfen?", "VERWERFEN", "ABBRECHEN"))
                    {
                        _discarded = true;
                        await Navigation.PopAsync();
                    }
                });
                return true;
            }
            return false;
        }

        public override bool OnBackPressed()
        {
            return OnBackButtonPressed();
        }

        private void ImageCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            var radius = e.Info.Height / 2f;
            var centerX = e.Info.Width / 2f;
            var centerY = radius;
            var halfVer = 0.3f * radius;
            centerY += halfVer * 0.25f;
            using (var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = SKColors.White, StrokeWidth = radius * 0.05f })
            {
                var top = centerY - halfVer;
                var halfHor = halfVer * 1.32f;
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(centerX - halfHor, top, centerX + halfHor, centerY + halfVer), 10, 10), paint);
                paint.Style = SKPaintStyle.Fill;
                halfHor = halfHor * 0.5f;
                var innerCenterY = centerY - 0.2f * halfHor;
                canvas.DrawCircle(centerX, innerCenterY, halfHor, paint);
                using (var eraserPaint = new SKPaint() { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = SKColors.Transparent, StrokeWidth = paint.StrokeWidth * 1.05f, BlendMode = SKBlendMode.Src, StrokeCap = SKStrokeCap.Round })
                using (var eraserPath = new SKPath())
                {
                    var h2 = 0.4f * halfHor;
                    eraserPath.MoveTo(centerX, innerCenterY - h2);
                    eraserPath.LineTo(centerX, innerCenterY + h2);
                    eraserPath.MoveTo(centerX - h2, innerCenterY);
                    eraserPath.LineTo(centerX + h2, innerCenterY);
                    canvas.DrawPath(eraserPath, eraserPaint);
                    eraserPath.Reset();
                    eraserPaint.StrokeCap = SKStrokeCap.Butt;
                    eraserPath.MoveTo(centerX - halfHor, top);
                    eraserPath.LineTo(centerX + halfHor, top);
                    canvas.DrawPath(eraserPath, eraserPaint);
                }
                paint.Style = SKPaintStyle.Stroke;
                using (var path = new SKPath())
                {
                    top = centerY - halfVer * 1.5f;
                    var left = centerX - halfHor;
                    var right = centerX + halfHor;
                    path.AddArc(new SKRect(left, top, right, top + right - left), 187.5f, 165);
                    canvas.DrawPath(path, paint);
                }
            }
        }

        private void OverlayCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            using (var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = ColorScheme.ImageOverlayColor.ToSKColor(), StrokeWidth = 0 })
            {
                canvas.DrawCircle(e.Info.Width / 2f, e.Info.Height / 2f, e.Info.Height * (5f / 12f), paint);
            }
        }

        //private const string takePhoto = "Foto aufnehmen";
        //private const string choosePhoto = "Vorhandenes Foto auswählen";
        //private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    if (ViewModel.IsBusy)
        //        return;
        //    var action = await DisplayActionSheet("Profilbild", "Abbrechen", null, takePhoto, choosePhoto);
        //    switch (action)
        //    {
        //        case takePhoto:
        //            if (!CrossMedia.Current.IsTakePhotoSupported)
        //            {
        //                await DisplayAlert("Bild aufnehmen", "Die Funktion wird von Ihrem Gerät derzeit nicht unterstützt", "OK");
        //                return;
        //            }
        //            break;
        //        case choosePhoto:
        //            if (!CrossMedia.Current.IsPickPhotoSupported)
        //            {
        //                await DisplayAlert("Bild auswählen", "Die Funktion wird von Ihrem Gerät derzeit nicht unterstützt", "OK");
        //                return;
        //            }
        //            break;
        //        default:
        //            return;
        //    }
        //    try
        //    {
        //        ViewModel.IsBusy = true;
        //        using (var imageFile = action == choosePhoto ? await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium })
        //                                                     : await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium, SaveToAlbum = false, AllowCropping = false }))
        //        {
        //            if (imageFile != null)
        //            {
        //                using (var stream = imageFile.GetStreamWithImageRotatedForExternalStorage())
        //                using (var ms = new MemoryStream())
        //                {
        //                    await stream.CopyToAsync(ms);
        //                    await Navigation.PushAsync(new ImageEditorPage(ms.ToArray()));
        //                    MessagingCenter.Subscribe<ImageEditorPage, byte[]>(this, ImageEditorPage.ImageSavedMessage, ImageEditor_Saved);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Cannot access camera. Error: ", ex.Message);
        //    }
        //    finally
        //    {
        //        ViewModel.IsBusy = false;
        //    }
        //}

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (ViewModel.IsBusy)
                return;
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Bild auswählen", "Die Funktion wird von Ihrem Gerät derzeit nicht unterstützt", "OK");
                return;
            }
            try
            {
                ViewModel.IsBusy = true;
                var imageFile = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium });
                if (imageFile != null)
                {
                    using (var stream = imageFile.GetStreamWithImageRotatedForExternalStorage())
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        await Navigation.PushAsync(new ImageEditorPage(ms.ToArray()));
                        MessagingCenter.Subscribe<ImageEditorPage, byte[]>(this, ImageEditorPage.ImageSavedMessage, ImageEditor_Saved);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot access camera. Error: ", ex.Message);
            }
            finally
            {
                ViewModel.IsBusy = false;
            }
        }

        private void ImageEditor_Saved(ImageEditorPage page, byte[] arr)
        {
            ((UserProfileViewModel)ViewModel).User.SetPicture(arr);
        }
    }
}

namespace MajaMobile.ViewModels
{
    public class UserProfileViewModel : ViewModelBase
    {
        public MajaUser User { get; }
        private IUser _originalUser;
        public ICommand SaveCommand { get; }
        private bool _saved;

        public UserProfileViewModel(IUser user, SessionHandler sessionHandler) : base(sessionHandler)
        {
            _originalUser = user;
            User = new MajaUser(user);
            SaveCommand = new Command(Save);
        }

        public bool DataChanged()
        {
            return !_saved && !User.Equals(_originalUser);
        }

        private async void Save()
        {
            if (IsBusy)
                return;
            User.Firstname = User.Firstname.Trim();
            User.Lastname = User.Lastname.Trim();
            if (!DataChanged())
                GoBack();
            IsBusy = true;
            try
            {
                await SessionHandler.ExecuteOpenbiCommand((s, t) => s.CreateUser(User));
                _saved = true;
                SessionHandler.OpenBiUser = User;
                GoBack();
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

namespace MajaMobile.Models
{
    public class MajaUser : User, IEquatable<IUser>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MajaUser(IUser user) : base(user.Username)
        {
            foreach (var prop in typeof(IUser).GetTypeInfo().DeclaredProperties)
            {
                if (prop.CanWrite)
                    prop.SetValue(this, prop.GetValue(user));
            }
        }

        public void SetPicture(byte[] picture)
        {
            Picture = picture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Picture)));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            if (obj is IUser user)
                return Equals(user);
            return false;
        }

        public bool Equals(IUser other)
        {
            if (other == null) return false;
            return Firstname == other.Firstname && Lastname == other.Lastname && Birthdate == other.Birthdate && Picture == other.Picture;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = (hashCode * 397) ^ Username.GetHashCode();
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(Firstname) ? Firstname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(Lastname) ? Lastname.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}