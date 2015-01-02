using SuperScript.ExternalFile.Storage;

namespace SuperScript.ExternalFile.UrlHelpers
{
	public static class QueryStringKeys
	{
		/// <summary>
		/// Deletes a specified item.
		/// </summary>
		public const string Delete = "delete";

		/// <summary>
		/// Empties the store of all items.
		/// </summary>
		public const string Empty = "empty";

		/// <summary>
		/// Initialises the store (calls <see cref="IStore.Init"/>).
		/// </summary>
		public const string Init = "init";

		/// <summary>
		/// Removes the store, then initialises it (calls <see cref="IStore.Remove"/> then <see cref="IStore.Init"/>).
		/// </summary>
		public const string ReInit = "reinit";

		/// <summary>
		/// Completely removes the store from the system.
		/// </summary>
		public const string Remove = "remove";

		/// <summary>
		/// The query-string key for a unique value, used for making the URI unique.
		/// </summary>
		public const string UniqueKey = "h";
	}
}