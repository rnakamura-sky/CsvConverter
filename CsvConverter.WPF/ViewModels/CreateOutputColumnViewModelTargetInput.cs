using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CsvConverter.WPF.ViewModels
{
    public class CreateOutputColumnViewModelTargetInput : CreateOutputColumnViewModelTargetBase
    {
        private ObservableCollection<HeaderEntity> _headers;
        public ObservableCollection<HeaderEntity> Headers
        {
            get { return _headers; }
            set { SetProperty(ref _headers, value); }
        }

        private HeaderEntity _selectedHeader;
        public HeaderEntity SelectedHeader
        {
            get { return _selectedHeader; }
            set { SetProperty(ref _selectedHeader, value); }
        }

        public CreateOutputColumnViewModelTargetInput(IReadOnlyList<HeaderEntity> headers)
            : base(TargetSettingType.Input)
        {
            Headers = new ObservableCollection<HeaderEntity>();
            foreach (var header in headers)
            {
                Headers.Add(header);
            }
        }

        public override BaseTargetSettingEntity GetTargetSettingEntity()
        {
            return new InputTargetSettingEntity(SelectedHeader.Header);
        }
    }
}
