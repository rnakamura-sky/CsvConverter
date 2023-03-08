namespace CsvConverter.Domain.Repositories
{
    /// <summary>
    /// 出力CSVファイルリポジトリ
    /// </summary>
    public interface IOutputCsvFileRepository
    {
        /// <summary>
        /// ファイル情報書き込み
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="data">書き込み情報</param>
        void WriteData(string filePath, string data);
    }
}
