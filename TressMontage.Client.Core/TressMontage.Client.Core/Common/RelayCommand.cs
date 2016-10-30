using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Common
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute)
            : this(execute, o => true) { }

        public RelayCommand(Action<object> execute)
            : base(execute) { }

        public RelayCommand(Action execute, Predicate<object> canExecute)
            : base(o => execute(), o => canExecute(o)) { }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : base(o => execute(), o => canExecute()) { }
    }
}
