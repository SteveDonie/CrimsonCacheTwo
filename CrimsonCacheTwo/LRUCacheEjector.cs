namespace CrimsonCacheTwo
{
	public class LRUCacheEjector : IEjectCacheEntries
	{
		private readonly Cache _cache;

		public LRUCacheEjector(Cache cache)
		{
			_cache = cache;
		}

		public CacheEntry GetEjectionVictim()
		{
			return _cache.Tail.Prev;
		}
	}
}