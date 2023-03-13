using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Repositories
{
    /// <summary>
    /// 設定管理リポジトリ
    /// </summary>
    public interface ISettingRepository
    {
        /// <summary>
        /// 設定を読み込む
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>設定情報</returns>
        SettingEntity Read(string filePath);

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="setting">設定情報</param>
        void Save(string filePath, SettingEntity setting);
    }
}
