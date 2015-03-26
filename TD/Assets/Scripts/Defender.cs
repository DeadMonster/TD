using UnityEngine;
using System.Collections;

public class Defender : MonoBehaviour {
    //格子状态
    public enum TileStatus
    {
        LOCK=0,//不能在上面做任何事
        ROAD=1,//敌人在上面行走
        FREE=2,//专门用于创建防守单位的格子
    }

    //攻击范围
    public float m_attackArea = 2.0f;
    //攻击力
    public int m_power = 1;
    //目标敌人
    protected Enemy m_targetEnemy;
    //防守单位模型
    protected GameObject m_model;
    //防守单位动画
    protected Animation m_ani;

    public delegate void VoidDelegate();
    //用一个委托指向不同行为（在一个项目中最好使用状态机来管理不同行为）
    public event VoidDelegate m_action;
    public void SetAction(VoidDelegate act)
    {
        m_action = act;
    }

    //静态工厂函数 创建防守单位实例
    public static T Create<T>(Vector3 pos,Vector3 angle)where T:Defender
    {
        GameObject go = new GameObject();
        go.transform.position = pos;
        go.transform.eulerAngles = angle;
        go.name = "defender";
        T d = go.AddComponent<T>();
        d.Init();//初始化

        //将自己所占的格子信息设为占用
        TileObject.get.setData(d.transform.position.x, d.transform.position.z, (int)TileStatus.LOCK);
        return d;
    }

    //初始化数值
    protected virtual void Init()
    {
        m_attackArea = 2.0f;//在实际项目中，数值通常会从配置文件中读取
        m_power = 2;
        CreateModel("swordman");
        m_action = Idle;
    }

    //创建模型
    protected void CreateModel(string myname)
    {
        //读取模型和动画资源
        GameObject model=Resources.Load<GameObject>(myname+"@skin");
        GameObject ani_run = Resources.Load<GameObject>(myname+"@attack");
        GameObject ani_idle = Resources.Load<GameObject>(myname+"@idle");
        //创建模型
        m_model = (GameObject)Instantiate(model, transform.position, transform.rotation);
        model.transform.parent = this.transform;
        m_ani = m_model.GetComponent<Animation>();
        m_ani.AddClip(ani_run.GetComponent<Animation>().GetClip("attack"), "attack");
        m_ani.AddClip(ani_idle.GetComponent<Animation>().GetClip("idle"), "idle");
        //ClampForever动画会停留在最后一帧
        m_ani["attack"].wrapMode = WrapMode.ClampForever;
        m_ani["idle"].wrapMode = WrapMode.Loop;
        m_ani.CrossFade("idle");

    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	//更新当前行为
        if (m_action != null)
            m_action();
	}

    //待机状态
    protected void Idle()
    {
        if (!m_ani.IsPlaying("idle")) //如果没有播放待机动画则播放待机动画
        {
            m_ani.CrossFade("idle");
        }
        FindEnemy();
        if (m_targetEnemy != null)//如果找到敌人则进入攻击状态
        {
            m_action = Attack;
        }
    }

    protected virtual void Attack()
    {
        bool ok = RotateTo();
        if (ok)
        {
            if (!m_ani.IsPlaying("attack"))
            {
                m_ani.CrossFade("attack");
            }
            else if(m_ani["attack"].time>m_ani["attack"].length*0.5f)
            {
                //当动画播到一半时进入OnAttack状态
                m_action = OnAttack;
            }
        }
    }

    protected virtual void OnAttack()
    {
        if (m_targetEnemy != null)
        {
            m_targetEnemy.SetDamage(m_power);//减少敌人生命值
        }
        m_targetEnemy = null;
        if (m_ani["attack"].time >= m_ani["attack"].length) //播完动画回到待机状态
        {
            m_action = Idle;
            m_ani.Stop("attack");
        }
    }

    //转向敌人
    public bool RotateTo()
    {
        if (m_targetEnemy == null)
            return true;
        //是否转向目标
        bool ok = false;
        Vector3 targetpos = m_targetEnemy.transform.position;
        targetpos.y = this.transform.position.y;

        //获得目标方向
        Vector3 targetDir = targetpos - transform.position;
        //向目标方向旋转，返回新的方向
        Vector3 rot_delta = Vector3.RotateTowards(this.transform.forward, targetDir, 20.0f * Time.deltaTime, 0.0f);
        //转向这个方向
        Quaternion targetrotation = Quaternion.LookRotation(rot_delta);

        //获得目标方向与当前方向的差值
        float angle = Vector3.Angle(targetDir,transform.forward);

        //如果
        if (angle < 1.0f)
        {
            //已转向目标
            ok = true;
        }
        transform.rotation = targetrotation;
        return ok;
    }

    //查找最近的敌人
    void FindEnemy()
    {
        if (m_targetEnemy != null)
        {
            return;
        }

        float mindist = -1;
        foreach(Enemy enemy in GameManager.Instance.m_EnemyList)
        {
            if (enemy.m_life == 0)
                continue;
            Vector3 pos1 = this.transform.position;
            pos1.y = 0;
            Vector3 pos2 = enemy.transform.position;
            pos2.y = 0;
            //获得目标距离自己的距离
            float dist = Vector3.Distance(pos1, pos2);
            //如果目标不在攻击范围内
            if (dist > m_attackArea)
            {
                continue;
            }
            //寻找距离最近的
            if(mindist<0||mindist>dist)
            {
                //更新目标
                m_targetEnemy = enemy;
                mindist = dist;
            }

        }
    }
}
