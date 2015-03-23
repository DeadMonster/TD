using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

    public static GameCamera Inst = null;

    //摄像机距离地面距离
    protected float m_distance = 15;

    //摄像机角度
    protected Vector3 m_rot = new Vector3(-55, 180, 0);

    //摄像机的移动速度
    protected float m_moveSpeed = 60;

    //摄像机的移动值；
    protected float m_vx = 0;
    protected float m_vy = 0;

    //Transform组件
    protected Transform m_transform;

    //摄像机的焦点
    protected Transform m_cameraPoint;

    void Awake()
    {
        Inst = this;
        m_transform = this.transform;
    }
	// Use this for initialization
	void Start () {
	//获得摄像机的焦点
        m_cameraPoint = CameraPoint.Instance.transform;
        Follow();
        
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Follow();
	}

    //摄像机对齐到焦点的位置和距离
    void Follow()
    {
        m_transform.position = m_cameraPoint.position;
        m_transform.eulerAngles = m_rot;
        m_transform.Translate(0, 0, m_distance);

        this.transform.LookAt(m_cameraPoint);
    }

    //控制摄像机移动
    public void Control(bool mouse,float mx,float my)
    {
        if(!mouse)
        {
            return;
        }
        m_cameraPoint.Translate(-mx,0,-my);
    }
}
