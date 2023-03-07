namespace CsvConverter.Domain.Entities
{
    public sealed class RowEntity
    {
        public IReadOnlyList<FieldEntity> Fields { get; }

        public RowEntity(IReadOnlyList<FieldEntity> fields)
        {
            Fields = fields;
        }
    }
}
