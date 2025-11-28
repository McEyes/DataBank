using Xunit.Abstractions;
using Xunit.Sdk;
using Furion;


// �������������ͣ���һ�������� Startup �������޶������ڶ��������ǵ�ǰ��Ŀ��������
[assembly: TestFramework("DataAssetManager.xUnitTest.Startup", "DataAssetManager.xUnitTest")]
namespace DataAssetManager.xUnitTest
{
    public class Startup : XunitTestFramework
    {
        public Startup(IMessageSink messageSink) : base(messageSink)
        {
            // ��ʼ�� IServiceCollection ����
            Serve.Run(silence: true);
            //var services = Serve.Run(silence: true).Services;// Inject.Create();

            //// ��������Ժ� .NET Core һ��ע������ˣ���������������������������
            //services.AddRemoteRequest();

            //services.AddSingleton(services.BuildServiceProvider());

            //// ���� ServiceProvider ����
            //services.Build();
        }
    }
}