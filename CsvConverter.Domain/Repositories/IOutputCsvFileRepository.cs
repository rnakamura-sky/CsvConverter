namespace CsvConverter.Domain.Repositories
{
    public interface IOutputCsvFileRepository
    {
        void WriteData(string filePath, string data);
    }
}
