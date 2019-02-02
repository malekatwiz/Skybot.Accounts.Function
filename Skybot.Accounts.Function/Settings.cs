using System;

namespace Skybot.Accounts.Function
{
    public static class Settings
    {
        public static string IncomingRequestsQueue => GetEnvironmentVariable("IncomingRequestsQueue");
        public static string NewAccountsQueue => GetEnvironmentVariable("NewAccountsQueue");
        public static string SkybotAccountsApiUri => GetEnvironmentVariable("SkybotAccountsApiUri");
        public static string SkybotAuthUri => GetEnvironmentVariable("SkybotAuthUri");
        public static string SkybotAuthClientId => GetEnvironmentVariable("SkybotAuthClientId");
        public static string SkybotAuthClientSecret => GetEnvironmentVariable("SkybotAuthClientSecret");
        public static string BusConnectionString => GetEnvironmentVariable("SkybotBusConnectionString");

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
