using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class BehaviorTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }

    public Action<NodeView> OnNodeSelected;

    private BehaviorTree _tree;
    
    public BehaviorTreeView()
    {
        Insert(0, new GridBackground());
        
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviorTreeEditorWindow.uss");
        styleSheets.Add(styleSheet);
    }

    private NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    public void PopulateView(BehaviorTree tree)
    {
        _tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (_tree.root == null)
        {
            _tree.root = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(_tree);
            AssetDatabase.SaveAssets();
        }
        
        tree.nodes.ForEach(n => CreateNodeView(n));
        
        tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node
        ).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
    {
        if (graphviewchange.elementsToRemove != null)
        {
            graphviewchange.elementsToRemove.ForEach(e =>
            {
                NodeView nodeView = e as NodeView;
                if (nodeView != null)
                {
                    _tree.DeleteNode(nodeView.node);
                }
                
                Edge edge = e as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    _tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (graphviewchange.edgesToCreate != null)
        {
            graphviewchange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                
                _tree.AddChild(parentView.node, childView.node);
            });
        }
        
        return graphviewchange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a)=>CreateNode(type));
            }
        }
        
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a)=>CreateNode(type));
            }
        }
        
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a)=>CreateNode(type));
            }
        }
    }

    private void CreateNode(System.Type type)
    {
        Node node = _tree.CreateNode(type);
        CreateNodeView(node);
    }

    private void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }
    
}
