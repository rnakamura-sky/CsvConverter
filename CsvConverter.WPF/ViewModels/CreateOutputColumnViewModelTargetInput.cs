using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "入力元項目")]
        [Required(ErrorMessage = "選択してください。")]
        public HeaderEntity SelectedHeader
        {
            get { return _selectedHeader; }
            set { SetValidateProperty(ref _selectedHeader, value); }
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
            return new InputTargetSettingEntity(SelectedHeader.HeaderName);
        }
    }
}
