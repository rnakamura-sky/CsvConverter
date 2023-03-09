namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// 出力項目の値を生成する方法を管理するEntityのベース
    /// </summary>
    public abstract class BaseTargetSettingEntity
    {
        public abstract string CreateFieldValue(RowEntity rowEntity);
    }
}
