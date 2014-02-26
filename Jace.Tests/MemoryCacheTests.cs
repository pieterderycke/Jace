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
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Jace.Tests
{
    [TestClass]
    public class MemoryCacheTests
    {
        [TestMethod]
        public void TestCacheCleanupOnlyAdd()
        {
            MemoryCache<string, int> cache = new MemoryCache<string, int>(3, 1);
            cache.GetOrAdd("test1", k => 1);
            cache.GetOrAdd("test2", k => 2);
            cache.GetOrAdd("test3", k => 3);
            cache.GetOrAdd("test4", k => 3);

            Assert.IsFalse(cache.ContainsKey("test1"));
            Assert.AreEqual(3, cache.Count);
        }

        [TestMethod]
        public void TestCacheCleanupRetrieve()
        {
            MemoryCache<string, int> cache = new MemoryCache<string, int>(3, 1);
            cache.GetOrAdd("test1", k => 1);
            cache.GetOrAdd("test2", k => 2);
            cache.GetOrAdd("test3", k => 3);
            
            Assert.AreEqual(1, cache["test1"]);
            
            cache.GetOrAdd("test4", k => 3);

            Assert.IsTrue(cache.ContainsKey("test1"));
            Assert.AreEqual(3, cache.Count);
        }

        [TestMethod]
        public void TestCacheCleanupBiggerThanCacheSize()
        {
            MemoryCache<string, int> cache = new MemoryCache<string, int>(1, 3);
            cache.GetOrAdd("test1", k => 1);
            cache.GetOrAdd("test2", k => 2);
            cache.GetOrAdd("test3", k => 3);

            Assert.IsTrue(cache.ContainsKey("test3"));
            Assert.AreEqual(1, cache.Count);
        }
    }
}
