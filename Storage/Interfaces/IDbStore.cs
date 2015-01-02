namespace SuperScript.ExternalFile.Storage
{
	public interface IDbStore : IStore
	{
		/// <summary>
		/// An instance of <see cref="IDbStoreProvider"/> which contains the instructions for initialising a database-based implementation of <see cref="IStore"/>.
		/// </summary>
		IDbStoreProvider DbStoreProvider { get; set; }
	}
}