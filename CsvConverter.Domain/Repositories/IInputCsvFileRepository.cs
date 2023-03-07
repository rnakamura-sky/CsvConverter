using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Repositories
{
    public interface IInputCsvFileRepository
    {
        string GetData(string filePath);

    }
}
