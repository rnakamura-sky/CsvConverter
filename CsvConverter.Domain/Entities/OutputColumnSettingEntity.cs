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
        /// 入力ファイルの項目かのチェック
        /// </summary>
        public bool IsInputField { get; }

        /// <summary>
        /// 入力ファイルの場合の入力ファイルヘッダー名
        /// </summary>
        public string InputHeader { get; }

        /// <summary>
        /// 出力有無フラグ
        /// </summary>
        public bool IsOutput { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="isInputField">入力ファイル項目かのチェック</param>
        /// <param name="inputHeader">入力ファイル項目でのヘッダー名</param>
        /// <param name="isOutput">出力有無フラグ</param>
        public OutputColumnSettingEntity(int index, bool isInputField, string inputHeader, bool isOutput)
        {
            Index = index;
            IsInputField = isInputField;
            InputHeader = inputHeader;
            IsOutput = isOutput;
        }
    }
}
