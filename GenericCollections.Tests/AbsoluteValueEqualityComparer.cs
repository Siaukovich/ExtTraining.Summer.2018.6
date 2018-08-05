namespace GenericCollections.Tests
{
    using System;
    using System.Collections.Generic;

    public class AbsoluteValueEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => Math.Abs(x) == Math.Abs(y);

        public int GetHashCode(int obj) => obj.GetHashCode();
    }
}