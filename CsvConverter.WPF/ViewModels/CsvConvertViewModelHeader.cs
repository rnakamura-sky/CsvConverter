using CsvConverter.Domain.Entities;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace CsvConverter.WPF.ViewModels
{
    public class CsvConvertViewModelHeader : BindableBase
    {
        private readonly HeaderEntity _entity;

        private string _fieldName;
        public string FieldName
        {
            get { return _fieldName; }
            set { SetProperty(ref _fieldName, value); }
        }

        public CsvConvertViewModelHeader(HeaderEntity headerEntity)
        {
            _entity = headerEntity;
            FieldName = _entity.Header;
        }
    }
}
