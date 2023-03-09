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
        public int HeaderId { get; }

        /// <summary>
        /// ヘッダー名
        /// </summary>
        public string HeaderName { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">項番</param>
        /// <param name="header">ヘッダー名</param>
        public HeaderEntity(int headerId, string headerName)
        {
            HeaderId = headerId;
            HeaderName = headerName;
        }   
    }
}
