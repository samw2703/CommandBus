namespace CommandBus
{
	internal class Config
	{
		public bool PublishSynchronously { get; }

		public Config(bool publishSynchronously)
		{
			PublishSynchronously = publishSynchronously;
		}
	}
}