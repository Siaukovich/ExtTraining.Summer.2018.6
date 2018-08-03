using System.Runtime.CompilerServices;

namespace GenericCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    public class Set<T> : ISet<T>
    {
        public int Count { get; private set; }
        public bool IsReadOnly => false;

        private Node[] buckets;

        private int amountOfFilledBuckets;

        private int version;

        private readonly IEqualityComparer<T> comparer;
        
        public Set(int defaultCapacity = 5)
        {
            this.buckets = new Node[defaultCapacity];
            this.comparer = EqualityComparer<T>.Default;
        }

        public Set(IEqualityComparer<T> comparer, int defaultCapacity = 5)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            this.buckets = new Node[defaultCapacity];
        }
        
        public Set(IEnumerable<T> collection, int defaultCapacity = 5)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            
            this.buckets = new Node[defaultCapacity];

            foreach (var element in collection)
            {
                this.Add(element);
            }
        }
        
        public Set(ICollection<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            
            this.buckets = new Node[GetNextPrimeNumber(collection.Count)];

            foreach (var element in collection)
            {
                this.Add(element);
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            int startVersion = this.version;
            for (int i = 0; i < this.buckets.Length; i++)
            {
                Node currentNode = buckets[i];
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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void UpdateVersion() => version++;

        public bool Add(T item)
        {
            int hash = (item == null) ? 0 : item.GetHashCode();
            int bucketIndex = hash % this.buckets.Length;

            if (this.buckets[bucketIndex] == null)
            {
                this.CreateNewBucket(item, bucketIndex);
            }
            else
            {
                Node currentNode = buckets[bucketIndex];
                Node parentNode = buckets[bucketIndex];
                while (currentNode != null)
                {
                    if (this.comparer.Equals(currentNode.Value, item))
                    {
                        return false;
                    }

                    parentNode = currentNode;
                    currentNode = currentNode.Next;
                }
                
                parentNode.Next = new Node(item);
            }
                           
            UpdateVersion();
            this.Count++;
            
            return true;
        }

        private void CreateNewBucket(T item, int emptyBucketIndex)
        {
            this.buckets[emptyBucketIndex] = new Node(item);
            this.amountOfFilledBuckets++;
            if (this.AllBucketsAreFilled())
            {
                this.Resize();
            }
        }

        private void Resize()
        {
            int newSize = GetNextPrimeNumber(this.Count);
            var oldBuckets = this.buckets;
            this.buckets = new Node[newSize];
            
            for (int i = 0; i < oldBuckets.Length; i++)
            {
                Node currentNode = oldBuckets[i];
                while (currentNode != null)
                {
                    this.Add(currentNode.Value);
                    currentNode = currentNode.Next;
                }
            }
        }

        private bool AllBucketsAreFilled() => this.amountOfFilledBuckets == this.buckets.Length;

        private int GetNextPrimeNumber(int start)
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

        private bool IsPrime(int number)
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
        
        bool ISet<T>.Add(T item) => this.Add(item);


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
        
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            int hash = (item == null) ? 0 : item.GetHashCode();
            int bucketIndex = hash % this.buckets.Length;

            Node currentNode = this.buckets[bucketIndex];
            while (currentNode != null)
            {
                if (this.comparer.Equals(currentNode.Value, item))
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

        public bool Remove(T item)
        {
            int hash = (item == null) ? 0 : item.GetHashCode();
            int bucketIndex = hash % this.buckets.Length;

            // TODO If this item is single item in bucket.
            
            if (this.comparer.Equals(buckets[bucketIndex].Value, item))
            {
                buckets[bucketIndex].Next = buckets[bucketIndex].Next.Next;
            }
            
            Node parentNode = this.buckets[bucketIndex];
            Node currentNode = this.buckets[bucketIndex];
            while (currentNode != null)
            {
                if (this.comparer.Equals(currentNode.Value, item))
                {
                    parentNode.Next = currentNode.Next;
                    return true;
                }

                parentNode = currentNode;
                currentNode = currentNode.Next;
            }

            return false;
        }

        private class Node
        {
            public Node(T value)
            {
                this.Value = value;
            }
            
            public T Value { get; }
            public Node Next { get; set; }
        }
    }
}