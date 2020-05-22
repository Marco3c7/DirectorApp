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
    public partial class Cuenta : ContentPage
    {
        CuentaViewModel vm;
        public Cuenta()
        {
            InitializeComponent();
            vm = this.BindingContext as CuentaViewModel;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (vm != null)
            {
                vm.ActualizarVistaCommand.Execute(null);
            }
        }
    }
}