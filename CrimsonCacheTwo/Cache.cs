using System;
using System.Collections.Generic;

namespace CrimsonCacheTwo
{
	/**
	 * The Cache uses a doubly-linked list to store a set of objects,
	 * keyed using objects. By putting all newly added items at the head
	 * of the list, and moving any recently accessed items to the head of 
	 * the list, the list is kept sorted. The doubly-linked list allows 
	 * for easy insertion and removal of items, and a paired Dictionary
	 * allows for quick retrieval of any item in the Cache. The size of
	 * the cache is set at construction time. 
	 * 
	 * The cache can use different ejection strategies. The strategies are
	 * somewhat tightly coupled to the cache in that they require intimate 
	 * knowledge of how the cache is implemented. 
	 */
	public class Cache
	{
		protected readonly int _size;
		protected readonly Dictionary<object, CacheEntry> _map;
		protected readonly CacheEntry _head;
		protected readonly CacheEntry _tail;
		private IEjectCacheEntries _ejector;

		public Cache(int size)
		{
			_size = size;
			_map = new Dictionary<object, CacheEntry>(size);
			_head = new CacheEntry(null, null);
			_tail = new CacheEntry(null, null);
			_head.Next = _tail;
			_tail.Prev = _head;
		}

		public IEjectCacheEntries Ejector
		{
			get {
				return _ejector;
			}
			set {
				_ejector = value;
			}
		}

		public CacheEntry Tail
		{
			get { return _tail; }
		}

		public int Size
		{
			get {
				return _size;
			}
		}

		public CacheEntry Head
		{
			get {
				return _head;
			}
		}


		public void Add(object key, object value)
		{
			if (_ejector == null)
			{
				throw new InvalidOperationException("You must set the Ejector strategy in order to use the cache.");
			}

			CacheEntry entry;
			if (_map.TryGetValue(key, out entry))
			{
				entry.Remove();
				entry.Insert(_head);
				entry.Value = value;
			}
			else
			{
				entry = new CacheEntry(key, value);
				_map.Add(key, entry);
				entry.Insert(_head);
				if (_map.Count > _size)
				{
					CacheEntry victim = _ejector.GetEjectionVictim();
					_map.Remove(victim.Key);
					victim.Remove();
				}
			}
		}


		public bool Exists(string key)
		{
			return (Get(key) != null);
		}

		public object Get(string key)
		{
			CacheEntry entry;
			if (_map.TryGetValue(key, out entry))
			{
				entry.Remove();
				entry.Insert(_head);
				return entry.Value;
			}
			return null;
		}
	}
}
