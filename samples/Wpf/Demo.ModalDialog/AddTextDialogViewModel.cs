using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.ModalDialog
{
    public class AddTextDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
    {
        private string? text;
        private bool? dialogResult;

        public AddTextDialogViewModel()
        {
            OkCommand = new RelayCommand(Ok);
        }

        public string? Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public ICommand OkCommand { get; }

        public bool? DialogResult
        {
            get => dialogResult;
            private set => SetProperty(ref dialogResult, value);
        }

        public event EventHandler? RequestClose;

        private void Ok()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                DialogResult = true;
                this.RequestClose?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
