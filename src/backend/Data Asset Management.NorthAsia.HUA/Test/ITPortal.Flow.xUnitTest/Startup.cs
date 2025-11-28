using Xunit.Abstractions;
using Xunit.Sdk;
using Furion;


[assembly: TestFramework("ITPortal.Flow.xUnitTest.Startup", "ITPortal.Flow.xUnitTest")]
namespace ITPortal.Flow.xUnitTest
{
    public class Startup : XunitTestFramework
    {
        public Startup(IMessageSink messageSink) : base(messageSink)
        {
            Serve.Run(silence: true);
        }
    }
}