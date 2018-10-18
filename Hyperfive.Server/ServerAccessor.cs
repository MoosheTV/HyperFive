namespace Hyperfive.Server
{
	public class ServerAccessor
	{

		protected Server Server { get; }

		protected ServerAccessor( Server server ) {
			Server = server;
		}
	}
}
