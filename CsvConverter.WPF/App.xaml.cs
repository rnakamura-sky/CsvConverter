using CsvConverter.WPF.Views;
using Prism.Ioc;
using System.Windows;

namespace CsvConverter.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ////画面遷移用登録
            containerRegistry.Register<Views.CsvConvertView>();

            ////ダイアログ用登録
            containerRegistry.RegisterDialog<Views.CreateOutputColumnView>();
        }
    }
}
