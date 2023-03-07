using CsvConverter.Domain.Repositories;

namespace CsvConverter.Domain.Entities
{
    public class OutputCsvFileEntity
    {
        private IOutputCsvFileRepository _outputCsvFileRepository;

        public string CsvFilePath { get; }

        public OutputCsvFileEntity(string outputCsvFilePath)
            : this(new CsvFileAccess(), outputCsvFilePath)
        {
        }

        public OutputCsvFileEntity(IOutputCsvFileRepository outputCsvFileRepository, string outputCsvFilePath)
        {
            _outputCsvFileRepository = outputCsvFileRepository;
            CsvFilePath = outputCsvFilePath;
        }


        public void WriteData(string data)
        {
            _outputCsvFileRepository.WriteData(CsvFilePath, data);
        }
    }
}
