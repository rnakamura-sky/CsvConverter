using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CsvConverter.WPF.ValidationAttributes
{
    /// <summary>
    /// ObservableCollectionの要素数を検証するための属性クラス
    /// </summary>
    public class CollectionCountAttribute : ValidationAttribute
    {
        /// <summary>
        /// 要素数が0以上であることを検証
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            var collection = value as IList;
            if (collection is null)
            {
                return false;
            }
            if (collection.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
