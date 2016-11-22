using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MemBus.Tests.Help
{
    public static class AssertExtensions
    {
        public static void ShouldHaveCount(this IEnumerable collection, int expectedCount)
        {
            var actual = collection.Cast<object>().Count();
            Assert.Equal(expectedCount, actual);
        }

        public static void ShouldHaveMoreItemsThan(this ICollection collection, int smallestCount)
        {
            Assert.True(collection.Count > smallestCount);
        }

        public static void ShouldBeGreaterThan<T>(this T target, T other) where T : IComparable<T>
        {
            Assert.True(target.CompareTo(other) > 0);
        }

        public static void ShouldBeLessThan<T>(this T target, T other) where T : IComparable<T>
        {
            Assert.True(target.CompareTo(other) < 0);
        }

        public static void ShouldHaveLength(this Array array, int expectedCount)
        {
            Assert.Equal(expectedCount,array.Length);
        }

        public static void ShouldBeEqualTo<T>(this T target, T expectedValue)
        {
            Assert.Equal(expectedValue, target);
        }

        public static void ShouldAllBeEqualTo<T>(this IEnumerable<T> target, T value)
        {
            Assert.Collection(target, item => item.Equals(value));
        }

        public static void ShouldBeASubsetOf<T>(this IEnumerable<T> target, IEnumerable<T> expectedSet)
        {
            Assert.ProperSubset(new HashSet<T>(expectedSet), new HashSet<T>(target));
        }

        public static void ShouldBeTheSameAs<T>(this T target, T expectedValue)
        {
            Assert.Same(expectedValue, target);
        }


        public static void ShouldBeTrue(this bool target)
        {
            Assert.True(target);
        }

        public static void ShouldBeTrue(this bool target, string message)
        {
            Assert.True(target, message);
        }

        public static void ShouldBeFalse(this bool target)
        {
            Assert.False(target);
        }

        public static void ShouldContain<T>(this IEnumerable<T> target, Func<T, bool> predicate)
        {
            Assert.Collection(target, item => Assert.True(predicate(item)));
        }

        public static void ShouldContain(this string target, string piece)
        {
            target.Contains(piece).ShouldBeTrue();
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> target, Func<T, bool> predicate)
        {
            Assert.False(target.Any(predicate), "The checked element does contain what is looked for");
        }

        public static void ShouldNotBeNull<T>(this T target) where T : class
        {
            Assert.NotNull(target);
        }

        public static void ShouldBeNull<T>(this T target) where T : class
        {
            Assert.Null(target);
        }

        public static void ShouldBeOfType<T>(this object target)
        {
            Assert.IsAssignableFrom<T>(target);
        }

        public static T Throws<T>(this Action action) where T : Exception
        {
            return Assert.Throws<T>(action);
        }
    }
}