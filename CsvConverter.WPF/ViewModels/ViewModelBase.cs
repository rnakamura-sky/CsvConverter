using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CsvConverter.WPF.ViewModels
{
    /// <summary>
    /// エラー処理を行う用ViewModelBase
    /// </summary>
    public abstract class ViewModelBase : BindableBase, INotifyDataErrorInfo
    {
        /// <summary>
        /// PrismValidationError用エラーコンテナ
        /// </summary>
        private ErrorsContainer<string> _errors;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewModelBase()
        {
            _errors = new ErrorsContainer<string>(OnErrorsChanged);
        }

        /// <summary>
        /// エラー情報保持チェック
        /// </summary>
        public bool HasErrors => _errors.HasErrors;

        /// <summary>
        /// エラー更新イベント
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// エラー取得
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return _errors.GetErrors(propertyName);
        }

        /// <summary>
        /// プロパティ値チェック
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            var context = new ValidationContext(this)
            {
                MemberName = propertyName,
            };
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                _errors.SetErrors(propertyName, validationErrors.Select(error => error.ErrorMessage));
            }
            else
            {
                _errors.ClearErrors(propertyName);
            }
        }

        /// <summary>
        /// プロパティ設定に検証を行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetValidateProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);
            ValidateProperty(value, propertyName);
            return result;
        }

        /// <summary>
        /// 全てのエラーの中で最初のエラーを取得
        /// </summary>
        /// <returns></returns>
        internal KeyValuePair<string, string> GetFirstError()
        {
            var firstError = _errors.GetErrors()
                    .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.First()))
                    .First();
            return firstError;
        }

        /// <summary>
        /// プロパティ名を取得
        /// エラー表示等、ソースコードのプロパティ名を日本語に変換する時に使用します。
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal virtual string GetPropertyName(string propertyName)
        {
            var prop = this.GetType().GetProperty(propertyName);
            if (prop is null)
            {
                return propertyName;
            }

            var displayAttribute = prop.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute is null)
            {
                return propertyName;
            }
            return displayAttribute.GetName();
        }

        /// <summary>
        /// エラーメッセージを取得します。
        /// </summary>
        /// <returns></returns>
        internal virtual string GetFirstErrorMessage()
        {
            var firstError = GetFirstError();
            var propertyName = GetPropertyName(firstError.Key);
            return $"[{propertyName}] {firstError.Value}";
        }

        /// <summary>
        /// 全てのプロパティエラーチェック
        /// </summary>
        /// <returns></returns>
        internal bool ValidateAllObjects()
        {
            if (!HasErrors)
            {
                var context = new ValidationContext(this);
                var validationErrors = new List<ValidationResult>();

                ////値についても確認を行うためvalidateAllProperties=trueとする
                if (Validator.TryValidateObject(this, context, validationErrors, true))
                {
                    return true;
                }

                var errors = validationErrors
                    .Where(x => x.MemberNames.Any())
                    .GroupBy(x => x.MemberNames.First());
                foreach (var error in errors)
                {
                    _errors.SetErrors(error.Key, error.Select(x => x.ErrorMessage));
                }
            }
            return false;
        }

        /// <summary>
        /// エラー情報更新処理
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            var handler = ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}
