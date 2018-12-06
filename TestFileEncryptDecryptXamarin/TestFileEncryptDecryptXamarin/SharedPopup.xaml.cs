using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestFileEncryptDecryptXamarin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SharedPopup : ContentPage, INotifyPropertyChanged
    {
        public SharedPopup()
        {
            InitializeComponent();
            BindingContext = this;
            IsLoading = true;
            ToggleTemp = true;
        }

        public bool isLoading;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public bool _toggleTemp;

        public bool ToggleTemp
        {
            get
            {
                return this._toggleTemp;
            }

            set
            {
                this._toggleTemp = value;
                RaisePropertyChanged("ToggleTemp");
            }
        }
    }
}