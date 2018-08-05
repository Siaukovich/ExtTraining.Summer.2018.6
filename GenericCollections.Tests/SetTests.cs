namespace GenericCollections.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class SetTests
    {
        [Test]
        public void Ctor_NoParameters_EmptySet()
        {
            var set = new Set<int>();

            Assert.That(set.Count, Is.EqualTo(0));
            CollectionAssert.AreEquivalent(Enumerable.Empty<int>(), set);
        }

        [Test]
        public void Ctor_CustomComparer()
        {
            var comparer = new AbsoluteValueEqualityComparer();
            var set = new Set<int>(comparer);

            set.Add(1);
            set.Add(-1);
            
            Assert.That(set.Count, Is.EqualTo(1));
        }

        [Test]
        public void Ctor_ICollectionInitialization()
        {
            var data = Enumerable.Range(0, 100).ToArray();
            var set = new Set<int>(data);

            CollectionAssert.AreEquivalent(set, data);
        }

        [Test]
        public void Ctor_CustomComparerWithICollectionInitialization()
        {
            var data = Enumerable.Range(-50, 100).ToArray();
            var comparer = new AbsoluteValueEqualityComparer();

            var set = new Set<int>(data, comparer);

            CollectionAssert.AreEquivalent(set, data);
        }

        [Test]
        public void Add_ValidInput_ValidResult()
        {
            const int SET_SIZE = 10;
            var set = new Set<int>(SET_SIZE);
            var data = new[] {1, 2, 3, SET_SIZE, 2*SET_SIZE};

            foreach (var i in data)
            {
                set.Add(i);
            }
            
            CollectionAssert.AreEquivalent(data, set);
        }
        
        [Test]
        public void AddWithResizing_ValidInput_ValidResult()
        {
            const int SET_SIZE = 3;
            var set = new Set<int>(SET_SIZE);
            var data = Enumerable.Range(0, 100);

            foreach (var i in data)
            {
                set.Add(i);
            }
            
            CollectionAssert.AreEquivalent(data, set);
        }
        
        [Test]
        public void AddWithResizingWithRepeatingElements_ValidInput_ValidResult()
        {
            const int SET_SIZE = 3;
            var set = new Set<int>(SET_SIZE);
            var data = Enumerable.Range(0, 100);

            foreach (var i in data)
            {
                set.Add(i);
            }
            
            foreach (var i in data)
            {
                set.Add(i);
            }
            
            CollectionAssert.AreEquivalent(data, set);
        }
        
        [Test]
        public void ContainsWithResizingWithRepeatingElements_ValidInput_ValidResult()
        {
            const int SET_SIZE = 3;
            var set = new Set<int>(SET_SIZE);
            var data = Enumerable.Range(0, 100);

            foreach (var i in data)
            {
                set.Add(i);
            }
            
            foreach (var i in data)
            {
                set.Add(i);
            }
            
            Assert.That(set.Contains(data.First()), Is.EqualTo(true));
        }

        [TestCase(1, ExpectedResult = false)]
        [TestCase(2, ExpectedResult = true)]
        [TestCase(3, ExpectedResult = true)]
        [TestCase(11, ExpectedResult = true)]
        [TestCase(12, ExpectedResult = false)]
        [TestCase(21, ExpectedResult = false)]
        [TestCase(1201, ExpectedResult = true)]
        [TestCase(104729, ExpectedResult = true)]
        [TestCase(104730, ExpectedResult = false)]
        public bool IsPrime_ValidInput_ValidResult(int number)
        {
            var set = new Set<int>();
            MethodInfo isPrime = set.GetType().GetMethod("IsPrime", BindingFlags.NonPublic | BindingFlags.Static);
            return (bool)isPrime.Invoke(set, new object[] { number });
        }

        [TestCase(1, ExpectedResult = true)]
        [TestCase(-1, ExpectedResult = false)]
        [TestCase(10, ExpectedResult = true)]
        [TestCase(99, ExpectedResult = true)]
        public bool Remove_ValidInput_ValidResult(int removeValue)
        {
            var data = Enumerable.Range(0, 100).ToArray();
            var set = new Set<int>(data);

            return set.Remove(removeValue);
        }

        [Test]
        public void RemoveFromEmptySet_ReturnsFalse()
        {
            var set = new Set<int>();

            Assert.That(set.Remove(42), Is.EqualTo(false));
        }

        [Test]
        public void RemoveAllElementsFromSet_CountIsZeroAndEnumerationResultsInEmptyCollection()
        {
            var data = Enumerable.Range(0, 100).ToArray();
            var set = new Set<int>(data);

            foreach (int value in data)
            {
                if (!set.Remove(value))
                {
                    Assert.Fail($"Element {value} was not in set.");
                }
            }

            Assert.That(set.Count, Is.EqualTo(0));
            CollectionAssert.AreEquivalent(Enumerable.Empty<int>(), set);
        }

        [Test]
        public void Clear_CountIsZeroAndEnumerationResultsInEmptyCollection()
        {
            var data = Enumerable.Range(0, 100).ToArray();
            var set = new Set<int>(data);

            set.Clear();

            Assert.That(set.Count, Is.EqualTo(0));
            CollectionAssert.AreEquivalent(Enumerable.Empty<int>(), set);
        }

        [Test]
        public void Clear_EmptySet_CountIsZeroAndEnumerationResultsInEmptyCollection()
        {
            var set = new Set<int>();

            set.Clear();

            Assert.That(set.Count, Is.EqualTo(0));
            CollectionAssert.AreEquivalent(Enumerable.Empty<int>(), set);
        }
    }
}