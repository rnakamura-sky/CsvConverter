using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;
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
            TargetSettingTypes = new ObservableCollection<TargetSettingType>();
            foreach (var targetSettingType in TargetSettingType.GetTargetSettingTypes())
            {
                TargetSettingTypes.Add(targetSettingType);
            }

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

        private ObservableCollection<TargetSettingType> _targetSettingTypes;
        public ObservableCollection<TargetSettingType> TargetSettingTypes
        {
            get { return _targetSettingTypes; }
            set { SetProperty(ref _targetSettingTypes, value); }
        }

        private TargetSettingType _selectedTargetSettingType;
        public TargetSettingType SelectedTargetSettingType
        {
            get { return _selectedTargetSettingType; }
            set
            {
                if (SetProperty(ref _selectedTargetSettingType, value))
                {
                    if (value is null)
                    {
                        Target = null;
                    }
                    if (value == TargetSettingType.Input)
                    {
                        Target = new CreateOutputColumnViewModelTargetInput(InputHeaders);
                    }
                    if (value == TargetSettingType.Concatenate)
                    {
                        Target = new CreateOutputColumnViewModelTargetConcatenate(OutputHeaders);
                    }
                }
            }
        }

        private CreateOutputColumnViewModelTargetBase _target;

        public CreateOutputColumnViewModelTargetBase Target
        {
            get => _target;
            set => SetProperty(ref _target, value);
        }

        private void ExecuteCreateCommand()
        {
            var targetSetting = Target.GetTargetSettingEntity();
            var column = new OutputColumnSettingEntity(
                0, HeaderName, IsOutput, targetSetting);
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
            var inputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(InputHeaders));
            foreach (var header in inputHeaders)
            {
                InputHeaders.Add(header);
            }
            var outputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(OutputHeaders));
            foreach (var header in outputHeaders)
            {
                OutputHeaders.Add(header);
            }
        }

        private ObservableCollection<HeaderEntity> _inputHeaders = new();

        public ObservableCollection<HeaderEntity> InputHeaders
        {
            get { return _inputHeaders; }
            set { SetProperty(ref _inputHeaders, value); }
        }

        private ObservableCollection<HeaderEntity> _outputHeaders = new();

        public ObservableCollection<HeaderEntity> OutputHeaders
        {
            get { return _outputHeaders; }
            set { SetProperty(ref _outputHeaders, value); }
        }
    }
}
