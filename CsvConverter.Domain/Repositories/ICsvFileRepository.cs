namespace CsvConverter.Domain.Repositories
{
    public interface ICsvFileRepository
    {
        string GetData();

        void WriteData(string data);
    }
}
