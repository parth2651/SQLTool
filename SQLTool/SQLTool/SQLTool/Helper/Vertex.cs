using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTool.Helper
{
    public class Vertex<T> : IEqualityComparer
    {
        private CaseInsensitiveComparer ciComparer = null;

        public T Value { get; private set; }
        public bool Visited { get; set; }
        public List<Vertex<T>> Vertices { get; set; } // Vertices connected to this vertex by an edge.

        public Vertex(T value)
        {
            this.Value = value;
            this.Vertices = new List<Vertex<T>>();
            this.ciComparer = CaseInsensitiveComparer.DefaultInvariant;
        }

        public Vertex<T> FindVertex(T value)
        {
            return this.Vertices.Where(v => this.Equals(v.Value, value)).SingleOrDefault();
        }

        public void AddVertex(Vertex<T> vertex)
        {
            this.Vertices.Add(vertex);
        }

        public new bool Equals(object x, object y)
        {
            if (this.ciComparer.Compare(x, y) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(object obj)
        {
            return obj.ToString().ToLower().GetHashCode();
        }
    }
}
