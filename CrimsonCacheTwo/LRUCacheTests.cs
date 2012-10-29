using NUnit.Framework;
using Should;

namespace CrimsonCacheTwo
{
	[TestFixture]
	public class LRUCacheTests
	{
		[Test]
		public void Should_be_able_to_add_to_a_new_cache()
		{
			var cache = new Cache(10);
			cache.Ejector = new LRUCacheEjector(cache);

			string key = "key1";
			string value = "value1";

			cache.Add(key, value);
			cache.Exists(key).ShouldBeTrue();
		}

		[Test]
		public void adding_duplicate_keys_should_replace_value()
		{
			var cache = new Cache(10);
			cache.Ejector = new LRUCacheEjector(cache);

			string key = "key1";
			string value = "value1";

			cache.Add(key, value);
			cache.Exists(key).ShouldBeTrue();
			cache.Get(key).ShouldEqual(value);

			value = "some new value";
			cache.Add(key,value);
			cache.Get(key).ShouldEqual(value);
		}

		[Test]
		public void adding_more_items_than_size_should_remove_oldest_item()
		{
			var cache = new Cache(3);
			cache.Ejector = new LRUCacheEjector(cache);

			cache.Add("key1", "value1");
			cache.Add("key2", "value2");
			cache.Add("key3", "value3");

			cache.Add("key4","value4");

			cache.Exists("key1").ShouldBeFalse("key1 should not be in cache");
			cache.Exists("key2").ShouldBeTrue("key2 should be in cache");
			cache.Exists("key3").ShouldBeTrue("key3 should be in cache");
			cache.Exists("key4").ShouldBeTrue("key4 should be in cache");
			
		}

		[Test]
		public void get_should_reset_timestamp()
		{
			var cache = new Cache(3);
			cache.Ejector = new LRUCacheEjector(cache);

			cache.Add("key1", "value1");
			cache.Add("key2", "value2");
			cache.Add("key3", "value3");

			// if we call Get using key1, it becomes youngest
			cache.Get("key1");

			// now when we add one more, the oldest should be removed. Oldest is now key2
			cache.Add("key4", "value4");

			cache.Exists("key1").ShouldBeTrue("key1 should be in cache");
			cache.Exists("key2").ShouldBeFalse("key2 should NOT be in cache");
			cache.Exists("key3").ShouldBeTrue("key3 should be in cache");
			cache.Exists("key4").ShouldBeTrue("key4 should be in cache");
		}

		[Test]
		public void exists_should_reset_timestamp()
		{
			var cache = new Cache(3);
			cache.Ejector = new LRUCacheEjector(cache);

			cache.Add("key1", "value1");
			cache.Add("key2", "value2");
			cache.Add("key3", "value3");

			// if we check whether key1 Exists, it becomes youngest
			cache.Exists("key1");

			// now when we add one more, the oldest (which is now key2) should be removed. 
			cache.Add("key4", "value4");

			cache.Exists("key1").ShouldBeTrue("key1 should be in cache");
			cache.Exists("key2").ShouldBeFalse("key2 should NOT be in cache");
			cache.Exists("key3").ShouldBeTrue("key3 should be in cache");
			cache.Exists("key4").ShouldBeTrue("key4 should be in cache");
		}

	}
}