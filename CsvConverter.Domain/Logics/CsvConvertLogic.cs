using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Logics
{
    public class CsvConvertLogic : ICsvConvertLogic
    {
        public void Execute(InputCsvFileEntity inputCsvFile, OutputCsvFileEntity outputCsvFile)
        {
            var data = inputCsvFile.GetData();
            outputCsvFile.WriteData(data);
        }
    }
}
