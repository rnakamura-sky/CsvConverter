using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using Prism.Commands;
using Prism.Mvvm;

namespace CsvConverter.WPF.ViewModels
{
    public class CsvConvertViewModel : BindableBase
    {
        private ICsvConvertLogic _logic;

        public CsvConvertViewModel()
            : this(new CsvConvertLogic())
        {

        }

        public CsvConvertViewModel(ICsvConvertLogic logic)
        {
            _logic = logic;
            ExecuteCommand = new DelegateCommand(ExecuteCommandExecute, CanExecuteCommand);
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

        public DelegateCommand ExecuteCommand { get; }

        private void ExecuteCommandExecute()
        {
            var inputCsvFile = new InputCsvFileEntity(InputCsvFilePath);
            var outputCsvFile = new OutputCsvFileEntity(OutputCsvFilePath);

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
    }
}
