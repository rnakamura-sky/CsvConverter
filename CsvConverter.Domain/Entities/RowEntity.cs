namespace CsvConverter.Domain.Entities
{
    public sealed class RowEntity
    {
        public IReadOnlyList<FieldEntity> Fields { get; }

        public RowEntity(IReadOnlyList<FieldEntity> fields)
        {
            Fields = fields;
        }

        public FieldEntity GetField(string header)
        {
            var field = Fields.Where(x => x.Header.Header == header).FirstOrDefault();
            return field ?? FieldEntity.None;
        }
    }
}
