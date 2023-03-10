using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;

namespace CsvConverter.WPF.ViewModels
{
    public abstract class CreateOutputColumnViewModelTargetBase : ViewModelBase
	{
        /// <summary>
        /// TargetSettingType
        /// </summary>
        public TargetSettingType TargetSettingType { get; }

        public CreateOutputColumnViewModelTargetBase(TargetSettingType targetSettingType)
        {
            TargetSettingType = targetSettingType;
        }

        /// <summary>
        /// TargetSetting取得
        /// </summary>
        /// <returns></returns>
        public abstract BaseTargetSettingEntity GetTargetSettingEntity();
	}
}
