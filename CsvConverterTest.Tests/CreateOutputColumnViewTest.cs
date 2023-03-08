using CsvConverter.Domain.Entities;
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
                Assert.AreEqual("Field2", column.InputHeader);
                Assert.AreEqual(false, column.IsInputField);
                Assert.AreEqual(true, column.IsOutput);
            };

            Assert.AreEqual("", viewModel.HeaderName);
            Assert.AreEqual(true, viewModel.IsOutput);
            Assert.IsNull(viewModel.SelectedInputHeader);
            Assert.AreEqual(false, viewModel.IsInputHeader);
            Assert.AreEqual(3, viewModel.Headers.Count);
            Assert.AreEqual("Field1", viewModel.Headers[0].Header);
            Assert.AreEqual("Field2", viewModel.Headers[1].Header);
            Assert.AreEqual("Field3", viewModel.Headers[2].Header);

            viewModel.HeaderName = "Field4";
            viewModel.SelectedInputHeader = viewModel.Headers[1];

            viewModel.CreateCommand.Execute();
        }
    }
}
