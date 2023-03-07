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
            ExecuteCommand = new DelegateCommand(ExecuteCommandExecute);
        }

        private string _inputCsvFilePath = string.Empty;
        public string InputCsvFilePath
        {
            get { return _inputCsvFilePath; }
            set { SetProperty(ref _inputCsvFilePath, value); }
        }

        private string _outputCsvFilePath = string.Empty;
        public string OutputCsvFilePath
        {
            get { return _outputCsvFilePath; }
            set { SetProperty(ref _outputCsvFilePath, value); }
        }

        public DelegateCommand ExecuteCommand { get; }

        private void ExecuteCommandExecute()
        {
            var inputCsvFile = new InputCsvFileEntity(InputCsvFilePath);
            var outputCsvFile = new OutputCsvFileEntity(OutputCsvFilePath);

            _logic.Execute(inputCsvFile, outputCsvFile);

        }
    }
}
