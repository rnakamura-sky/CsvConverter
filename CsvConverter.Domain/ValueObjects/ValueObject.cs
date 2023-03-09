namespace CsvConverter.Domain.ValueObjects
{
    /// <summary>
    /// オブジェクトをプリミティブ型と同様の値比較が行えるようにするBaseクラス
    /// </summary>
    /// <typeparam name="T">オブジェクトが保持する値の型</typeparam>
    public abstract class ValueObject<T> where T : ValueObject<T>
    {

        public override bool Equals(object? obj)
        {
            T? vo = obj as T;
            if (vo is null)
            {
                return false;
            }
            return EqualsCore(vo);
        }

        public static bool operator ==(ValueObject<T> vo1, ValueObject<T> vo2)
        {
            return Equals(vo1, vo2);
        }

        public static bool operator !=(ValueObject<T> vo1, ValueObject<T> vo2)
        {
            return !Equals(vo1, vo2);
        }

        public override int GetHashCode()
        {
            return GetHashCodeCore();
        }

        protected abstract bool EqualsCore(T other);

        protected abstract int GetHashCodeCore();

    }
}
