using Furion;

using ITPortal.AuthServer.Core.AccountService;

using MapsterMapper;

using Xunit.Abstractions;

namespace ITPortal.xUnitTest
{
    //public class UnitTest1
    //{
    //    [Fact]
    //    public void Test1()
    //    {

    //    }
    //}
    public class UnitTest1
    {
        private readonly ITestOutputHelper Output;
        private readonly DbAccountService _accountService;
        /// <summary>
        /// 
        /// </summary>
        public UnitTest1(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
            var userService = App.GetService<IUserService>();
            _accountService = new DbAccountService( userService);//_passwordHasher
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


    }
}