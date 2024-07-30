using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviorTree : ScriptableObject
{
    public Node root;
    public Status treeStatus = Status.Running;

    public List<Node> nodes = new List<Node>();
    
    public Status Tick()
    {
        if (treeStatus == Status.Running)
            return treeStatus = root.Tick();
        
        return treeStatus;
    }

    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(Node node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            rootNode.child = child;
        }
        
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode)
        {
            decoratorNode.child = child;
        }
        
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            compositeNode.children.Add(child);
        }
    }
    
    public void RemoveChild(Node parent, Node child)
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            rootNode.child = null;
        }
        
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode)
        {
            decoratorNode.child = null;
        }
        
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            compositeNode.children.Remove(child);
        }
    }

    public List<Node> GetChildren(Node node)
    {
        List<Node> children = new List<Node>();
        
        RootNode rootNode = node as RootNode;
        if (rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }
        
        DecoratorNode decoratorNode = node as DecoratorNode;
        if (decoratorNode && decoratorNode.child != null)
        {
            children.Add(decoratorNode.child);
        }
        
        CompositeNode compositeNode = node as CompositeNode;
        if (compositeNode)
        {
            return compositeNode.children;
        }

        return children;
    }

    public BehaviorTree Clone()
    {
        BehaviorTree tree = Instantiate(this);
        tree.root = tree.root.Clone();
        return tree;
    }
}
