using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TressMontage.Client.Features.Common
{
    public partial class MasterNavigationPage : NavigationPage
    {
        public MasterNavigationPage(ContentPage content)
            : base(content)
        {
            InitializeComponent();
        }
    }
}
