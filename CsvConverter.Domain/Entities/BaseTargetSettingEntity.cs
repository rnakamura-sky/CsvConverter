namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// 出力項目の値を生成する方法を管理するEntityのベース
    /// </summary>
    public abstract class BaseTargetSettingEntity
    {
        /// <summary>
        /// 項目値を生成
        /// </summary>
        /// <param name="inputRowEntity"></param>
        /// <param name="outputRowEntity"></param>
        /// <returns></returns>
        public abstract string CreateFieldValue(RowEntity inputRowEntity, RowEntity outputRowEntity);

        /// <summary>
        /// 項目がどのような加工を行うか文字列で取得
        /// </summary>
        /// <returns></returns>
        public abstract string GetFieldContent();

        /// <summary>
        /// 項目の入力項目を取得
        /// 取得が入力項目ではない場合は空白文字列を返す
        /// </summary>
        /// <returns></returns>
        public abstract string GetInputFieldName();
    }
}
