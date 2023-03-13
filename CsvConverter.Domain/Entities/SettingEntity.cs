namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// CSV変換設定管理Entity
    /// </summary>
    public sealed class SettingEntity
    {
        /// <summary>
        /// 設定名称
        /// </summary>
        public string SettingName { get; }

        /// <summary>
        /// 入力ファイルヘッダー情報
        /// </summary>
        public IReadOnlyList<HeaderEntity> Headers { get; }

        /// <summary>
        /// 出力ファイル情報
        /// </summary>
        public OutputSettingEntity OutputSetting { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingEntity()
            : this(
                  string.Empty,
                  new List<HeaderEntity>(),
                  new OutputSettingEntity(new List<OutputColumnSettingEntity>()))
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settingName">設定名称</param>
        /// <param name="headers">入力ヘッダー情報</param>
        /// <param name="outputSetting">出力情報</param>
        public SettingEntity(string settingName, IReadOnlyList<HeaderEntity> headers, OutputSettingEntity outputSetting)
        {
            SettingName = settingName;
            Headers = headers;
            OutputSetting = outputSetting;
        }
    }
}
