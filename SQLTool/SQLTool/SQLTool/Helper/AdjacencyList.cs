using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTool.Helper
{
    public class AdjacencyList<T>
    {
        //public delegate void OnVertexTraverseDelegate(T value, List<Vertex<T>> path);
        public delegate void OnVertexTraverseDelegate(List<T> path);
        private List<Vertex<T>> vertices = new List<Vertex<T>>();
        private Vertex<T> startingVertex = null;
        private OnVertexTraverseDelegate onVertexTraverse = null;

        public AdjacencyList(Vertex<T> startingVertex, OnVertexTraverseDelegate onVertexTraverse)
        {
            this.startingVertex = startingVertex;
            this.onVertexTraverse = onVertexTraverse;
            this.vertices.Add(startingVertex);
        }

        public void AddPath(List<T> values)
        {
            Vertex<T> lastVertex = this.startingVertex;

            values
                .ForEach(
                    (value) =>
                    {
                        Vertex<T> vertex = this.vertices.Where(v => v.Equals(v.Value, value)).SingleOrDefault();
                        if (vertex == null)
                        {
                            // Create a new vertex.
                            vertex = new Vertex<T>(value);

                            // Add the new vertex to the adjacency list of vertices.
                            this.vertices.Add(vertex);
                        }

                        // Add the vertex as an edge to the edges associated with the last vertex if it does not already exist.
                        if (lastVertex.FindVertex(value) == default(Vertex<T>))
                        {
                            lastVertex.AddVertex(vertex);
                        }

                        lastVertex = vertex;
                    }
               );
        }

        public void Traverse()
        {
            //var startingVertexList = new List<Vertex<T>>();
            //this.TraverseVertex(this.startingVertex, startingVertexList);
            var startingVertexList = new List<T>();
            this.TraverseVertex(this.startingVertex, startingVertexList);
        }

        //private void TraverseVertex(Vertex<T> v, List<Vertex<T>> path)
        //{
        //    path.Add(v);
        //    if (this.onVertexTraverse != null) this.onVertexTraverse(v.Value, path);

        //    v.Vertices
        //        .ForEach(
        //            (nextV) =>
        //            {
        //                this.TraverseVertex(nextV, path);
        //            }
        //       );

        //    v.Visited = true;
        //}
        private void TraverseVertex(Vertex<T> v, List<T> path)
        {
            //path.Add(v.Value);
            //if (this.onVertexTraverse != null) this.onVertexTraverse(path);

            List<T> cumulativePath = new List<T>(path.Count + 1);
            cumulativePath.AddRange(path);
            cumulativePath.Add(v.Value);
            if (this.onVertexTraverse != null) this.onVertexTraverse(cumulativePath);

            v.Vertices
                .ForEach(
                    (nextV) =>
                    {
                        //this.TraverseVertex(nextV, path);
                        this.TraverseVertex(nextV, cumulativePath);
                    }
               );

            v.Visited = true;
        }
    }
}
