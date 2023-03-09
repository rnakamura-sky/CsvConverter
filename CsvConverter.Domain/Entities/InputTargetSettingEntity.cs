using System.Net.WebSockets;

namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// 入力ファイル項目をそのまま出力するための設定Entity
    /// </summary>
    public class InputTargetSettingEntity : BaseTargetSettingEntity
    {
        /// <summary>
        /// 入力ファイルの項目名
        /// </summary>
        public string InputHeaderName { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="inputHeaderName">入力ファイルの項目名</param>
        public InputTargetSettingEntity(string inputHeaderName)
        {
            InputHeaderName = inputHeaderName;
        }

        /// <summary>
        /// 項目の値を生成
        /// </summary>
        /// <param name="rowEntity">入力データ</param>
        /// <returns></returns>
        public override string CreateFieldValue(RowEntity rowEntity)
        {
            return rowEntity.GetField(InputHeaderName).FieldValue;
        }
    }
}
