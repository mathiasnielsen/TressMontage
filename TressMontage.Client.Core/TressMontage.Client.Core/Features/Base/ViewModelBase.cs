using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Features.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLoading
        {
            get { return isLoading; }
            set { Set(ref isLoading, value); }
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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title { get; set; }

        public async void ViewInitialized(Dictionary<string, string> parameters)
        {
            await OnViewInitialized(parameters);
        }

        public async void ViewReloading()
        {
            await OnViewReloaded();
        }

        public virtual async Task OnViewInitialized(Dictionary<string, string> parameters)
        {
            await Task.FromResult(true);
        }

        public virtual async Task OnViewReloaded()
        {
            await Task.FromResult(true);
        }
    }
}
