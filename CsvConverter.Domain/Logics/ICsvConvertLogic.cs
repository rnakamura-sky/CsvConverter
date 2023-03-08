using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Logics
{
    /// <summary>
    /// 出力ファイル情報作成ロジック
    /// </summary>
    public interface ICsvConvertLogic
    {
        /// <summary>
        /// 出力ファイル作成
        /// </summary>
        /// <param name="inputCsvFile">入力CSVファイル情報</param>
        /// <param name="outputCsvFile">出力CSVファイル情報</param>
        /// <param name="outputSetting">出力設定情報</param>
        void Execute(
            InputCsvFileEntity inputCsvFile,
            OutputCsvFileEntity outputCsvFile,
            OutputSettingEntity outputSetting);
    }
}
