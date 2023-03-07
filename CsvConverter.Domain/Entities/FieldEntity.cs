namespace CsvConverter.Domain.Entities
{
    public sealed class FieldEntity
    {
        public static readonly FieldEntity None = new FieldEntity(HeaderEntity.None, string.Empty);

        public HeaderEntity Header { get; }

        public string Data { get; }

        public FieldEntity(HeaderEntity header, string data)
        {
            Header = header;
            Data = data;
        }
    }
}
