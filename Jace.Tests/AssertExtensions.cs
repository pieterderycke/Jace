using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Jace.Tests
{
    public static class AssertExtensions
    {
        public static T ThrowsException<T>(Action action) where T : Exception
        {
            try
            {
                action();

                Assert.Fail("An exception of type \"{0}\" was expected but no exception was thrown.", typeof(T).Name);
                return null;
            }
            catch (T ex)
            {
                return ex;
            }
        }
    }
}
