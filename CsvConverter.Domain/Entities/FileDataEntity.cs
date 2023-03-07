namespace CsvConverter.Domain.Entities
{
    public sealed class FileDataEntity
    {
        public string FileString { get; }
        public FileDataEntity()
            : this(string.Empty)
        {

        }

        public FileDataEntity(string fileString)
        {
            FileString = fileString;
        }
        public string GetFileString()
        {
            return FileString;
        }
    }
}
