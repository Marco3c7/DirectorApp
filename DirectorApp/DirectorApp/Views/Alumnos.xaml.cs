﻿using DirectorApp.ViewModels;
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
    public partial class Alumnos : ContentPage
    {
        AlumnosViewModel vm;
        public Alumnos()
        {
            InitializeComponent();
            vm = this.BindingContext as AlumnosViewModel;
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                if (vm != null)
                {
                    vm.BuscarCommand.Execute(txtBuscar.Text);
                }
            }
            else
            {
                if (vm != null)
                {
                    vm.DescargarCommand.Execute(null);
                }
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (vm != null)
            {
                vm.DescargarCommand.Execute(null);
            }
        }
    }
}