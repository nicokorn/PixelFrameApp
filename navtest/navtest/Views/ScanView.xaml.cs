﻿using navtest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace navtest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanView : ContentPage
    {
        public ScanView()
        {
            InitializeComponent();
            this.BindingContext = new ScanViewModel(this.Navigation);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}