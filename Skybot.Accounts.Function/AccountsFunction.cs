using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Skybot.Accounts.Function
{
    public class AccountsFunction
    {
        private static readonly SkybotAccountsClient AccountsClient;
        private static readonly QueueClient QueueClient;

        static AccountsFunction()
        {
            AccountsClient = new SkybotAccountsClient();
            QueueClient = new QueueClient(Settings.BusConnectionString, Settings.IncomingRequestsQueue);
        }

        [FunctionName("SkybotAccountsFunc")]
        public static async void Run([ServiceBusTrigger("incomingtexts", Connection = "SkybotBusConnectionString")]string message, ILogger log)
        {
            log.LogInformation($"incomingtexts queue trigger function processed message: {message}");

            var textMessage = ConvertToTextMessage(message);
            RemoveInvalidCharacters(textMessage);

            if (!string.IsNullOrEmpty(textMessage.Message) && 
                await AccountsClient.HasAccount(textMessage.FromNumber))
            {
                log.LogInformation($"Phone number '{textMessage.FromNumber}' exists");
                PushToQueue(AccountsClient.GetAccount(textMessage.FromNumber), Settings.IncomingRequestsQueue);
            }

            log.LogInformation($"Phone number '{textMessage.FromNumber}' doesn't exist, creating account...");
            await AccountsClient.CreateAccount(textMessage.FromNumber);

            log.LogInformation($"Account for '{textMessage.FromNumber}' has been created");
            PushToQueue(new UserAccount
            {
                PhoneNumber = textMessage.FromNumber
            }, Settings.NewAccountsQueue);
        }

        private static void PushToQueue<T>(T item, string queueName)
        {
            QueueClient.SendAsync(new Message
            {
                To = queueName,
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item))
            });
        }

        private static TextMessage ConvertToTextMessage(string queueItem)
        {
            return JsonConvert.DeserializeObject<TextMessage>(queueItem);
        }

        private static void RemoveInvalidCharacters(TextMessage message)
        {
            if (message.FromNumber.Contains('+'))
            {
                message.FromNumber = message.FromNumber.Replace("+", string.Empty);
            }

            if (message.ToNumber.Contains('+'))
            {
                message.ToNumber = message.ToNumber.Replace("+", string.Empty);
            }
        }
    }
}
