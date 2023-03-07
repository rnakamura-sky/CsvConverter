using CsvConverter.Domain.Entities;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace CsvConverter.WPF.ViewModels
{
    public class CsvConvertViewModelRow : BindableBase
    {
        private readonly RowEntity _entity;

        private ObservableCollection<FieldEntity> _fields = new();
        public ObservableCollection<FieldEntity> Fields
        {
            get { return _fields; }
            set { SetProperty(ref _fields, value); }
        }

        public CsvConvertViewModelRow(RowEntity rowEntity)
        {
            _entity = rowEntity;
            foreach (var field in _entity.Fields)
            {
                Fields.Add(field);
            }
        }
    }
}
