namespace CsvConverter.Domain.Repositories
{
    public interface IOutputCsvFileRepository
    {
        void WriteData(string data);
    }
}
