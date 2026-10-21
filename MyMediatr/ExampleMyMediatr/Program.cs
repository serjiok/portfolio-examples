using ExampleMyMediatr.Commands;
using Microsoft.Extensions.DependencyInjection;
using MyMediatr.Extensions;
using MyMediatr.Interfaces;
using System.Reflection;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddMyMediatr(Assembly.GetExecutingAssembly());

var serviceProvider = serviceCollection.BuildServiceProvider();

var mediator = serviceProvider.GetRequiredService<IMediator>();

await mediator.Publish(new NotifyMessage() { Message = "hello" });
await mediator.Send(new RequestCommand() { Value = 1 });
var result = await mediator.Send(new RequestResponseCommand() { Value = "test" });
Console.WriteLine(result);
Console.ReadKey();
