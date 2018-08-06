namespace GenericCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Set class.
    /// </summary>
    /// <typeparam name="T">
    /// Type of set objects.
    /// </typeparam>
    public class Set<T> : ISet<T>
    {
        #region Private Fields

        /// <summary>
        /// Comparer that compares two elements.
        /// </summary>
        private readonly IEqualityComparer<T> equalityComparer;

        /// <summary>
        /// Array of buckets.
        /// </summary>
        private Node[] buckets;

        /// <summary>
        /// Amount of filled buckets.
        /// </summary>
        private int amountOfFilledBuckets;

        /// <summary>
        /// Version of this set. For enumeration.
        /// </summary>
        private int version;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="defaultCapacity">
        /// The default set's capacity.
        /// </param>
        public Set(int defaultCapacity = 5)
        {
            this.buckets = new Node[defaultCapacity];
            this.equalityComparer = EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="comparer">
        /// Comparer that tests set elements for equality.
        /// </param>
        /// <param name="defaultCapacity">
        /// The default capacity.
        /// </param>
        public Set(IEqualityComparer<T> comparer, int defaultCapacity = 5)
        {
            this.equalityComparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            this.buckets = new Node[defaultCapacity];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="collection">
        /// Collection which elements will be added to this set.
        /// </param>
        public Set(ICollection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            this.equalityComparer = EqualityComparer<T>.Default;
            
            this.buckets = new Node[GetNextPrimeNumber(collection.Count)];

            foreach (var element in collection)
            {
                this.Add(element);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="collection">
        /// Collection which elements will be added to this set.
        /// </param>
        /// <param name="comparer">
        /// Comparer that tests set elements for equality.
        /// </param>
        public Set(ICollection<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            
            this.equalityComparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            
            this.buckets = new Node[GetNextPrimeNumber(collection.Count)];

            foreach (var element in collection)
            {
                this.Add(element);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="collection">
        /// Collection which elements will be added to this set.
        /// </param>
        public Set(IEnumerable<T> collection) : this(collection.ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="collection">
        /// Collection which elements will be added to this set.
        /// </param>
        /// <param name="comparer">
        /// Comparer that tests set elements for equality.
        /// </param>
        public Set(IEnumerable<T> collection, IEqualityComparer<T> comparer) :
              this(collection.ToList(), comparer)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the amount of elements in this set.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Indicates if this collection is readonly.
        /// </summary>
        public bool IsReadOnly => false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets this set's enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if set was changed during the enumeration.
        /// </exception>
        public IEnumerator<T> GetEnumerator()
        {
            int startVersion = this.version;
            foreach (Node bucket in this.buckets)
            {
                Node currentNode = bucket;
                while (currentNode != null)
                {
                    if (startVersion != this.version)
                    {
                        throw new InvalidOperationException("Set was modified.");
                    }
                    
                    yield return currentNode.Value;
                    currentNode = currentNode.Next;
                }
            }
        }

        /// <summary>
        /// Gets this set's enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if set was changed during the enumeration.
        /// </exception>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Adds item to this set.
        /// </summary>
        /// <param name="item">
        /// Item to add.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// True if item was added to set, false if not.
        /// </returns>
        public bool Add(T item)
        {
            int bucketIndex = this.GetBucketIndex(item);

            if (this.buckets[bucketIndex] == null)
            {
                this.CreateNewBucket(item, bucketIndex);
            }
            else
            {
                Node currentNode = this.buckets[bucketIndex];
                Node parentNode = this.buckets[bucketIndex];
                while (currentNode != null)
                {
                    if (this.equalityComparer.Equals(currentNode.Value, item))
                    {
                        return false;
                    }

                    parentNode = currentNode;
                    currentNode = currentNode.Next;
                }

                parentNode.Next = new Node(item);
            }

            this.UpdateVersion();
            this.Count++;

            return true;
        }

        /// <summary>
        /// Adds item to this set.
        /// </summary>
        /// <param name="item">
        /// Item to add.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// True if item was added to set, false if not.
        /// </returns>
        bool ISet<T>.Add(T item) => this.Add(item);

        /// <summary>
        /// Adds item to this set.
        /// </summary>
        /// <param name="item">
        /// Item to add.
        /// </param>
        void ICollection<T>.Add(T item) => this.Add(item);

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if other was null.
        /// </exception>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (T element in other)
            {
                this.Remove(element);
            }
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">
        /// The other collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if other was null.
        /// </exception>
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var otherSet = new Set<T>(other);
            var thisSet = new Set<T>(this);

            this.Clear();

            foreach (T element in otherSet)
            {
                if (thisSet.Contains(element))
                {
                    this.Add(element);
                }
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set 
        /// overlaps with the specified collection.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if other was null.
        /// </exception>
        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return other.Any(this.Contains);
        }

        /// <summary>
        /// Determines whether the current set and 
        /// the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if other was null.
        /// </exception>
        /// <remarks>
        /// This method ignores the order of elements and any duplicate elements in other.
        /// </remarks>
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var otherSet = new Set<T>(other.ToList());

            if (otherSet.Count != this.Count)
            {
                return false;
            }

            return otherSet.All(this.Contains);
        }

        /// <summary>
        /// Modifies the current set so that it contains 
        /// only elements that are present either in the current set 
        /// or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if other was null.
        /// </exception>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (T element in other)
            {
                if (this.Contains(element))
                {
                    this.Remove(element);
                }
                else
                {
                    this.Add(element);
                }
            }
        }

        /// <summary>
        /// Modifies the current set so that it contains all 
        /// elements that are present in the current set, 
        /// in the specified collection, or in both.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if other was null.
        /// </exception>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (T element in other)
            {
                this.Add(element);
            }
        }

        /// <summary>
        /// Removes all elements from this set.
        /// </summary>
        public void Clear()
        {
            if (this.Count == 0)
            {
                return;
            }

            Array.Clear(this.buckets, 0, this.buckets.Length);
            this.Count = 0;
            this.amountOfFilledBuckets = 0;
            this.UpdateVersion();
        }

        /// <summary>
        /// Checks if this set contains passed item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// True if this set contains passed item, false otherwise.
        /// </returns>
        public bool Contains(T item)
        {
            int bucketIndex = this.GetBucketIndex(item);

            Node currentNode = this.buckets[bucketIndex];
            while (currentNode != null)
            {
                if (this.equalityComparer.Equals(currentNode.Value, item))
                {
                    return true;
                }

                currentNode = currentNode.Next;
            }

            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an Array, 
        /// starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="arrayIndex">
        /// The array index.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if array was null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if arrayIndex is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ICollection{T}"/> is greater 
        /// than the available space from arrayIndex to the end of the destination array.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), $"{nameof(array)} is null.");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), $"{nameof(arrayIndex)} is less than 0.");
            }

            if (this.Count > array.Length - arrayIndex + 1)
            {
                throw new ArgumentException("Array have fewer elements than the collection.");
            }

            foreach (T element in this)
            {
                array[arrayIndex] = element;
                arrayIndex++;
            }
        }

        /// <summary>
        /// Removes item from set.
        /// </summary>
        /// <param name="item">
        /// Item to remove.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// True if item was removed, false otherwise.
        /// </returns>
        public bool Remove(T item)
        {
            int bucketIndex = this.GetBucketIndex(item);

            Node currentNode = this.buckets[bucketIndex];

            if (currentNode == null)
            {
                return false;
            }

            if (this.equalityComparer.Equals(currentNode.Value, item))
            {
                ProcessValueInFirstNode();
                this.Count--;
                return true;
            }

            Node parentNode = this.buckets[bucketIndex];
            while (currentNode != null)
            {
                if (this.equalityComparer.Equals(currentNode.Value, item))
                {
                    parentNode.Next = currentNode.Next;
                    this.Count--;
                    return true;
                }

                parentNode = currentNode;
                currentNode = currentNode.Next;
            }

            return false;

            void ProcessValueInFirstNode()
            {
                if (currentNode.Next != null)
                {
                    this.buckets[bucketIndex].Next = this.buckets[bucketIndex].Next.Next;
                }
                else
                {
                    this.buckets[bucketIndex] = null;
                    this.amountOfFilledBuckets--;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get least prime number that is greater than the passed number.
        /// </summary>
        /// <param name="start">
        /// Start number.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetNextPrimeNumber(int start)
        {
            int number = start;
            while (true)
            {
                if (number % 6 == 1 || number % 6 == 5)
                {
                    if (IsPrime(number))
                    {
                        return number;
                    }
                }

                number++;
            }
        }

        /// <summary>
        /// Checks if given number is prime.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// True if passed number is prime, false otherwise.
        /// </returns>
        private static bool IsPrime(int number)
        {
            if (number == 1)
            {
                return false;
            }

            int sqrt = (int)Math.Sqrt(number);

            for (int i = 2; i < sqrt; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets bucket index of passed object based on its hash.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetBucketIndex(T item)
        {
            int hash = (item == null) ? 0 : item.GetHashCode();
            int bucketIndex = Math.Abs(hash % this.buckets.Length);
            return bucketIndex;
        }

        /// <summary>
        /// Updates this set's version.
        /// </summary>
        private void UpdateVersion() => this.version++;

        /// <summary>
        /// Creates new bucket if bucket ad <see cref="emptyBucketIndex"/> was null.
        /// </summary>
        /// <param name="item">
        /// Item to add.
        /// </param>
        /// <param name="emptyBucketIndex">
        /// The empty bucket index.
        /// </param>
        private void CreateNewBucket(T item, int emptyBucketIndex)
        {
            this.buckets[emptyBucketIndex] = new Node(item);
            this.amountOfFilledBuckets++;
            if (this.AllBucketsAreFilled())
            {
                this.Resize();
            }
        }

        /// <summary>
        /// Resize this set in a way that new capacity is least prime number
        /// that is bigger than this set's amount of items.
        /// </summary>
        private void Resize()
        {
            int newSize = GetNextPrimeNumber(this.Count);
            Node[] oldBuckets = this.buckets;
            this.buckets = new Node[newSize];

            foreach (Node bucket in oldBuckets)
            {
                Node currentNode = bucket;
                while (currentNode != null)
                {
                    this.Add(currentNode.Value);
                    currentNode = currentNode.Next;
                }
            }
        }

        /// <summary>
        /// Checks if all buckets are filled.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// True if all buckets are filled, false otherwise.
        /// </returns>
        private bool AllBucketsAreFilled() => this.amountOfFilledBuckets == this.buckets.Length;
        
        #endregion

        /// <summary>
        /// Class that represents node in bucket.
        /// </summary>
        private class Node
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Node"/> class.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            public Node(T value)
            {
                this.Value = value;
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            public T Value { get; }

            /// <summary>
            /// Gets or sets the next node.
            /// </summary>
            public Node Next { get; set; }
        }
    }
}