namespace GenericCollections.Tests
{
    using System;
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

            Assert.That(set.Count, Is.Zero);
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
        public void Ctor_IEnumerableInitialization()
        {
            var data = Enumerable.Range(0, 100);
            var set = new Set<int>(data);

            CollectionAssert.AreEquivalent(set, data);
        }

        [Test]
        public void Ctor_CustomComparerWithICollectionInitialization()
        {
            var data = Enumerable.Range(-50, 100).ToArray();
            var comparer = new AbsoluteValueEqualityComparer();

            var set = new Set<int>(data, comparer);

            var expected = data.Where(v => v <= 0);

            CollectionAssert.AreEquivalent(expected, set);
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
            
            Assert.That(set.Contains(data.First()), Is.True);
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

            Assert.That(set.Remove(42), Is.False);
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

            Assert.That(set.Count, Is.Zero);
            CollectionAssert.AreEquivalent(Enumerable.Empty<int>(), set);
        }

        [Test]
        public void Clear_CountIsZeroAndEnumerationResultsInEmptyCollection()
        {
            var data = Enumerable.Range(0, 100).ToArray();
            var set = new Set<int>(data);

            set.Clear();

            Assert.That(set.Count, Is.Zero);
            CollectionAssert.AreEquivalent(Enumerable.Empty<int>(), set);
        }

        [Test]
        public void Clear_EmptySet_CountIsZeroAndEnumerationResultsInEmptyCollection()
        {
            var set = new Set<int>();

            set.Clear();

            Assert.That(set.Count, Is.Zero);
            CollectionAssert.AreEquivalent(Enumerable.Empty<int>(), set);
        }

        [Test]
        public void SetEquals_EquivalentInput_ResultTrue()
        {
            var data = Enumerable.Range(0, 100).ToList();
            var set = new Set<int>(data);

            Assert.That(set.SetEquals(data), Is.True);
        }

        [Test]
        public void SetEquals_NonequivalentInput_ResultTrue()
        {
            var data = Enumerable.Range(0, 100).ToList();
            var set = new Set<int>(data);

            data.AddRange(Enumerable.Range(0, 100));

            Assert.That(set.SetEquals(data), Is.True);
        }

        [Test]
        public void SetEquals_NonequivalentInput_ResultFalse()
        {
            var data = Enumerable.Range(0, 100).ToList();
            var set = new Set<int>(data);

            data.AddRange(Enumerable.Range(10, 100));

            Assert.That(set.SetEquals(data), Is.False);
        }

        [Test]
        public void SetEquals_PassedSelf_ResultTrue()
        {
            var data = Enumerable.Range(0, 100).ToList();
            var set = new Set<int>(data);
            Assert.That(set.SetEquals(set), Is.True);
        }

        [Test]
        public void SetEquals_EmptySet_PassedEmpty_ResultTrue()
        {
            var set = new Set<int>();
            Assert.That(set.SetEquals(Enumerable.Empty<int>()), Is.True);
        }

        [Test]
        public void SetEquals_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.SetEquals(null));
        }

        [Test]
        public void IntersectWith_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.IntersectWith(null));
        }

        [Test]
        public void IntersectWith_PassedSelf_ReturnsEquivalemtToSelf()
        {
            var set = new Set<int>();
            var setCopy = new Set<int>(set);

            set.IntersectWith(set);

            Assert.That(set, Is.EquivalentTo(setCopy));
        }

        [Test]
        public void IntersectWith_PassedEmpty_ReturnsEmpty()
        {
            var set = new Set<int>();

            set.IntersectWith(Enumerable.Empty<int>());

            Assert.That(set, Is.Empty);
        }

        [Test]
        public void IntersectWith_PassedValid_ReturnsValid()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(50, 100);

            var set1 = new Set<int>(data1);

            var expected = data1.Intersect(data2);

            set1.IntersectWith(data2);

            Assert.That(set1, Is.EquivalentTo(expected));
        }

        [Test]
        public void CopyTo_NullArray_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.CopyTo(null, 0));
        }

        [Test]
        public void CopyTo_ArrayIndexLessThanZero_ThrowsArgumentOutOfRangeExc()
        {
            var set = new Set<int>();
            var array = new int[0];
            Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(array, -1));
        }

        [Test]
        public void CopyTo_ArrayIndexLessThanZero_ThrowsArgumentExc()
        {
            var set = new Set<int>(Enumerable.Range(0, 10));
            var array = new int[5];
            Assert.Throws<ArgumentException>(() => set.CopyTo(array, 0));
        }

        [Test]
        public void CopyTo_EmptySet_ReturnsEmptyArray()
        {
            var set = new Set<int>();
            var array = new int[0];

            set.CopyTo(array, 0);

            Assert.That(array, Is.Empty);
        }

        [Test]
        public void CopyTo_ValidInput_ValidResult()
        {
            var data = Enumerable.Range(-50, 100);
            var set = new Set<int>(data);
            var array = new int[100];

            set.CopyTo(array, 0);

            Assert.That(array, Is.EquivalentTo(data));
        }
    }
}