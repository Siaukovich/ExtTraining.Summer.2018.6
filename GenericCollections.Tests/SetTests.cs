namespace GenericCollections.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using NUnit.Framework;

    /// <summary>
    /// The set tests.
    /// </summary>
    [TestFixture]
    [SuppressMessage("ReSharper", "StyleCop.SA1600", Justification = "No need for codestyle in tests.")]
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

        [Test]
        public void Enumeration_CollectionChangedDuringEnumeration_ThrowsInvalidOperationExc()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            Assert.Throws<InvalidOperationException>(
                () =>
                    {
                        foreach (int element in set)
                        {
                            set.Add(11);
                        }
                    });
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

        [Test]
        public void StaticUnion_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => Set<int>.Union(set, null));
            Assert.Throws<ArgumentNullException>(() => Set<int>.Union(null, set));
        }

        [Test]
        public void StaticUnion_PassedEmpty_ReturnsEquivalent()
        {
            var data = Enumerable.Range(0, 10);
            var set1 = new Set<int>(data);
            var set2 = new Set<int>();

            var unionSet = Set<int>.Union(set1, set2);

            Assert.That(unionSet, Is.EquivalentTo(data));
        }

        [Test]
        public void StaticUnion_EmptySet_ReturnsEquivalent()
        {
            var data = Enumerable.Range(0, 10);
            var set1 = new Set<int>();
            var set2 = new Set<int>(data);

            var unionSet = Set<int>.Union(set1, set2);

            Assert.That(unionSet, Is.EquivalentTo(data));
        }

        [Test]
        public void StaticUnion_ValidData_ReturnsValidUnion()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(50, 100);

            var set1 = new Set<int>(data1);
            var set2 = new Set<int>(data2);

            var unionSet = Set<int>.Union(set1, set2);

            var expected = set1.Union(set2); // Linq.

            Assert.That(unionSet, Is.EquivalentTo(expected));
        }

        [Test]
        public void UnionWith_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.UnionWith(null));
        }

        [Test]
        public void UnionWith_PassedEmpty_ReturnsEquivalent()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            set.UnionWith(Enumerable.Empty<int>());

            Assert.That(set, Is.EquivalentTo(data));
        }

        [Test]
        public void UnionWith_EmptySet_ReturnsEquivalent()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>();

            set.UnionWith(data);

            Assert.That(set, Is.EquivalentTo(data));
        }

        [Test]
        public void UnionWith_ValidData_ReturnsValidUnion()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(50, 100);

            var set = new Set<int>(data1);

            var expected = data1.Union(data2);

            set.UnionWith(data2);

            Assert.That(set, Is.EquivalentTo(expected));
        }

        [Test]
        public void SymmetricExceptWith_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.SymmetricExceptWith(null));
        }

        [Test]
        public void SymmetricExceptWith_PassedEmpty_ReturnsEquivalent()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            set.SymmetricExceptWith(Enumerable.Empty<int>());

            Assert.That(set, Is.EquivalentTo(data));
        }

        [Test]
        public void SymmetricExceptWith_EmptySet_ReturnsEquivalent()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>();

            set.SymmetricExceptWith(data);

            Assert.That(set, Is.EquivalentTo(data));
        }

        [Test]
        public void SymmetricExceptWith_ValidInput_ReturnsValidSymmetricExcept()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(50, 100);

            var set = new Set<int>(data1);

            var expected = data1.Except(data2).Union(data2.Except(data1));

            set.SymmetricExceptWith(data2);

            Assert.That(set, Is.EquivalentTo(expected));
        }

        [Test]
        public void ExceptWith_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.ExceptWith(null));
        }

        [Test]
        public void ExceptWith_PassedEmpty_ReturnsEquivalent()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            set.ExceptWith(Enumerable.Empty<int>());

            Assert.That(set, Is.EquivalentTo(data));
        }

        [Test]
        public void ExceptWith_EmptySet_ReturnsEmpty()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>();

            set.ExceptWith(data);

            Assert.That(set, Is.Empty);
        }

        [Test]
        public void ExceptWith_ValidInput_ReturnsValidExcept()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(50, 100);

            var set = new Set<int>(data1);

            var expected = data1.Except(data2);

            set.ExceptWith(data2);

            Assert.That(set, Is.EquivalentTo(expected));
        }

        [Test]
        public void StaticExcept_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => Set<int>.Except(set, null));
            Assert.Throws<ArgumentNullException>(() => Set<int>.Except(null, set));
        }

        [Test]
        public void StaticExcept_PassedEmpty_ReturnsEquivalent()
        {
            var data = Enumerable.Range(0, 10);

            var set1 = new Set<int>(data);
            var set2 = new Set<int>();

            var exceptSet = Set<int>.Except(set1, set2);

            Assert.That(exceptSet, Is.EquivalentTo(data));
        }

        [Test]
        public void StaticExcept_EmptySet_ReturnsEmpty()
        {
            var data = Enumerable.Range(0, 10);

            var set1 = new Set<int>();
            var set2 = new Set<int>(data);

            var exceptSet = Set<int>.Except(set1, set2);

            Assert.That(exceptSet, Is.Empty);
        }

        [Test]
        public void StaticExcept_ValidInput_ReturnsValidExcept()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(50, 100);

            var set1 = new Set<int>(data1);
            var set2 = new Set<int>(data2);

            var expected = data1.Except(data2);

            var exceptSet = Set<int>.Except(set1, set2);

            Assert.That(exceptSet, Is.EquivalentTo(expected));
        }

        [Test]
        public void Overlaps_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.Overlaps(null));
        }

        [Test]
        public void Overlaps_PassedEmpty_ReturnsFalse()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            var result = set.Overlaps(Enumerable.Empty<int>());

            Assert.That(result, Is.False);
        }

        [Test]
        public void Overlaps_EmptySet_ReturnsFalse()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>();

            var result = set.Overlaps(data);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Overlaps_ValidInput_ReturnsTrue()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(50, 100);

            var set = new Set<int>(data1);

            var result = set.Overlaps(data2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Overlaps_ValidInput_ReturnsFalse()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(101, 100);

            var set = new Set<int>(data1);

            var result = set.Overlaps(data2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsSubsetOf_EquivalentSets_ReturnsFalse()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(0, 100);

            var set = new Set<int>(data1);

            var result = set.IsProperSubsetOf(data2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsSupersetOf_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.IsSupersetOf(null));
        }

        [Test]
        public void IsSupersetOf_EmptySetPassedEmpty_ReturnsTrue()
        {
            var set = new Set<int>();

            var result = set.IsSupersetOf(Enumerable.Empty<int>());

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSupersetOf_EmptySetPassedNonempty_ReturnsFalse()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>();

            var result = set.IsSupersetOf(data);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsSupersetOf_NonemptySetPassedEmpty_ReturnsTrue()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            var result = set.IsSupersetOf(Enumerable.Empty<int>());

            Assert.That(result, Is.True);
        }


        [Test]
        public void IsSupersetOf_ValidInput_ReturnsTrue()
        {
            var data1 = Enumerable.Range(-30, 100); 
            var data2 = Enumerable.Range(0, 50);

            var set = new Set<int>(data1);

            var result = set.IsSupersetOf(data2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSupersetOf_PassedSelf_ReturnsTrue()
        {
            var data1 = Enumerable.Range(0, 50);

            var set = new Set<int>(data1);

            var result = set.IsSubsetOf(set);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSupersetOf_ValidInput_ReturnsFalse()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(1, 100);

            var set = new Set<int>(data1);

            var result = set.IsSubsetOf(data2);

            Assert.That(result, Is.False);
        }
    
        [Test]
        public void IsSubsetOf_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.IsSubsetOf(null));
        }

        [Test]
        public void IsSubsetOf_EmptySetPassedEmpty_ReturnsTrue()
        {
            var set = new Set<int>();

            var result = set.IsSubsetOf(Enumerable.Empty<int>());

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSubsetOf_EmptySetPassedNonempty_ReturnsTrue()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>();

            var result = set.IsSubsetOf(data);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSubsetOf_NonemptySetPassedEmpty_ReturnsFalse()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            var result = set.IsSubsetOf(Enumerable.Empty<int>());

            Assert.That(result, Is.False);
        }


        [Test]
        public void IsSubsetOf_ValidInput_ReturnsTrue()
        {
            var data1 = Enumerable.Range(0, 50);
            var data2 = Enumerable.Range(-30, 100);

            var set = new Set<int>(data1);

            var result = set.IsSubsetOf(data2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSubsetOf_PassedSelf_ReturnsTrue()
        {
            var data1 = Enumerable.Range(0, 50);

            var set = new Set<int>(data1);

            var result = set.IsSubsetOf(set);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSubsetOf_ValidInput_ReturnsFalse()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(1, 100);

            var set = new Set<int>(data1);

            var result = set.IsSubsetOf(data2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsProperSubsetOf_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.IsProperSubsetOf(null));
        }

        [Test]
        public void IsProperSubsetOf_EmptySetPassedEmpty_ReturnsFalse()
        {
            var set = new Set<int>();

            var result = set.IsProperSubsetOf(Enumerable.Empty<int>());

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsProperSubsetOf_EmptySetPassedNonempty_ReturnsTrue()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>();

            var result = set.IsProperSubsetOf(data);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsProperSubsetOf_NonemptySetPassedEmpty_ReturnsFalse()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            var result = set.IsProperSubsetOf(Enumerable.Empty<int>());

            Assert.That(result, Is.False);
        }


        [Test]
        public void IsProperSubsetOf_ValidInput_ReturnsTrue()
        {
            var data1 = Enumerable.Range(0, 50);
            var data2 = Enumerable.Range(-30, 100);

            var set = new Set<int>(data1);

            var result = set.IsProperSubsetOf(data2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsProperSubsetOf_PassedSelf_ReturnsFalse()
        {
            var data1 = Enumerable.Range(0, 50);

            var set = new Set<int>(data1);

            var result = set.IsProperSubsetOf(set);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsProperSupersetOf_PassedNull_ThrowsArgumentNullExc()
        {
            var set = new Set<int>();
            Assert.Throws<ArgumentNullException>(() => set.IsProperSupersetOf(null));
        }

        [Test]
        public void IsProperSupersetOf_EmptySetPassedEmpty_ReturnsFalse()
        {
            var set = new Set<int>();

            var result = set.IsProperSupersetOf(Enumerable.Empty<int>());

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsProperSupersetOf_EmptySetPassedNonempty_ReturnsFalse()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>();

            var result = set.IsProperSupersetOf(data);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsProperSupersetOf_NonemptySetPassedEmpty_ReturnsTrue()
        {
            var data = Enumerable.Range(0, 10);
            var set = new Set<int>(data);

            var result = set.IsSupersetOf(Enumerable.Empty<int>());

            Assert.That(result, Is.True);
        }


        [Test]
        public void IsProperSupersetOf_ValidInput_ReturnsTrue()
        {
            var data1 = Enumerable.Range(-30, 100);
            var data2 = Enumerable.Range(0, 50);

            var set = new Set<int>(data1);

            var result = set.IsProperSupersetOf(data2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSupersetOf_PassedSelf_ReturnsFalse()
        {
            var data1 = Enumerable.Range(0, 50);

            var set = new Set<int>(data1);

            var result = set.IsProperSupersetOf(set);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsProperSupersetOf_ValidInput_ReturnsFalse()
        {
            var data1 = Enumerable.Range(0, 100);
            var data2 = Enumerable.Range(0, 100);

            var set = new Set<int>(data1);

            var result = set.IsProperSupersetOf(data2);

            Assert.That(result, Is.False);
        }
    }
}