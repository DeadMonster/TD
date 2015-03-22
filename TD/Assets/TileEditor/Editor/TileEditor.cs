using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileObject))]
public class TileEditor : Editor {

    /// <summary>
    /// 是否处于编辑模式
    /// </summary>
    protected bool editMode = false;
    /// <summary>
    /// 编辑模式id
    /// </summary>
    protected int editID = 0;
    /// <summary>
    /// 是否显示数据信息
    /// </summary>
    protected bool showData = false;
    /// <summary>
    /// 受编辑器影响的tile角本
    /// </summary>
    protected TileObject tileObject;
    /// <summary>
    /// 编辑的Mesh
    /// </summary>
    protected Mesh tileMesh;

    // 当前材质球 id
    protected int matID=0;

    void OnEnable()
    {
        // 获得tile角本
        tileObject = (TileObject)target;
        // 获得Mesh
        tileMesh = tileObject.GetComponent<MeshFilter>().sharedMesh;
    }

    public void OnSceneGUI()
    {
        if (editMode)
        {
            // 取消编辑器的选择功能
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            // 获取Input事件
            Event e = Event.current;

            if ( editID==1 )
                tileObject.showData = true;

            // 如果是鼠标左键
            if ( e.isMouse && e.button == 0 && e.clickCount == 1)
            {
                // 获取由鼠标位置产生的射线
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                // 计算碰撞
                RaycastHit hitinfo;
                if (Physics.Raycast(ray, out hitinfo, 2000, tileObject.tileLayer))
                {
                    float tx = hitinfo.point.x - tileObject.transform.position.x;
                    float tz = hitinfo.point.z - tileObject.transform.position.z;

                    if (editID == 0)
                    {
                        int submeshcount = tileObject.GetComponent<MeshRenderer>().sharedMaterials.Length;
                        tileMesh.subMeshCount = submeshcount;

                        // 取得所有顶点 法线 uv
                        Vector3[] vertices = tileMesh.vertices;
                        Vector3[] normals = tileMesh.normals;
                        Vector2[] uvs = tileMesh.uv;
                        Vector2[] uvs2 = tileMesh.uv2;

                        // 取得所有三角形
                        ArrayList[] triglist = new ArrayList[submeshcount];
                        for (int i = 0; i < submeshcount; i++)
                        {
                            if (triglist[i] == null)
                                triglist[i] = new ArrayList();
                            int[] trs = tileMesh.GetTriangles(i);
                            for (int j = 0; j < trs.Length; )
                            {
                                int index = i;

                                if (i != matID && j % 6 == 0 &&
                                    (vertices[trs[j]].x <= tx && vertices[trs[j + 4]].x >= tx && vertices[trs[j]].z <= tz && vertices[trs[j + 4]].z >= tz))
                                {
                                    index = matID;
                                }

                                if (triglist[index] == null)
                                    triglist[index] = new ArrayList();
                                for (int k = 0; k < 6; k++)
                                    triglist[index].Add((object)trs[j + k]);
                                j += 6;
                            }
                        }

                        // 重新构造Mesh
                        tileMesh.Clear();
                        tileMesh.vertices = vertices;
                        tileMesh.uv = uvs;
                        tileMesh.uv2 = uvs2;
                        tileMesh.normals = normals;

                        tileMesh.subMeshCount = submeshcount;
                        for (int i = 0; i < triglist.Length; i++)
                        {
                            int[] newtrigs = (int[])triglist[i].ToArray(typeof(int));
                            if (newtrigs != null && newtrigs.Length > 0)
                                tileMesh.SetTriangles(newtrigs, i);
                        }
                    }
                    else if (editID == 1)
                    {
                        // 重新指定tile数组信息
                        tileObject.setData(tx, tz, tileObject.dataID);
                    }
                }
            }
        }
        else
        {
            // 恢复编辑器的选择功能
            HandleUtility.Repaint();
            tileObject.showData = false;
        }
    }

    public override void OnInspectorGUI()
    {
        int maxcount = tileObject.GetComponent<MeshRenderer>().sharedMaterials.Length;

        GUILayout.Label("Tile Editor");
        EditorGUILayout.LabelField("Tile Size:" + tileObject.tileSize);
        EditorGUILayout.LabelField("X Tile Count:" + tileObject.xTileCount);
        EditorGUILayout.LabelField("Z Tile Count:" + tileObject.zTileCount);
        EditorGUILayout.LabelField("Materials:" + maxcount);
        EditorGUILayout.Separator();

        editMode = EditorGUILayout.Toggle("Edit", editMode);
        string[] editModeStr = {"Materials","Data"};

        editID = GUILayout.Toolbar(editID, editModeStr);
        if (editID == 0)
        {
            matID = EditorGUILayout.IntSlider("Material ID", matID, 0, maxcount - 1);
        }
        else if (editID == 1)
        {
            tileObject.dataID = EditorGUILayout.IntSlider("Data ID", tileObject.dataID, 0, 9);
        }

        EditorGUILayout.Separator();

        DrawDefaultInspector();
    }
}
