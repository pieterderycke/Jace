using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __ANDROID__
using NUnit.Framework;
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

                Assert.Fail("An exception of type \"{0}\" was expected, but no exception was thrown.", typeof(T).FullName);
                return null;
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                Assert.Fail("An exception of type \"{0}\" was expected, but instead an exception of type \"{1}\" was thrown.",
                    typeof(T).FullName, ex.GetType().FullName);
                return null;
            }
        }
    }
}
