using AutoFixture;
using Azure.Messaging.ServiceBus;

namespace ServiceBusEmulator.IntegrationTests
{
    [Collection(Consts.TopicCollection)]
    public class ServiceBusProcessorTest : Base
    {
        [Fact]
        public async Task ThatDisposeWorks()
        {
            var messageBody = Fixture.Create<string>();

            var serviceBusOptions = new ServiceBusProcessorOptions { AutoCompleteMessages = false, MaxConcurrentCalls = 1, PrefetchCount = 0 };
            var serviceBusProcessor = Client.CreateProcessor(Consts.TestSubsciption3Name, serviceBusOptions);

            serviceBusProcessor.ProcessMessageAsync += OnProcessMessageAsync;
            serviceBusProcessor.ProcessErrorAsync += OnProcessErrorAsync;

            await serviceBusProcessor.StartProcessingAsync();

            var sender = Client.CreateSender(Consts.TestTopicName);
            var receiver1 = Client.CreateReceiver(Consts.TestSubsciption1Name, new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            });
            var receiver2 = Client.CreateReceiver(Consts.TestSubsciption2Name, new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            });

            await sender.SendMessageAsync(new ServiceBusMessage(messageBody));

            await Task.Delay(500);

            var receivedMessage1 = await receiver1.ReceiveMessageAsync();
            var nextMessage1 = await receiver1.PeekMessageAsync();
            var receivedMessage2 = await receiver2.ReceiveMessageAsync();
            var nextMessage2 = await receiver2.PeekMessageAsync();

            Assert.Multiple(
                () => Assert.Equal(messageBody, receivedMessage1.Body.ToString()),
                () => Assert.Equal(messageBody, receivedMessage2.Body.ToString()),
                () => Assert.Null(nextMessage1),
                () => Assert.Null(nextMessage2)
            );

            await serviceBusProcessor.StopProcessingAsync();

            serviceBusProcessor.ProcessMessageAsync -= OnProcessMessageAsync;
            serviceBusProcessor.ProcessErrorAsync -= OnProcessErrorAsync;
            await serviceBusProcessor.DisposeAsync();
        }

        private async Task OnProcessMessageAsync(ProcessMessageEventArgs args)
        {
            await Task.Delay(100);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);            
        }

        private Task OnProcessErrorAsync(ProcessErrorEventArgs args)
        {
            await Task.Delay(100);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);            
        }    
    }
}
