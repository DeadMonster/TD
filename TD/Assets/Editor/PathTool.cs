using UnityEngine;
using System.Collections;
using UnityEditor;

public class PathTool : ScriptableObject {

    //父路点
    static PathNode m_parent = null;

    [MenuItem("PathTool/Creat PathNode")]
    static void CreatPathNode()
    {
        //创建一个新路点
        GameObject go = new GameObject();
        go.AddComponent<PathNode>();
        go.name = "pathnode";
        //设置tag
        go.tag = "pathnode";
        //使该路点处于选中状态
        Selection.activeTransform = go.transform;
    }

    //菜单SetParent，用来设置父节点，快捷键Ctrl+Q
    [MenuItem("PathTool/Set Parent %q")]
    static void SetParent()
    {
        //如果没有选中物体或者选择的物体大于1,返回
        if (!Selection.activeGameObject || Selection.GetTransforms(SelectionMode.Unfiltered).Length > 1)
        {
            return;
        }
        //如果选中物体的Tag设为pathnode
        if(Selection.activeGameObject.tag.CompareTo("pathnode")==0)
        {
            //保存当前选中节点
            m_parent=Selection.activeGameObject.GetComponent<PathNode>();
        }
    }

    //菜单SetNextChild,用来设置子路点，快捷键Ctrl+w
    [MenuItem("PathTool/Set Next %w")]
    static void SetNextChild()
    {
        if (!Selection.activeGameObject || m_parent == null)
        {
            return;
        }
        if (Selection.activeGameObject.tag.CompareTo("pathnode") == 0)
        {
            //设置子路点
            m_parent.SetNext(Selection.activeGameObject.GetComponent<PathNode>());
            m_parent = null;
        }
    }

}
