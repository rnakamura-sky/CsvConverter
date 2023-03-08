using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Repositories
{
    /// <summary>
    /// 入力CSVファイルリポジトリ
    /// </summary>
    public interface IInputCsvFileRepository
    {
        /// <summary>
        /// ファイル情報を取得
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        string GetData(string filePath);

    }
}
