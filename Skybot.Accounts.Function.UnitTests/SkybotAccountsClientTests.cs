using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Skybot.Accounts.Function.UnitTests
{
    [TestClass]
    public class SkybotAccountsClientTests
    {
        private const string PhoneNumber = "001112223333";

        [TestMethod]
        public async void HasAccount_Returns_TrueWhenPhoneNumberExists()
        {
            var client = new SkybotAccountsClient();

            var result = await client.HasAccount(PhoneNumber);

            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }
    }
}
