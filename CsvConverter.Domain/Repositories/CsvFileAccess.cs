using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Repositories
{
    public class CsvFileAccess : ICsvFileRepository
    {
        public string GetData(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void WriteData(string filePath, string data)
        {
            File.WriteAllText(filePath, data);
        }
    }
}
