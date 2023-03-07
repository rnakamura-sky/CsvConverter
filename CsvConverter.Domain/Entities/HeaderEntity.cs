namespace CsvConverter.Domain.Entities
{
    public sealed class HeaderEntity
    {
        public static readonly HeaderEntity None = new HeaderEntity(0, string.Empty);

        public int Id { get; }
        public string Header { get; }

        public HeaderEntity(int id, string header)
        {
            Id = id;
            Header = header;
        }   
    }
}
