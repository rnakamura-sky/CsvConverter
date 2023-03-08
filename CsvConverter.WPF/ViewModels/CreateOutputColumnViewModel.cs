using CsvConverter.Domain.Entities;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CsvConverter.WPF.ViewModels
{
    public class CreateOutputColumnViewModel : BindableBase, IDialogAware
    {
        public CreateOutputColumnViewModel()
        {
            CreateCommand = new DelegateCommand(ExecuteCreateCommand);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
        }

        public string Title => "新規項目作成";

        public DelegateCommand CreateCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private string _headerName = string.Empty;
        public string HeaderName
        {
            get { return _headerName; }
            set { SetProperty(ref _headerName, value); }
        }

        private bool _isOutput = true;
        public bool IsOutput
        {
            get { return _isOutput; }
            set { SetProperty(ref _isOutput, value); }
        }

        private HeaderEntity _selectedInputHeader;
        public HeaderEntity SelectedInputHeader
        {
            get { return _selectedInputHeader; }
            set { SetProperty(ref _selectedInputHeader, value); }
        }

        private bool _isInputHeader = false;
        public bool IsInputHeader
        {
            get { return _isInputHeader; }
            set { SetProperty(ref _isInputHeader, value); }
        }

        private void ExecuteCreateCommand()
        {
            var column = new OutputColumnSettingEntity(
                0, HeaderName, IsInputHeader, SelectedInputHeader.Header, IsOutput);
            var parameters = new DialogParameters()
            {
                {nameof(OutputColumnSettingEntity), column},
            };
            var result = new DialogResult(ButtonResult.OK, parameters);

            RequestClose?.Invoke(result);
        }

        private void ExecuteCancelCommand()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var headers = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(Headers));
            foreach (var header in headers)
            {
                Headers.Add(header);
            }
        }

        private ObservableCollection<HeaderEntity> _headers = new();

        public ObservableCollection<HeaderEntity> Headers
        {
            get { return _headers; }
            set { SetProperty(ref _headers, value); }
        }

    }
}
