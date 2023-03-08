using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CsvConverter.WPF.ViewModels
{
    public class CsvConvertViewModel : BindableBase
    {
        private ICsvConvertLogic _logic;
        private ICsvFileRepository _csvFileRepository;

        public CsvConvertViewModel()
            : this(new CsvConvertLogic(), new CsvFileAccess())
        {

        }

        public CsvConvertViewModel(ICsvConvertLogic logic, ICsvFileRepository csvFileRepository)
        {
            _logic = logic;
            _csvFileRepository = csvFileRepository;

            ExecuteCommand = new DelegateCommand(ExecuteCommandExecute, CanExecuteCommand);
            InputCommand = new DelegateCommand(ExecuteInputCommand, CanInputCommand);
            ReplaceOutputRowCommand = new DelegateCommand<int?>(ExecuteReplaceOutputRowCommand);
        }

        private string _inputCsvFilePath = string.Empty;
        public string InputCsvFilePath
        {
            get { return _inputCsvFilePath; }
            set
            {
                if (SetProperty(ref _inputCsvFilePath, value))
                {
                    ExecuteCommand.RaiseCanExecuteChanged();
                    InputCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _outputCsvFilePath = string.Empty;
        public string OutputCsvFilePath
        {
            get { return _outputCsvFilePath; }
            set
            {
                if (SetProperty(ref _outputCsvFilePath, value))
                {
                    ExecuteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private ObservableCollection<CsvConvertViewModelHeader> _outputRows = new();

        
        public ObservableCollection<CsvConvertViewModelHeader> OutputRows {
            get => _outputRows;
            private set => SetProperty(ref _outputRows, value);
        }

        private int _selectedOutputRowIndex;
        public int SelectedOutputRowIndex
        {
            get { return _selectedOutputRowIndex; }
            set { SetProperty(ref _selectedOutputRowIndex, value); }
        }

        public DelegateCommand ExecuteCommand { get; }

        public DelegateCommand InputCommand { get; }

        public DelegateCommand<int?> ReplaceOutputRowCommand { get; }

        private void ExecuteCommandExecute()
        {
            var inputCsvFile = new InputCsvFileEntity(_csvFileRepository, InputCsvFilePath);
            var outputCsvFile = new OutputCsvFileEntity(_csvFileRepository, OutputCsvFilePath);

            var outputSetting = GetOutputSetting();
            _logic.Execute(inputCsvFile, outputCsvFile, outputSetting);
        }

        private bool CanExecuteCommand()
        {
            if (InputCsvFilePath == string.Empty)
            {
                return false;
            }
            if (OutputCsvFilePath == string.Empty)
            {
                return false;
            }
            return true;
        }


        private void ExecuteInputCommand()
        {
            var inputCsvFile = new InputCsvFileEntity(_csvFileRepository, InputCsvFilePath);
            var fileData = inputCsvFile.GetData();

            OutputRows.Clear();
            foreach (var header in fileData.Headers)
            {
                OutputRows.Add(new CsvConvertViewModelHeader(header));
            }
        }

        private bool CanInputCommand()
        {
            if (InputCsvFilePath == string.Empty)
            {
                return false;
            }
            return true;
        }

        private void ExecuteReplaceOutputRowCommand(int? index)
        {
            if (index is null)
            {
                return;
            }
            OutputRows.Move(SelectedOutputRowIndex, index.Value);
        }

        private OutputSettingEntity GetOutputSetting()
        {
            if (OutputRows.Count == 0)
            {
                return OutputSettingEntity.None;
            }
            var settingRows = new List<OutputRowSettingEntity>();
            var index = 0;
            foreach (var row in OutputRows)
            {
                settingRows.Add(row.GetRowSettingEntity(index));
                index++;
            }

            return new OutputSettingEntity(settingRows);
        }
    }
}
