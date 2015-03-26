using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    //当前路点
    public PathNode m_currentNode;
    //生命
    public int m_life = 15;
    public int m_maxlife = 15;

    //移动速度
    public float m_speed = 2;

	// Use this for initialization
	void Start () {
        GameManager.Instance.m_EnemyList.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
        RotateTo();
        MoveTo();
	}
    //旋转目标
    public void RotateTo()
    {
        //获得当前y轴方向
        float current = this.transform.eulerAngles.y;
        //使用LookAt方向转向目标,因此获得了最终角度
        this.transform.LookAt(m_currentNode.transform);
        //使用Mathf.MoveTowardsAngle方法获得由当前y轴转到最终角度的中间值
        float next = Mathf.MoveTowardsAngle(current,this.transform.eulerAngles.y,120*Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0,next,0);
    }
    //向目标方向移动
    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x,pos1.z),new Vector2(pos2.x,pos2.z));
        if(dist<1.0f)
        {
            if (m_currentNode.m_next == null) //如果到达我方基地
            {
                GameManager.Instance.SetDamage(1);
                DestoryMe();
            }
            else
            {
                m_currentNode = m_currentNode.m_next;
            }
        }
        this.transform.Translate(new Vector3(0,0,m_speed *Time.deltaTime));
    }

    public void DestoryMe()
    {
        GameManager.Instance.m_EnemyList.Remove(this);
        Destroy(this.gameObject);
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life < 0)
        {
            GameManager.Instance.SetPoint(5);
            DestoryMe();
        }
    }
}
