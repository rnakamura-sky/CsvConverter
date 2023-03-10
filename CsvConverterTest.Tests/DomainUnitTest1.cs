using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using Moq;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class DomainUnitTest1
    {
        [TestMethod]
        public void CSVファイルを読み取って出力する()
        {
            var inputFileString =
@"Field1,Field2,Field3,Field4,Field5
A,B,C,D,E
a,b,c,d,e
";

            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            inputCsvFileMock.Setup(x => x.GetData("InputFilePath")).Returns(inputFileString);
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", It.IsAny<string>())).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                Assert.AreEqual(inputFileString, data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");

            var data = inputCsvFile.GetData();
            Assert.AreEqual(true, data.HasHeader);
            Assert.AreEqual(5, data.Headers.Count);
            Assert.AreEqual(0, data.Headers[0].HeaderId);
            Assert.AreEqual("Field1", data.Headers[0].HeaderName);
            Assert.AreEqual(1, data.Headers[1].HeaderId);
            Assert.AreEqual("Field2", data.Headers[1].HeaderName);
            Assert.AreEqual(2, data.Headers[2].HeaderId);
            Assert.AreEqual("Field3", data.Headers[2].HeaderName);
            Assert.AreEqual(3, data.Headers[3].HeaderId);
            Assert.AreEqual("Field4", data.Headers[3].HeaderName);
            Assert.AreEqual(4, data.Headers[4].HeaderId);
            Assert.AreEqual("Field5", data.Headers[4].HeaderName);
            Assert.AreEqual(2, data.Data.Count);
            Assert.AreEqual(5, data.Data[0].Fields.Count);
            Assert.AreEqual("A", data.Data[0].Fields[0].FieldValue);
            Assert.AreEqual("B", data.Data[0].Fields[1].FieldValue);
            Assert.AreEqual("C", data.Data[0].Fields[2].FieldValue);
            Assert.AreEqual("D", data.Data[0].Fields[3].FieldValue);
            Assert.AreEqual("E", data.Data[0].Fields[4].FieldValue);
            Assert.AreEqual(5, data.Data[1].Fields.Count);
            Assert.AreEqual("a", data.Data[1].Fields[0].FieldValue);
            Assert.AreEqual("b", data.Data[1].Fields[1].FieldValue);
            Assert.AreEqual("c", data.Data[1].Fields[2].FieldValue);
            Assert.AreEqual("d", data.Data[1].Fields[3].FieldValue);
            Assert.AreEqual("e", data.Data[1].Fields[4].FieldValue);
            Assert.AreEqual(inputFileString, data.GetFileString());

            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");
            outputCsvFile.WriteData(data);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();

        }

        [TestMethod]
        public void そのまま出力シナリオ()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            inputCsvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns("SampleData");
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", It.IsAny<string>())).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                Assert.AreEqual("SampleData\r\n", data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");
            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");


            var logic = new CsvConvertLogic();
            logic.Execute(inputCsvFile, outputCsvFile, OutputSettingEntity.None);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();

        }

        [TestMethod]
        public void 簡単なCSVそのまま出力シナリオ()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            var inputFileContent =
@"Field1,Field2,Field3
a,b,c
C,A,B
う,あ,い
";
            inputCsvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(inputFileContent);
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", It.IsAny<string>())).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                var resultContent =
@"Field1,Field2,Field3
a,b,c
C,A,B
う,あ,い
";
                Assert.AreEqual(resultContent, data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");
            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");


            var logic = new CsvConvertLogic();
            logic.Execute(inputCsvFile, outputCsvFile, OutputSettingEntity.None);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();
        }

        [TestMethod]
        public void 自分で項目作成シナリオ()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            var inputFileContent =
@"Field1,Field2,Field3
a,b,c
C,A,B
う,あ,い
";
            inputCsvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(inputFileContent);
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", It.IsAny<string>())).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                var resultContent =
@"OutputField1,OutputField2,OutputField3
a,b,c
C,A,B
う,あ,い
";
                Assert.AreEqual(resultContent, data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");
            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");

            var outputSetting = new OutputSettingEntity(new List<OutputColumnSettingEntity>()
            {
                new OutputColumnSettingEntity(0, "OutputField1", true, new InputTargetSettingEntity("Field1")),
                new OutputColumnSettingEntity(1, "OutputField2", true, new InputTargetSettingEntity("Field2")),
                new OutputColumnSettingEntity(2, "OutputField3", true, new InputTargetSettingEntity("Field3")),
            });

            var logic = new CsvConvertLogic();
            logic.Execute(inputCsvFile, outputCsvFile, outputSetting);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();
        }

        [TestMethod]
        public void フォーマット変換シナリオ_項目の連結()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            var inputFileContent =
@"Field1,Field2,Field3
a,b,c
C,A,B
う,あ,い
";
            inputCsvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(inputFileContent);
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", It.IsAny<string>())).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                var resultContent =
@"Field1,Field2,Field3,Concatenate
a,b,c,bac
C,A,B,ACB
う,あ,い,あうい
";
                Assert.AreEqual(resultContent, data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");
            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");

            var outputSetting = new OutputSettingEntity(new List<OutputColumnSettingEntity>()
            {
                new OutputColumnSettingEntity(0, "Field1", true, new InputTargetSettingEntity("Field1")),
                new OutputColumnSettingEntity(1, "Field2", true, new InputTargetSettingEntity("Field2")),
                new OutputColumnSettingEntity(2, "Field3", true, new InputTargetSettingEntity("Field3")),
                new OutputColumnSettingEntity(3, "Concatenate", true, new ConcatenateTargetSettingEntity(
                    new List<HeaderEntity>(){
                        new HeaderEntity(1, "Field2"),
                        new HeaderEntity(0, "Field1"),
                        new HeaderEntity(3, "Field3"),
                    })),
            });

            var logic = new CsvConvertLogic();
            logic.Execute(inputCsvFile, outputCsvFile, outputSetting);

            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();
        }

        [TestMethod]
        public void 自作入力項目と連結項目の組み合わせシナリオ()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            var inputFileContent =
@"Field1,Field2,Field3
a,b,c
C,A,B
う,あ,い
";
            inputCsvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(inputFileContent);
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", It.IsAny<string>())).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                var resultContent =
@"Field1,Field2,Field3,OutputField4,Concatenate
a,b,c,b,bab
C,A,B,A,ACA
う,あ,い,あ,あうあ
";
                Assert.AreEqual(resultContent, data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");
            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");

            var outputSetting = new OutputSettingEntity(new List<OutputColumnSettingEntity>()
            {
                new OutputColumnSettingEntity(0, "Field1", true, new InputTargetSettingEntity("Field1")),
                new OutputColumnSettingEntity(1, "Field2", true, new InputTargetSettingEntity("Field2")),
                new OutputColumnSettingEntity(2, "Field3", true, new InputTargetSettingEntity("Field3")),
                new OutputColumnSettingEntity(3, "OutputField4", true, new InputTargetSettingEntity("Field2")),
                new OutputColumnSettingEntity(4, "Concatenate", true, new ConcatenateTargetSettingEntity(
                    new List<HeaderEntity>(){
                        new HeaderEntity(1, "Field2"),
                        new HeaderEntity(0, "Field1"),
                        new HeaderEntity(3, "OutputField4"),
                    })),
            });

            var logic = new CsvConvertLogic();
            logic.Execute(inputCsvFile, outputCsvFile, outputSetting);

            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();
        }
    }
}