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
    public partial class AgregarAlumno : ContentPage
    {
        public AgregarAlumno()
        {
            InitializeComponent();
        }

        private void txtContraseña_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                vm.HabilitarBotonCommand.Execute(false);
                txtContraseña2.Text = "";
            }
            else
            {
                vm.HabilitarBotonCommand.Execute(true);
            }
        }

        AlumnosViewModel vm;
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm = this.BindingContext as AlumnosViewModel;
        }
    }
}