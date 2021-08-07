using framecontroller.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace framecontroller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceView : ContentPage
    {
        public DeviceView()
        {
            InitializeComponent();
            this.BindingContext = new DeviceViewModel(this.Navigation); 
        }
    }
}