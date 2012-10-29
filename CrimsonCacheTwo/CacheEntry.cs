using System;

namespace CrimsonCacheTwo
{
	public class CacheEntry
	{
		public CacheEntry(object key, object value)
		{
			Key = key;
			Value = value;
		}

		public CacheEntry Next { get; set; }
		public CacheEntry Prev { get; set; }
		public Object Key { get; set; }
		public Object Value { get; set; }

		public void Remove()
		{
			Prev.Next = Next;
			Next.Prev = Prev;
		}

		public void Insert(CacheEntry head)
		{
			Prev = head;
			Next = head.Next;
			Next.Prev = this;
			Prev.Next = this;
		}
	}
}