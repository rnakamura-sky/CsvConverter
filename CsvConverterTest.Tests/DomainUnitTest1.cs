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
a,b,c,d,e";

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
            Assert.AreEqual(0, data.Headers[0].Id);
            Assert.AreEqual("Field1", data.Headers[0].Header);
            Assert.AreEqual(1, data.Headers[1].Id);
            Assert.AreEqual("Field2", data.Headers[1].Header);
            Assert.AreEqual(2, data.Headers[2].Id);
            Assert.AreEqual("Field3", data.Headers[2].Header);
            Assert.AreEqual(3, data.Headers[3].Id);
            Assert.AreEqual("Field4", data.Headers[3].Header);
            Assert.AreEqual(4, data.Headers[4].Id);
            Assert.AreEqual("Field5", data.Headers[4].Header);
            Assert.AreEqual(2, data.Data.Count);
            Assert.AreEqual(5, data.Data[0].Fields.Count);
            Assert.AreEqual("A", data.Data[0].Fields[0].Data);
            Assert.AreEqual("B", data.Data[0].Fields[1].Data);
            Assert.AreEqual("C", data.Data[0].Fields[2].Data);
            Assert.AreEqual("D", data.Data[0].Fields[3].Data);
            Assert.AreEqual("E", data.Data[0].Fields[4].Data);
            Assert.AreEqual(5, data.Data[1].Fields.Count);
            Assert.AreEqual("a", data.Data[1].Fields[0].Data);
            Assert.AreEqual("b", data.Data[1].Fields[1].Data);
            Assert.AreEqual("c", data.Data[1].Fields[2].Data);
            Assert.AreEqual("d", data.Data[1].Fields[3].Data);
            Assert.AreEqual("e", data.Data[1].Fields[4].Data);
            Assert.AreEqual(inputFileString, data.GetFileString());

            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");
            outputCsvFile.WriteData(data);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();

        }

        [TestMethod]
        public void ロジックのテスト()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            inputCsvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns("SampleData");
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", "SampleData")).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                Assert.AreEqual("SampleData", data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");
            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");


            var logic = new CsvConvertLogic();
            logic.Execute(inputCsvFile, outputCsvFile);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();

        }
    }
}