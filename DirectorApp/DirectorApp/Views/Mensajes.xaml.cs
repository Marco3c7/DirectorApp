using DirectorApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DirectorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Mensajes : ContentPage
    {
        MensajesViewModel vm;
        public Mensajes()
        {
            InitializeComponent();
            vm = this.BindingContext as MensajesViewModel;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.DescargarCommand.Execute(null);
        }

        private void dtpFecha_DateSelected(object sender, DateChangedEventArgs e)
        {
            vm.FiltrarCommand.Execute(dtpFecha.Date);
        }
    }
}