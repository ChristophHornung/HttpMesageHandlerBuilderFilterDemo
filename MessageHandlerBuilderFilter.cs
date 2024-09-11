using Microsoft.Extensions.Http;

namespace HandlerBuilderException;

/// <summary>
///     An <see cref="IHttpMessageHandlerBuilderFilter" /> which adds the correct DCL certificates.
/// </summary>
public class MessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
{
	private static bool isFirstCall;

	/// <inheritdoc />
	public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
	{
		return builder =>
		{
			if (builder.Name == "Test")
			{
				builder.PrimaryHandler = GetCustomClientHandler();
			}
			
			next(builder);
		};
	}

	private HttpMessageHandler GetCustomClientHandler()
	{
		Console.WriteLine("Getting Custom Handler");

		// Some kind of client handler is normally provided here.
		// To demonstrate the bug we throw on first try.
		if (!isFirstCall)
		{
			isFirstCall = true;
			Console.WriteLine("First call exception!");
			throw new Exception("This is a test exception.");
		}

		Console.WriteLine("No exception");

		HttpClientHandler handler = new();
		// Some kind of handler modifications are normally done here.
		return handler;
	}
}