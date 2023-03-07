using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Logics
{
    public interface ICsvConvertLogic
    {
        void Execute(InputCsvFileEntity inputCsvFile, OutputCsvFileEntity outputCsvFile);
    }
}
