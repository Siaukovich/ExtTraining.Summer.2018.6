namespace GenericCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

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

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
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

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
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

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
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