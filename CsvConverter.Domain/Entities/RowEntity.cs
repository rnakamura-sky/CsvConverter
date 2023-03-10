using CsvConverter.Domain.Exceptions;

namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// 行情報を管理するEntity
    /// </summary>
    public sealed class RowEntity
    {
        /// <summary>
        /// 項目情報
        /// </summary>
        public IReadOnlyList<FieldEntity> Fields { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RowEntity()
            : this(new List<FieldEntity>())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fields">項目情報</param>
        public RowEntity(IReadOnlyList<FieldEntity> fields)
        {
            Fields = fields;
        }

        /// <summary>
        /// 指定されたヘッダー名の項目情報取得
        /// </summary>
        /// <param name="header">ヘッダー名</param>
        /// <returns>項目情報Entity</returns>
        public FieldEntity GetField(string header)
        {
            var field = Fields.Where(x => x.Header.HeaderName == header).FirstOrDefault();
            if (field is null)
            {
                throw new InvalidException();
            }
            return field;
        }

        /// <summary>
        /// 項目情報追加
        /// </summary>
        /// <param name="fieldEntity"></param>
        /// <returns></returns>
        public RowEntity Add(FieldEntity fieldEntity)
        {
            var fields = Fields.ToList();
            fields.Add(fieldEntity);
            return new RowEntity(fields);
        }
    }
}
