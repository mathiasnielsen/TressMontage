using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TressMontage.Client.Core.Common
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var parm = (T)parameter;
            execute.Invoke(parm);
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                var parm = (T)parameter;
                return canExecute.Invoke(parm);
            }
            else
            {
                return true;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
