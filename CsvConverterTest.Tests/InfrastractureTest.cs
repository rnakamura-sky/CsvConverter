using CsvConverter.Domain.Entities;
using CsvConverter.Infrastructure.Drive;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class InfrastractureTest
    {
        [TestMethod]
        public void シリアライズテスト()
        {
            var settingEntity = new SettingEntity(
                "SettingName",
                new List<HeaderEntity>() {
                    new HeaderEntity(0, "Field1"),
                    new HeaderEntity(1, "Field2"),
                    new HeaderEntity(2, "Field3"),
                },
                new OutputSettingEntity(
                    new List<OutputColumnSettingEntity>()
                    {
                        new OutputColumnSettingEntity(0, "OutputField1", true, new InputTargetSettingEntity("Field1")),
                        new OutputColumnSettingEntity(1, "OutputField2", true, new InputTargetSettingEntity("Field2")),
                        new OutputColumnSettingEntity(2, "OutputField3", true, new ConcatenateTargetSettingEntity(new List<HeaderEntity>(){
                            new HeaderEntity(1, "OutputField2"),
                            new HeaderEntity(0, "OutputField1"),
                        })),
                    }));

            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
                Converters =
                {
                    new ColumnTargetConverter(),
                },
            };
            var jsonSetting = new JsonSettingEntity(settingEntity);
            var serializeString = JsonSerializer.Serialize(jsonSetting, options);

            var result =
@"{
  ""SettingName"": ""SettingName"",
  ""Headers"": [
    {
      ""HeaderId"": 0,
      ""HeaderName"": ""Field1""
    },
    {
      ""HeaderId"": 1,
      ""HeaderName"": ""Field2""
    },
    {
      ""HeaderId"": 2,
      ""HeaderName"": ""Field3""
    }
  ],
  ""OutputSetting"": {
    ""ColumnSettings"": [
      {
        ""Index"": 0,
        ""IsOutput"": true,
        ""OutputHeader"": ""OutputField1"",
        ""TargetSetting"": {
          ""InputHeaderName"": ""Field1"",
          ""TargetTypeId"": 1
        }
      },
      {
        ""Index"": 1,
        ""IsOutput"": true,
        ""OutputHeader"": ""OutputField2"",
        ""TargetSetting"": {
          ""InputHeaderName"": ""Field2"",
          ""TargetTypeId"": 1
        }
      },
      {
        ""Index"": 2,
        ""IsOutput"": true,
        ""OutputHeader"": ""OutputField3"",
        ""TargetSetting"": {
          ""OutputHeaderEntities"": [
            {
              ""HeaderId"": 1,
              ""HeaderName"": ""OutputField2""
            },
            {
              ""HeaderId"": 0,
              ""HeaderName"": ""OutputField1""
            }
          ],
          ""TargetTypeId"": 2
        }
      }
    ]
  }
}";
            Assert.AreEqual(result, serializeString);
        }
        [TestMethod]
        public void デシリアライズテスト()
        {
            var document =
@"{
  ""SettingName"": ""SettingName"",
  ""Headers"": [
    {
      ""HeaderId"": 0,
      ""HeaderName"": ""Field1""
    },
    {
      ""HeaderId"": 1,
      ""HeaderName"": ""Field2""
    },
    {
      ""HeaderId"": 2,
      ""HeaderName"": ""Field3""
    }
  ],
  ""OutputSetting"": {
    ""ColumnSettings"": [
      {
        ""Index"": 0,
        ""IsOutput"": true,
        ""OutputHeader"": ""OutputField1"",
        ""TargetSetting"": {
          ""InputHeaderName"": ""Field1"",
          ""TargetTypeId"": 1
        }
      },
      {
        ""Index"": 1,
        ""IsOutput"": true,
        ""OutputHeader"": ""OutputField2"",
        ""TargetSetting"": {
          ""InputHeaderName"": ""Field2"",
          ""TargetTypeId"": 1
        }
      },
      {
        ""Index"": 2,
        ""IsOutput"": true,
        ""OutputHeader"": ""OutputField3"",
        ""TargetSetting"": {
          ""OutputHeaderEntities"": [
            {
              ""HeaderId"": 1,
              ""HeaderName"": ""OutputField2""
            },
            {
              ""HeaderId"": 0,
              ""HeaderName"": ""OutputField1""
            }
          ],
          ""TargetTypeId"": 2
        }
      }
    ]
  }
}";

            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
                Converters =
                {
                    new ColumnTargetConverter(),
                },
            };
            var deserialize = JsonSerializer.Deserialize(document, typeof(JsonSettingEntity), options) as JsonSettingEntity;

            Assert.IsNotNull(deserialize);

            var entity = deserialize.GetSettingEntity();
            Assert.AreEqual("SettingName", entity.SettingName);

            Assert.AreEqual(3, entity.Headers.Count);
            Assert.AreEqual(0, entity.Headers[0].HeaderId);
            Assert.AreEqual("Field1", entity.Headers[0].HeaderName);
            Assert.AreEqual(1, entity.Headers[1].HeaderId);
            Assert.AreEqual("Field2", entity.Headers[1].HeaderName);
            Assert.AreEqual(2, entity.Headers[2].HeaderId);
            Assert.AreEqual("Field3", entity.Headers[2].HeaderName);

            Assert.AreEqual(3, entity.OutputSetting.ColumnSettings.Count);
            var outputColumn0 = entity.OutputSetting.ColumnSettings[0];
            Assert.AreEqual(0, outputColumn0.Index);
            Assert.AreEqual("OutputField1", outputColumn0.OutputHeader);
            var target0 = outputColumn0.TargetSetting as InputTargetSettingEntity;
            Assert.IsNotNull(target0);
            Assert.AreEqual("Field1", target0.InputHeaderName);

            var outputColumn1 = entity.OutputSetting.ColumnSettings[1];
            Assert.AreEqual(1, outputColumn1.Index);
            Assert.AreEqual("OutputField2", outputColumn1.OutputHeader);
            var target1 = outputColumn1.TargetSetting as InputTargetSettingEntity;
            Assert.IsNotNull(target1);
            Assert.AreEqual("Field2", target1.InputHeaderName);

            var outputColumn2 = entity.OutputSetting.ColumnSettings[2];
            Assert.AreEqual(2, outputColumn2.Index);
            Assert.AreEqual("OutputField3", outputColumn2.OutputHeader);
            var target2 = outputColumn2.TargetSetting as ConcatenateTargetSettingEntity;
            Assert.IsNotNull(target2);
            Assert.AreEqual(2, target2.OutputHeaderEntities.Count);
            Assert.AreEqual(1, target2.OutputHeaderEntities[0].HeaderId);
            Assert.AreEqual("OutputField2", target2.OutputHeaderEntities[0].HeaderName);
            Assert.AreEqual(0, target2.OutputHeaderEntities[1].HeaderId);
            Assert.AreEqual("OutputField1", target2.OutputHeaderEntities[1].HeaderName);
        }
    }
}
