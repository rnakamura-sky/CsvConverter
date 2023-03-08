using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using Prism.Commands;
using Prism.Mvvm;
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

        public DelegateCommand ExecuteCommand { get; }

        public DelegateCommand InputCommand { get; }

        private void ExecuteCommandExecute()
        {
            var inputCsvFile = new InputCsvFileEntity(_csvFileRepository, InputCsvFilePath);
            var outputCsvFile = new OutputCsvFileEntity(_csvFileRepository, OutputCsvFilePath);

            _logic.Execute(inputCsvFile, outputCsvFile);

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
    }
}
