using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application;

using Furion;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

using Xunit.Abstractions;
using Furion.DistributedIDGenerator;
using ITPortal.Core.Extensions;

namespace DataAssetManager.xUnitTest
{
    public class UnitTest1
    {
        private readonly IDataApiService _dataApiService;
        private readonly ITestOutputHelper Output;
        private readonly IDistributedIDGenerator _idGenerator;
        /// <summary>
        /// 
        /// </summary>
        public UnitTest1(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
            _dataApiService = App.GetService<IDataApiService>();
            _idGenerator = App.GetService<IDistributedIDGenerator>();
        }


        //[Theory]
        //[InlineData(1, 2)]
        //[InlineData(3, 4)]
        //[InlineData(5, 7)]
        [Fact]
        public void Test1()
        {
            Output.WriteLine(" Furion");
            Assert.NotEqual("Furion", "Fur");
        }

        [Fact]
        public async Task Test_Web()
        {
            using var testServer = new TestServer(WebHost.CreateDefaultBuilder()
                                               .Inject()
                                               .UseStartup<Startup>()); 

            using var httpClient = testServer.CreateClient();

            var result = await httpClient.GetStringAsync("/api/user/1");
            Assert.Equal("Furion", result);
        }


        //[Theory]
        //[InlineData(1, 2)]
        //[InlineData(3, 4)]
        //[InlineData(5, 7)]
        [Fact]
        public void TestIDGen()
        {
            for (var i = 0; i < 10; i++)
            {
                Output.WriteLine($" IDGen :\t\t\t\t\t{IDGen.NextID()}");
            }
            for (var i = 0; i < 10; i++)
            {
                Output.WriteLine($" _idGenerator :\t\t\t\t{_idGenerator.Create(new SequentialGuidSettings { LittleEndianBinary16Format = true })}");
            }
            for (var i = 0; i < 10; i++)
            {
                Output.WriteLine($" LittleEndianBinary16Format :{_idGenerator.Create(new SequentialGuidSettings { LittleEndianBinary16Format = false })}");
            }
        }
        [Fact]
        public void TestShortIDGen()
        {
            for (var i = 0; i < 10; i++)
            {
                Output.WriteLine($" ShortIDGen :{ShortIDGen.NextID(new GenerationOptions
                {
                    Length = 18,
                    UseNumbers = true,
                    UseSpecialCharacters = false
                })}");
            }
            // 自定义生成短 ID 参与运算字符
            string characters = "ⒶⒷⒸⒹⒺⒻⒼⒽⒾⒿⓀⓁⓂⓃⓄⓅⓆⓇⓈⓉⓊⓋⓌⓍⓎⓏⓐⓑⓒⓓⓔⓕⓖⓗⓘⓙⓚⓛⓜⓝⓞⓟⓠⓡⓢⓣⓤⓥⓦⓧⓨⓩ①②③④⑤⑥⑦⑧⑨⑩⑪⑫"; //whatever you want;
            ShortIDGen.SetCharacters(characters);
            for (var i = 0; i < 10; i++)
            {
                Output.WriteLine($" ShortIDGen :{ShortIDGen.NextID(new GenerationOptions
                {
                    Length = 18,
                    UseNumbers = true,
                    UseSpecialCharacters = false
                })}");
            }
        }
        [Fact]
        public void TestSnowflakeId()
        {  // 初始化雪花算法生成器，传入机器 ID 和数据中心 ID
            var generator = new SnowflakeIdGenerator(workerId: 2, datacenterId: 3);
            for (var i = 0; i < 20; i++)
            {    // 生成一个 ID
                var id = generator.NextId();
                Output.WriteLine($" ShortIDGen :{generator.NextId()}");
            }
        }
    }
}