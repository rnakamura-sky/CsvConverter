namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// 結合設定Entity
    /// </summary>
    public class ConcatenateTargetSettingEntity : BaseTargetSettingEntity
    {
        /// <summary>
        /// 結合する項目リスト
        /// </summary>
        public List<HeaderEntity> OutputHeaderEntities { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="outputHeaderEntities"></param>
        public ConcatenateTargetSettingEntity(IReadOnlyList<HeaderEntity> outputHeaderEntities)
        {
            OutputHeaderEntities = new List<HeaderEntity>();
            foreach (var entity in outputHeaderEntities)
            {
                OutputHeaderEntities.Add(entity);
            }
        }

        /// <summary>
        /// 項目の値生成
        /// </summary>
        /// <param name="rowEntity"></param>
        /// <returns></returns>
        public override string CreateFieldValue(RowEntity inputRowEntity, RowEntity outputRowEntity)
        {
            var result = string.Empty;
            foreach (var header in OutputHeaderEntities)
            {
                result += outputRowEntity.GetField(header.HeaderName).FieldValue;
            }

            return result;
        }

        /// <summary>
        /// 項目がどのような加工を行うか文字列で取得
        /// </summary>
        /// <returns></returns>
        public override string GetFieldContent()
        {
            var result = string.Empty;
            foreach (var header in OutputHeaderEntities)
            {
                if (result == string.Empty)
                {
                    result += header.HeaderName;
                    continue;
                }
                result += $"+{header.HeaderName}";
            }

            return result;
        }

        /// <summary>
        /// 項目の入力項目を取得
        /// 取得が入力項目ではない場合は空白文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string GetInputFieldName()
        {
            return string.Empty;
        }
    }
}
