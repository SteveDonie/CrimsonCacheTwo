using System;

namespace CrimsonCacheTwo
{
	public class RandomEjector : IEjectCacheEntries
	{
		private readonly Cache _cache;
		private Random _random;

		public RandomEjector(Cache cache)
		{
			_cache = cache;
			_random = new Random();
		}


		public CacheEntry GetEjectionVictim()
		{
			int randomNumber = _random.Next(_cache.Size);
			CacheEntry victim = _cache.Head.Next;
			for (int i = 0; i <= randomNumber; i++)
			{
				victim = victim.Next;
			}
			return victim;
		}
	}
}