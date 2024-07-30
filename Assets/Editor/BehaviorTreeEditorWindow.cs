using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviorTreeEditorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private BehaviorTreeView _treeView;
    private InspectorView _inspectorView;
    
    [MenuItem("BehaviorTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviorTreeEditorWindow wnd = GetWindow<BehaviorTreeEditorWindow>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditorWindow");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviorTree)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        // Instantiate UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviorTreeEditorWindow.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviorTreeEditorWindow.uss");
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviorTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _treeView.OnNodeSelected = OnNodeSelectionChanged;
        
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        BehaviorTree tree = Selection.activeObject as BehaviorTree;

        if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        {
            _treeView.PopulateView(tree);
        }
    }

    private void OnNodeSelectionChanged(NodeView nodeView)
    {
        _inspectorView.UpdateSelection(nodeView);
    }
}
