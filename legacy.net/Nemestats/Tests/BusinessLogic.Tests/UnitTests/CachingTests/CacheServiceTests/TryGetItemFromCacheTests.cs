﻿using System;
using System.Linq;
using System.Runtime.Caching;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.CachingTests.CacheServiceTests
{
    [TestFixture]
    public class TryGetItemFromCacheTests : CacheServiceTestBase
    {
        [Test]
        public void ItReturnsFalseIfTheItemIsNotInTheCache()
        {
            //--arrange
            object itemInCache;

            //--act
            var isInCache = _autoMocker.ClassUnderTest.TryGetItemFromCache("some key that is not in the cache", out itemInCache);

            //--assert
            Assert.False(isInCache);
        }

        [Test]
        public void ItReturnsTrueAndSetsTheOutputParameterIfTheItemIsInTheCache()
        {
            //--arrange
            var itemInCache = new object();
            string cacheKey = "some cache key";
            var cacheItem = new CacheItem(cacheKey, itemInCache);

            var cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = new DateTimeOffset(
                    DateTime.UtcNow.AddSeconds(9999))
            };

            MemoryCache.Default.Add(cacheItem, cachePolicy);
            object outputItem;

            //--act
            var isInCache = _autoMocker.ClassUnderTest.TryGetItemFromCache(cacheKey, out outputItem);

            //--assert
            Assert.True(isInCache);
            Assert.That(outputItem, Is.SameAs(itemInCache));
        }
    }
}
