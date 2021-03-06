﻿using MajaMobile.Interfaces;
using MajaMobile.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace MajaMobile.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public const string OpenbirequestErrorMessage = "REQUEST_ERROR";
        public const string GoBackMessage = "GO_BACK";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SessionHandler SessionHandler { get; }

        public bool IsIdle
        {
            get => !IsBusy;
            set
            {
                IsBusy = !value;
            }
        }

        public bool IsBusy
        {
            get => GetField<bool>();
            set
            {
                SetField(value);
                OnPropertyChanged(nameof(IsIdle));
            }
        }

        public double StatusBarHeight
        {
            get
            {
                return DependencyService.Get<IDeviceInfo>().StatusBarHeight;
            }
        }

        public double NavigationBarHeight
        {
            get
            {
                return DependencyService.Get<IDeviceInfo>().NavigationBarHeight;
            }
        }

        public ICommand GoBackCommand { get; }
        public ViewModelBase()
        {
            GoBackCommand = new Command(GoBack);
        }

        public ViewModelBase(SessionHandler sessionHandler) : this()
        {
            SessionHandler = sessionHandler;
        }

        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        protected T GetField<T>([CallerMemberName] string caller = null)
        {
            object value = null;
            if (_fields.TryGetValue(caller, out value))
            {
                return (T)value;
            }
            return default(T);
        }

        protected void SetField(object value, [CallerMemberName] string caller = "", bool raiseChanged = true)
        {
            _fields[caller] = value;
            if (raiseChanged)
                OnPropertyChanged(caller);
        }

        protected void GoBack()
        {
            MessagingCenter.Send(this, GoBackMessage);
        }

        protected void DisplayException(Exception ex)
        {
            MessagingCenter.Send(this, OpenbirequestErrorMessage, ex);
        }

        public virtual void Dispose()
        {

        }

        protected bool IsActive;

        public virtual void SendAppearing()
        {
            IsActive = true;
        }

        public virtual void SendDisappearing()
        {
            IsActive = false;
        }

    }
}