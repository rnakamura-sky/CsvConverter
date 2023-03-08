using CsvConverter.Domain.Entities;
using Prism.Mvvm;

namespace CsvConverter.WPF.ViewModels
{
    /// <summary>
    /// ヘッダー情報ViewModel
    /// </summary>
    public class CsvConvertViewModelHeader : BindableBase
    {
        /// <summary>
        /// 入力元となったヘッダー情報
        /// </summary>
        private readonly HeaderEntity _entity;

        private string _fieldName;

        /// <summary>
        /// ヘッダー名
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set { SetProperty(ref _fieldName, value); }
        }

        private bool _isOutput;

        /// <summary>
        /// 出力有無
        /// </summary>
        public bool IsOutput
        {
            get { return _isOutput; }
            set { SetProperty(ref _isOutput, value); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="headerEntity"></param>
        public CsvConvertViewModelHeader(HeaderEntity headerEntity)
        {
            _entity = headerEntity;
            FieldName = _entity.Header;
            IsOutput = true;
        }

        /// <summary>
        /// 出力行情報Entity取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OutputColumnSettingEntity GetColumnSettingEntity(int index)
        {
            return new OutputColumnSettingEntity(index, true, FieldName, IsOutput);
        }
    }
}
