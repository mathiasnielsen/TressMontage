using System.Windows.Input;
using Xamarin.Forms;

namespace TressMontage.Client.Controls
{
    public class ClickableListView : ListView
    {
        public static BindableProperty ItemClickCommandProperty = 
            BindableProperty.Create(nameof(ItemClickCommand), typeof(ICommand), typeof(ClickableListView), null);

        public ClickableListView()
        {
            ////SeparatorVisibility = SeparatorVisibility.None;
            ItemTapped += OnItemTapped;
        }

        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null && ItemClickCommand != null && ItemClickCommand.CanExecute(e))
            {
                ItemClickCommand.Execute(e.Item);
                SelectedItem = null;
            }
        }
    }
}
