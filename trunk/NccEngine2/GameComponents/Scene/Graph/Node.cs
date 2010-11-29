using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NccEngine.GameComponents;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace NccEngine2.GameComponents.Scene.Graph
{
    public class Node : IComparable
    {
        public Guid Key = Guid.NewGuid();

        public List<Node> Nodes { get; private set; }

        public void Sort()
        {
            Nodes.Sort();
        }

        public Node()
        {
            Nodes = new List<Node>();
        }

        public void AddNode(Node newNode)
        {
            Nodes.Add(newNode);
        }

        public Node GetNode(string key)
        {
            return Nodes.FirstOrDefault(node => node.Key.ToString() == key);
        }

        public Node GetNode(Guid key)
        {
            return Nodes.FirstOrDefault(node => node.Key == key);
        }

        int IComparable.CompareTo(object obj)
        {
            var node1 = (SceneObjectNode)this;
            var node2 = (SceneObjectNode)obj;

            if (node1.SceneObject.Distance < node2.SceneObject.Distance)
                return -1;
            return node1.SceneObject.Distance > node2.SceneObject.Distance ? 1 : 0;
        }

        public virtual void HandleInput(GameTime gameTime, Input input)
        {
            Nodes.ForEach(node => node.HandleInput(gameTime, input));
        }

        public virtual void Update(GameTime gameTime)
        {
            Nodes.ForEach(node => node.Update(gameTime));
        }

        public void UnloadContent(NccSceneObject obj)
        {
            Nodes.ForEach(delegate(Node node)
            {
                if (((SceneObjectNode)node).SceneObject != obj) return;
                node.UnloadContent();
                Nodes.Remove(node);
            });

        }

        public void UnloadContent(INccSceneObject obj)
        {
            Nodes.ForEach(
                delegate(Node node)
                {
                    if (((SceneObjectNode)node).SceneObject != obj) return;
                    node.UnloadContent();
                    Nodes.Remove(node);
                });

        }

        public virtual void UnloadContent()
        {
            Nodes.ForEach(node => node.UnloadContent());
            Nodes.Clear();
        }

        public virtual void LoadContent()
        {
            Nodes.ForEach(node => node.LoadContent());
        }

        public virtual void Draw(GameTime gameTime)
        {
            Nodes.ForEach(node => node.Draw(gameTime));
        }

        public virtual void DrawCulling(GameTime gameTime)
        {
            Nodes.ForEach(node => node.DrawCulling(gameTime));
        }

    }

}