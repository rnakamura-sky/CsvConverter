using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;
using CsvConverter.WPF.Services;
using CsvConverter.WPF.ViewModels;
using Moq;
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
            var inputHeaders = new List<HeaderEntity>()
            {
                new HeaderEntity(0, "Field1"),
                new HeaderEntity(1, "Field2"),
                new HeaderEntity(2, "Field3"),
            };
            parameters.Add(nameof(CreateOutputColumnViewModel.InputHeaders), inputHeaders);
            parameters.Add(nameof(CreateOutputColumnViewModel.OutputHeaders), new List<HeaderEntity>());

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
            Assert.AreEqual("Field1", target.Headers[0].HeaderName);
            Assert.AreEqual("Field2", target.Headers[1].HeaderName);
            Assert.AreEqual("Field3", target.Headers[2].HeaderName);

            viewModel.HeaderName = "Field4";
            target.SelectedHeader = target.Headers[1];

            viewModel.CreateCommand.Execute();
        }

        [TestMethod]
        public void 入力項目設定の検証チェック()
        {
            var parameters = new DialogParameters();
            var inputHeaders = new List<HeaderEntity>()
            {
                new HeaderEntity(0, "Field1"),
                new HeaderEntity(1, "Field2"),
                new HeaderEntity(2, "Field3"),
            };
            parameters.Add(nameof(CreateOutputColumnViewModel.InputHeaders), inputHeaders);
            parameters.Add(nameof(CreateOutputColumnViewModel.OutputHeaders), new List<HeaderEntity>());

            var messageServiceMock = new Mock<IMessageService>();
            var viewModel = new CreateOutputColumnViewModel(messageServiceMock.Object);
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

            messageServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>()))
                .Callback((string message) => {
                    Assert.AreEqual("[新規項目名] 必須入力です。", message);
                });
            viewModel.CreateCommand.Execute();

            viewModel.HeaderName = "Field4";
            messageServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>()))
                .Callback((string message) => {
                    Assert.AreEqual("[項目タイプ] 選択してください。", message);
                });
            viewModel.CreateCommand.Execute();

            viewModel.SelectedTargetSettingType = TargetSettingType.Input;
            messageServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>()))
                .Callback((string message) => {
                    Assert.AreEqual("[入力元項目] 選択してください。", message);
                });
            viewModel.CreateCommand.Execute();

            var target = viewModel.Target as CreateOutputColumnViewModelTargetInput;
            Assert.IsNotNull(target);
            Assert.IsNull(target.SelectedHeader);
            Assert.AreEqual(3, target.Headers.Count);
            Assert.AreEqual("Field1", target.Headers[0].HeaderName);
            Assert.AreEqual("Field2", target.Headers[1].HeaderName);
            Assert.AreEqual("Field3", target.Headers[2].HeaderName);

            target.SelectedHeader = target.Headers[1];

            viewModel.CreateCommand.Execute();

            messageServiceMock.VerifyAll();
        }

        [TestMethod]
        public void 結合項目作成シナリオ()
        {
            var parameters = new DialogParameters();
            var inputHeaders = new List<HeaderEntity>()
            {
                new HeaderEntity(0, "Field1"),
                new HeaderEntity(1, "Field2"),
                new HeaderEntity(2, "Field3"),
            };
            var outputHeaders = new List<HeaderEntity>()
            {
                new HeaderEntity(0, "OutputField1"),
                new HeaderEntity(1, "OutputField2"),
                new HeaderEntity(2, "OutputField3"),
            };
            parameters.Add(nameof(CreateOutputColumnViewModel.InputHeaders), inputHeaders);
            parameters.Add(nameof(CreateOutputColumnViewModel.OutputHeaders), outputHeaders);

            var messageServiceMock = new Mock<IMessageService>();
            var viewModel = new CreateOutputColumnViewModel(messageServiceMock.Object);
            viewModel.OnDialogOpened(parameters);
            viewModel.RequestClose += (result) => {
                Assert.AreEqual(ButtonResult.OK, result.Result);
                var column = result.Parameters.GetValue<OutputColumnSettingEntity>(nameof(OutputColumnSettingEntity));
                Assert.IsNotNull(column);
                Assert.AreEqual("OutputField4", column.OutputHeader);
                var target = column.TargetSetting as ConcatenateTargetSettingEntity;
                Assert.IsNotNull(target);
                Assert.AreEqual(3, target.OutputHeaderEntities.Count);
                Assert.AreEqual("OutputField1", target.OutputHeaderEntities[0].HeaderName);
                Assert.AreEqual("OutputField2", target.OutputHeaderEntities[1].HeaderName);
                Assert.AreEqual("OutputField3", target.OutputHeaderEntities[2].HeaderName);
                Assert.AreEqual(true, column.IsOutput);
            };

            Assert.AreEqual("", viewModel.HeaderName);
            Assert.AreEqual(true, viewModel.IsOutput);

            messageServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>()))
                .Callback((string message) => {
                    Assert.AreEqual("[新規項目名] 必須入力です。", message);
                });
            viewModel.CreateCommand.Execute();

            viewModel.HeaderName = "OutputField4";
            messageServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>()))
                .Callback((string message) => {
                    Assert.AreEqual("[項目タイプ] 選択してください。", message);
                });
            viewModel.CreateCommand.Execute();

            viewModel.SelectedTargetSettingType = TargetSettingType.Concatenate;
            messageServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>()))
                .Callback((string message) => {
                    Assert.AreEqual("[結合項目リスト] 項目を選択してください。", message);
                });
            viewModel.CreateCommand.Execute();



            viewModel.SelectedTargetSettingType = TargetSettingType.Concatenate;
            var target = viewModel.Target as CreateOutputColumnViewModelTargetConcatenate;
            Assert.IsNotNull(target);
            Assert.AreEqual(3, target.Headers.Count);
            Assert.AreEqual("OutputField1", target.Headers[0].HeaderName);
            Assert.AreEqual("OutputField2", target.Headers[1].HeaderName);
            Assert.AreEqual("OutputField3", target.Headers[2].HeaderName);
            Assert.AreEqual(0, target.ConcatenateHeaders.Count);

            target.SelectedHeader = target.Headers[0];
            target.SelectCommand.Execute();
            target.SelectedHeader = target.Headers[1];
            target.SelectCommand.Execute();
            target.SelectedHeader = target.Headers[2];
            target.SelectCommand.Execute();


            Assert.AreEqual(3, target.ConcatenateHeaders.Count);
            Assert.AreEqual("OutputField1", target.ConcatenateHeaders[0].HeaderName);
            Assert.AreEqual("OutputField2", target.ConcatenateHeaders[1].HeaderName);
            Assert.AreEqual("OutputField3", target.ConcatenateHeaders[2].HeaderName);

            viewModel.CreateCommand.Execute();

            messageServiceMock.VerifyAll();
        }
    }
}
