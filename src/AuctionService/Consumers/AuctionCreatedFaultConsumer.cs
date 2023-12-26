using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("--> Consuming Fault: " + context.Message.Message);

        var exception = context.Message.Exceptions.First();

        if (exception.ExceptionType == "System.ArgumentException")
        {
            Console.WriteLine("----> Overwriting Foo Model");
            context.Message.Message.Model = "FooBar";
            await context.Publish(context.Message.Message);
        }
        else
        {
            Console.WriteLine("----> Not an argument exception - update error dashboard somewhere");
        }
    }
}