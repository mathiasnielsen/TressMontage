using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TressMontage.Client.Core.Features.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool isLoading;
        private float loadingProgress;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLoading
        {
            get { return isLoading; }
            set { Set(ref isLoading, value); }
        }

        public float LoadingProgress
        {
            get { return loadingProgress; }
            set { Set(ref loadingProgress, value); }
        }

        protected void ResetLoadingProgress()
        {
            LoadingProgress = 0;
        }

        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected Dictionary<string, string> NavigationParameters { get; set; }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
