namespace CsvConverter.Domain.Repositories
{
    /// <summary>
    /// CSVファイルリポジトリ
    /// ファイルIOを使用して実装しています。
    /// </summary>
    public class CsvFileAccess : ICsvFileRepository
    {
        /// <summary>
        /// ファイル情報取得
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイル内容</returns>
        public string GetData(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// ファイル情報書き込み
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="data">書き込み情報</param>
        public void WriteData(string filePath, string data)
        {
            File.WriteAllText(filePath, data);
        }
    }
}
