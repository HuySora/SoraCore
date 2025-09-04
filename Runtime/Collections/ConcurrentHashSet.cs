using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SoraCore.Collection {
    public partial class ConcurrentHashSet<T> {
        private readonly IEqualityComparer<T> m_Comparer;
        private readonly ConcurrentDictionary<T, bool> m_Dict;
        
        public int Count => m_Dict.Count;
        public bool IsReadOnly => false;
        
        public ConcurrentHashSet() {
            m_Comparer = EqualityComparer<T>.Default;
            m_Dict = new ConcurrentDictionary<T, bool>(m_Comparer);
        }
        
        public ConcurrentHashSet(IEqualityComparer<T> comparer) {
            m_Comparer = comparer ?? EqualityComparer<T>.Default;
            m_Dict = new ConcurrentDictionary<T, bool>(m_Comparer);
        }
        
        public bool Add(T item) => m_Dict.TryAdd(item, true);
        public void Clear() => m_Dict.Clear();
        public bool Contains(T item) => m_Dict.ContainsKey(item);
        public void CopyTo(T[] array, int arrayIndex) => m_Dict.Keys.CopyTo(array, arrayIndex);
        public bool Remove(T item) => m_Dict.TryRemove(item, out _);
    }
    
    public partial class ConcurrentHashSet<T> : ISet<T> {
        void ICollection<T>.Add(T item) => Add(item);
        bool ISet<T>.Add(T item) => Add(item);
        
        public void ExceptWith(IEnumerable<T> other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            foreach (var item in other) Remove(item);
        }
        
        public void IntersectWith(IEnumerable<T> other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            var toKeep = new HashSet<T>(other, m_Comparer);
            foreach (var item in this) {
                if (!toKeep.Contains(item)) Remove(item);
            }
        }
        
        public void SymmetricExceptWith(IEnumerable<T> other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            var unique = new HashSet<T>(other, m_Comparer);
            foreach (var item in unique) {
                if (!Remove(item)) Add(item);
            }
        }
        
        public void UnionWith(IEnumerable<T> other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            foreach (var item in other) Add(item);
        }
        
        public bool IsProperSubsetOf(IEnumerable<T> other) =>
            new HashSet<T>(this, m_Comparer).IsProperSubsetOf(other);
        
        public bool IsProperSupersetOf(IEnumerable<T> other) =>
            new HashSet<T>(this, m_Comparer).IsProperSupersetOf(other);
        
        public bool IsSubsetOf(IEnumerable<T> other) =>
            new HashSet<T>(this, m_Comparer).IsSubsetOf(other);
        
        public bool IsSupersetOf(IEnumerable<T> other) =>
            new HashSet<T>(this, m_Comparer).IsSupersetOf(other);
        
        public bool Overlaps(IEnumerable<T> other) =>
            new HashSet<T>(this, m_Comparer).Overlaps(other);
        
        public bool SetEquals(IEnumerable<T> other) =>
            new HashSet<T>(this, m_Comparer).SetEquals(other);
        
        public IEnumerator<T> GetEnumerator() => m_Dict.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}