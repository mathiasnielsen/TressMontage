using TressMontage.Client.Core.Features.Calendar;
using TressMontage.Client.Features.Base;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Calendar
{
    public abstract class CalendarViewBase : BindableViewBase<CalendarViewModel>
    {
    }

    public partial class CalendarView : CalendarViewBase
    {
        public CalendarView()
        {
            InitializeComponent();

            CalendarWebView.Source = new UrlWebViewSource
            {
                Url = "https://mail.tress.dk/owa/calendar/montage@tress.dk/Kalender/calendar.html",
            };
        }
    }
}
