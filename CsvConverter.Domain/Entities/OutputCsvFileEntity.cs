using CsvConverter.Domain.Repositories;

namespace CsvConverter.Domain.Entities
{
    public sealed class OutputCsvFileEntity
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

        public void WriteData(FileDataEntity data)
        {
            var fileString = data.GetFileString();
            _outputCsvFileRepository.WriteData(CsvFilePath, fileString);
        }
    }
}
