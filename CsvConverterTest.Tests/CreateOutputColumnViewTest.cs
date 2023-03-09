using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;
using CsvConverter.WPF.ViewModels;
using Prism.Services.Dialogs;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class CreateOutputColumnViewTest
    {
        [TestMethod]
        public void シナリオ()
        {
            var parameters = new DialogParameters();
            var headers = new List<HeaderEntity>()
            {
                new HeaderEntity(0, "Field1"),
                new HeaderEntity(1, "Field2"),
                new HeaderEntity(2, "Field3"),
            };
            parameters.Add(nameof(CreateOutputColumnViewModel.Headers), headers);

            var viewModel = new CreateOutputColumnViewModel();
            viewModel.OnDialogOpened(parameters);
            viewModel.RequestClose += (result) => {
                Assert.AreEqual(ButtonResult.OK, result.Result);
                var column = result.Parameters.GetValue<OutputColumnSettingEntity>(nameof(OutputColumnSettingEntity));
                Assert.IsNotNull(column);
                Assert.AreEqual("Field4", column.OutputHeader);
                Assert.AreEqual("Field2", ((InputTargetSettingEntity)column.TargetSetting).InputHeaderName);
                Assert.AreEqual(true, column.IsOutput);
            };

            Assert.AreEqual("", viewModel.HeaderName);
            Assert.AreEqual(true, viewModel.IsOutput);

            viewModel.SelectedTargetSettingType = TargetSettingType.Input;
            var target = viewModel.Target as CreateOutputColumnViewModelTargetInput;
            Assert.IsNotNull(target);
            Assert.IsNull(target.SelectedHeader);
            Assert.AreEqual(3, target.Headers.Count);
            Assert.AreEqual("Field1", target.Headers[0].Header);
            Assert.AreEqual("Field2", target.Headers[1].Header);
            Assert.AreEqual("Field3", target.Headers[2].Header);

            viewModel.HeaderName = "Field4";
            target.SelectedHeader = target.Headers[1];

            viewModel.CreateCommand.Execute();
        }
    }
}
