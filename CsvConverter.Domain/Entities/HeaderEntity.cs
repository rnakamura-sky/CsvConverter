namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// ヘッダー情報を管理するためのEntity
    /// </summary>
    public sealed class HeaderEntity
    {
        /// <summary>
        /// None
        /// </summary>
        public static readonly HeaderEntity None = new HeaderEntity(-1, string.Empty);

        /// <summary>
        /// Id ヘッダーの項番を表す
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// ヘッダー名
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">項番</param>
        /// <param name="header">ヘッダー名</param>
        public HeaderEntity(int id, string header)
        {
            Id = id;
            Header = header;
        }   
    }
}
