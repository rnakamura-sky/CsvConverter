namespace CsvConverter.Domain.ValueObjects
{
    /// <summary>
    /// 出力項目の生成方法管理タイプ
    /// </summary>
    public class TargetSettingType : ValueObject<TargetSettingType>
    {
        public static readonly TargetSettingType None = new(0, "None", "(なし)");
        public static readonly TargetSettingType Input = new(1, "Input", "入力項目");

        /// <summary>
        /// 値
        /// </summary>

        public int Value { get; }

        /// <summary>
        /// コード値
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="code">コード値</param>
        /// <param name="name">名前</param>
        private TargetSettingType(int value, string code, string name)
        {
            Value = value;
            Code = code;
            Name = name;
        }

        /// <summary>
        /// TargetSettingType一覧取得
        /// </summary>
        /// <returns>TargetSettingType一覧</returns>
        public static IReadOnlyList<TargetSettingType> GetTargetSettingTypes()
        {
            return new List<TargetSettingType>() {
                Input,
            };
        }


        protected override bool EqualsCore(TargetSettingType other)
        {
            return other.Value == Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}
