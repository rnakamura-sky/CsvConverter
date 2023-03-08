using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Logics
{
    /// <summary>
    /// 出力ファイル情報作成ロジック
    /// </summary>
    public class CsvConvertLogic : ICsvConvertLogic
    {

        /// <summary>
        /// 出力ファイル情報作成
        /// </summary>
        /// <param name="inputCsvFile">入力CSVファイル情報</param>
        /// <param name="outputCsvFile">出力CSVファイル情報</param>
        /// <param name="setting">出力設定情報</param>
        public void Execute(InputCsvFileEntity inputCsvFile, OutputCsvFileEntity outputCsvFile, OutputSettingEntity setting)
        {
            var data = inputCsvFile.GetData();

            ////出力設定情報が無ければ、入力CSV情報からそのまま作成する
            var outputSetting = setting != OutputSettingEntity.None ? setting : new OutputSettingEntity(data);

            var outputData = outputSetting.CreateFileData(data);

            outputCsvFile.WriteData(outputData);
        }
    }
}
