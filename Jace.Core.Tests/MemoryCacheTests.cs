using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Util;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __ANDROID__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif NETCORE
using Xunit;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Jace.Tests
{
#if !NETCORE
    [TestClass]
#endif
    public class MemoryCacheTests
    {
#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCacheCleanupOnlyAdd()
        {
            MemoryCache<string, int> cache = new MemoryCache<string, int>(3, 1);
            cache.GetOrAdd("test1", k => 1);
            cache.GetOrAdd("test2", k => 2);
            cache.GetOrAdd("test3", k => 3);
            cache.GetOrAdd("test4", k => 3);
#if !NETCORE
            Assert.IsFalse(cache.ContainsKey("test1"));
            Assert.AreEqual(3, cache.Count);
#else
            Assert.False(cache.ContainsKey("test1"));
            Assert.Equal(3, cache.Count);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCacheCleanupRetrieve()
        {
            MemoryCache<string, int> cache = new MemoryCache<string, int>(3, 1);
            cache.GetOrAdd("test1", k => 1);
            cache.GetOrAdd("test2", k => 2);
            cache.GetOrAdd("test3", k => 3);

#if !NETCORE
            Assert.AreEqual(1, cache["test1"]);
#else
            Assert.Equal(1, cache["test1"]);
#endif

            cache.GetOrAdd("test4", k => 3);
#if !NETCORE
            Assert.IsTrue(cache.ContainsKey("test1"));
            Assert.AreEqual(3, cache.Count);
#else
            Assert.True(cache.ContainsKey("test1"));
            Assert.Equal(3, cache.Count);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCacheCleanupBiggerThanCacheSize()
        {
            MemoryCache<string, int> cache = new MemoryCache<string, int>(1, 3);
            cache.GetOrAdd("test1", k => 1);
            cache.GetOrAdd("test2", k => 2);
            cache.GetOrAdd("test3", k => 3);
#if !NETCORE
            Assert.IsTrue(cache.ContainsKey("test3"));
            Assert.AreEqual(1, cache.Count);
#else
            Assert.True(cache.ContainsKey("test3"));
            Assert.Equal(1, cache.Count);
#endif

        }
    }
}
