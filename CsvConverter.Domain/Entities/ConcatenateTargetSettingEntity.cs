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
        public override string CreateFieldValue(RowEntity rowEntity)
        {
            var result = string.Empty;
            foreach (var header in OutputHeaderEntities)
            {
                result += rowEntity.GetField(header.HeaderName).FieldValue;
            }

            return result;
        }
    }
}
