using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __ANDROID__
using NUnit.Framework;
#elif NETCORE
using Xunit;
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
#if !NETCORE
                Assert.Fail("An exception of type \"{0}\" was expected, but no exception was thrown.", typeof(T).FullName);
#endif
                return null;
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
#if !NETCORE
                Assert.Fail("An exception of type \"{0}\" was expected, but instead an exception of type \"{1}\" was thrown.",
                    typeof(T).FullName, ex.GetType().FullName);
#endif
                return null;
            }
        }
    }
}
