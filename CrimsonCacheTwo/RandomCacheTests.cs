using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;

namespace CrimsonCacheTwo
{
	[TestFixture]
	public class RandomCacheTests
	{
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void YouHaveToInitializeTheEjectionStrategy()
		{
			var cache = new Cache(10);
			cache.Add("key1", "value1");
		}

		[Test]
		public void ShouldBeAbleToAddToANewCache()
		{
			var cache = new Cache(10);
			cache.Ejector = new RandomEjector(cache);

			const string key = "key1";
			const string value = "value1";

			cache.Add(key, value);
			cache.Exists(key).ShouldBeTrue();
		}

		[Test]
		public void AddingDuplicateKeysShouldReplaceValue()
		{
			var cache = new Cache(10);
			cache.Ejector = new RandomEjector(cache);

			const string key = "key1";
			var value = "value1";

			cache.Add(key, value);
			cache.Exists(key).ShouldBeTrue();
			cache.Get(key).ShouldEqual(value);

			value = "some new value";
			cache.Add(key, value);
			cache.Get(key).ShouldEqual(value);
		}

		// Testing behavior that is supposed to be 'random' is somewhat tricky. What you want to do is 
		// run a test many times, and ensure that the results can be categorized into different 'buckets'. 
		// After many runs, each of the buckets should contain approximately the same number of results. 
		// For this particular case, we run the test above (add 4 items to a cache of size 3) a large 
		// number of times. Each time we run the test, we check which item was removed. After many runs, 
		// we expect that each item (key1..key3) should have been removed from the cache 1/3 times, plus or
		// minus some expected variation. I'm not sure what variance to expect from the system random
		// number generator. Once we've implemented a system that doesn't always choose the same item, and 
		// at least chooses each item sometimes,, we start getting into the realm of testing the underlying 
		// psuedo-random number generator used. I am going to use the .NET provided random number generator, 
		// and I don't typically write tests of other people's code, so 20% variance seems reasonable.
		[Test]
		public void CheckHowRandomTheRemovalIs()
		{
			var testResults = new Dictionary<string, int> { { "key1", 0 }, { "key2", 0 }, { "key3", 0 } };

			const int numTests = 100000;
			const int oneThirdOfTotalTests = numTests / 3;
			const int variance = (int)(numTests * 0.20);

			const int lowValue = oneThirdOfTotalTests - variance;
			const int highValue = oneThirdOfTotalTests + variance;

			for (var i = 0; i < numTests; i++)
			{
				RunOneTest(testResults);
			}

			Console.WriteLine("key1 was removed " + testResults["key1"] + " times out of " + numTests + " tries.");
			Console.WriteLine("key2 was removed " + testResults["key2"] + " times out of " + numTests + " tries.");
			Console.WriteLine("key3 was removed " + testResults["key3"] + " times out of " + numTests + " tries.");

			testResults["key1"].ShouldBeInRange(lowValue, highValue);
			testResults["key2"].ShouldBeInRange(lowValue, highValue);
			testResults["key3"].ShouldBeInRange(lowValue, highValue);

		}

		private static void RunOneTest(Dictionary<string, int> testResults)
		{
			var cache = new Cache(3);
			cache.Ejector = new RandomEjector(cache);

			cache.Add("key1", "value1");
			cache.Add("key2", "value2");
			cache.Add("key3", "value3");
			// the fourth add should cause one of the first three items to be removed. Spec isn't clear 
			// whether removing a random item could include the item just added. I decided it should not.
			cache.Add("key4", "value4");

			UpdateTestResults(testResults, cache, "key1");
			UpdateTestResults(testResults, cache, "key2");
			UpdateTestResults(testResults, cache, "key3");
		}

		private static void UpdateTestResults(Dictionary<string, int> testResults, Cache cache, string key)
		{
			if (!cache.Exists(key))
			{
				testResults[key] += 1;
			}
		}

	}
}