﻿using BiExcellence.OpenBi.Api;
using MajaMobile.ViewModels;
using System;
using Xamarin.Forms;

namespace MajaMobile.Pages
{
    public class ContentPageBase : ContentPage, IDisposable
    {

        public ViewModelBase ViewModel { get; protected set; }

        public ContentPageBase()
        {
            var style = (Style)Application.Current.Resources["ContentPageStyle"];
            Style = style;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel != null)
            {
                MessagingCenter.Subscribe<ViewModelBase, Exception>(this, ViewModelBase.OpenbirequestErrorMessage, RequestOnError);
                MessagingCenter.Subscribe(this, ViewModelBase.GoBackMessage, async (ViewModelBase vm) =>
                {
                    if (vm == ViewModel)
                    {
                        await Navigation.PopAsync();
                    }
                });
                ViewModel.SendAppearing();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (ViewModel != null)
            {
                MessagingCenter.Unsubscribe<ViewModelBase, Exception>(this, ViewModelBase.OpenbirequestErrorMessage);
                MessagingCenter.Unsubscribe<ViewModelBase>(this, ViewModelBase.GoBackMessage);
                ViewModel.SendDisappearing();
            }
        }

        public async void RequestOnError(ViewModelBase viewmodel, Exception ex)
        {
            if (viewmodel == ViewModel)
            {
                var message = ex.Message;
                if (ex is OpenBiServerErrorException openBiServerError)
                {
                    if (openBiServerError.Response.Code == OpenBiResponseCodes.LoginFailed)
                    {
                        message = "Benutzername oder Passwort falsch";
                    }
                }
                await DisplayAlert("Fehler", message, "OK");
            }
        }

        public virtual void Dispose()
        {
            ViewModel.Dispose();
        }
    }

    public abstract class CancelBackContentPage : ContentPageBase
    {
        public abstract bool OnBackPressed();
    }
}