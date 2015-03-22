using UnityEngine;
using System.Collections;

public class TileObject : MonoBehaviour {

    public static TileObject get = null;

    protected Transform getTransform = null;

    // tile碰撞层
    public LayerMask tileLayer;
    // tile 大小
    [HideInInspector]
    public int tileSize = 1;
    // x 轴方向tile数量
    [HideInInspector]
    public int xTileCount = 2;
    // z 轴方向tile数量
    [HideInInspector]
    public int zTileCount = 2;
    // 是否显示数据信息
    [HideInInspector]
    public bool showData = false;
    [HideInInspector]
    // 每个tile的数值
    public int[] data;
    // 当前数据 id
    [HideInInspector]
    public int dataID = 0;

    void Awake()
    {
        get = this;
    }

    /// <summary>
    /// 获得相应tile的数值
    /// </summary>
    public int getData(float x, float z)
    {
        if (getTransform == null)
            getTransform = this.transform;
        int index = (int)(x - this.getTransform.position.x) * zTileCount + (int)(z - getTransform.position.z);

        return data[index];
    }

    /// <summary>
    /// 设置相应tile的数值
    /// </summary>
    public void setData( float x, float z, int number )
    {
        if (getTransform == null)
            getTransform = this.transform;
        int index = (int)(x - this.getTransform.position.x) * zTileCount + (int)(z - getTransform.position.z);

        data[index] = number;
    }

    void OnDrawGizmos()
    {
        if (!showData)
            return;
        
        for (int i = 0; i < xTileCount; i++)
        {
            for (int k = 0; k < zTileCount; k++)
            {
                if (getData(i, k) == dataID)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.3f);
                    Gizmos.DrawCube(new Vector3(i * tileSize + tileSize * 0.5f, 
                        0, 
                        k * tileSize +tileSize*0.5f), new Vector3(tileSize, 0.2f, tileSize));
                }
                
            }
        }

    }

}
