using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;
using Prism.Mvvm;

namespace CsvConverter.WPF.ViewModels
{
    public abstract class CreateOutputColumnViewModelTargetBase : BindableBase
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
