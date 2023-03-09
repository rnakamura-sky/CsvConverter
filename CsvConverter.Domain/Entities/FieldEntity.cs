namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// CSVファイルの一つの項目について管理するEntity
    /// ヘッダー情報も保持しています。
    /// </summary>
    public sealed class FieldEntity
    {
        /// <summary>
        /// None
        /// </summary>
        public static readonly FieldEntity None = new FieldEntity(HeaderEntity.None, string.Empty);

        /// <summary>
        /// ヘッダー情報
        /// </summary>
        public HeaderEntity Header { get; }

        /// <summary>
        /// 値
        /// </summary>
        public string FieldValue { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="header">ヘッダー情報</param>
        /// <param name="data">項目の値</param>
        public FieldEntity(HeaderEntity header, string data)
        {
            Header = header;
            FieldValue = data;
        }
    }
}
