namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// 出力列情報設定管理Entity
    /// </summary>
    public class OutputColumnSettingEntity
    {
        /// <summary>
        /// インデックス　出力される順
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 出力有無フラグ
        /// </summary>
        public bool IsOutput { get; }

        /// <summary>
        /// 出力ヘッダー名
        /// </summary>
        public string OutputHeader { get; }

        /// <summary>
        /// 
        /// </summary>
        public BaseTargetSettingEntity TargetSetting { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        /// <param name="outputHeader"></param>
        /// <param name="isOutput"></param>
        /// <param name="targetSettingEntity"></param>
        public OutputColumnSettingEntity(int index, string outputHeader, bool isOutput, BaseTargetSettingEntity targetSettingEntity)
        {
            Index = index;
            OutputHeader = outputHeader;
            IsOutput = isOutput;
            TargetSetting = targetSettingEntity;
        }

        /// <summary>
        /// 項目の値を生成
        /// </summary>
        /// <param name="rowEntity">入力データ</param>
        /// <returns></returns>
        public string CreateFieldValue(RowEntity inputRowEntity, RowEntity outputRowEntity)
        {
            return TargetSetting.CreateFieldValue(inputRowEntity, outputRowEntity);
        }

        /// <summary>
        /// 項目がどのような加工を行うか文字列で取得
        /// </summary>
        /// <returns></returns>
        public string GetFieldContent()
        {
            return TargetSetting.GetFieldContent();
        }

        /// <summary>
        /// 項目の入力項目を取得
        /// 取得が入力項目ではない場合は空白文字列を返す
        /// </summary>
        /// <returns></returns>
        public string GetInputFieldName()
        {
            return TargetSetting.GetInputFieldName();
        }
    }
}
