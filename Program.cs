using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;

namespace HandlerBuilderException;

internal class Program
{
	private static async Task Main(string[] args)
	{
		await Host.CreateDefaultBuilder(args)
			.ConfigureServices((hostContext, services) =>
			{
				services.AddHttpClient();
				services.AddSingleton<IHttpMessageHandlerBuilderFilter, MessageHandlerBuilderFilter>();
				services.AddHostedService<TestBackgroundCaller>();
			})
			.RunConsoleAsync();
	}
}

internal class TestBackgroundCaller : IHostedService
{
	public TestBackgroundCaller(IHttpClientFactory clientFactory)
	{
		ClientFactory = clientFactory;
	}

	public IHttpClientFactory ClientFactory { get; set; }

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		while (true)
		{
			Console.WriteLine("Calling HttpClient");
			
			try
			{
				using HttpClient client = ClientFactory.CreateClient("Test");
				await client.GetAsync("https://www.google.com", cancellationToken);
				Console.WriteLine("Called HttpClient");
			}
			catch (Exception e)
			{
				// Just ignore
				Console.WriteLine("Exception HttpClient");
			}

			await Task.Delay(1000, cancellationToken);
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}