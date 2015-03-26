using UnityEngine;
using System.Collections;

public class Archer : Defender {
    //模型边框
    Bounds bounds;

    //初始化数值
    protected override void Init()
    {
        m_attackArea = 5.0f;
        m_power = 1;
        CreateModel("archer");
        SetAction(Idle);
        //获得模型边框
        bounds = m_model.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
    }
    protected override void Attack()
    {
        bool ok = RotateTo();
        if (ok)
        {
            if(!m_ani.IsPlaying("attack"))
            {
                m_ani["attack"].time = 0;
                m_ani.CrossFade("attack");
            }
            else if(m_ani["attack"].time>m_ani["attack"].length*0.5f)
            {
                SetAction(OnAttack);
                if (m_targetEnemy != null)
                {
                    //将弓箭的初始位置设置到自身中心偏上并向前0.5个单位]
                    Vector3 pos = this.transform.TransformPoint(0,bounds.center.y+0.5f,0.5f);

                    //创建弓箭
                    Arrow.Creat(m_targetEnemy.transform,pos,(Enemy enemy)=>
                    {
                        enemy.SetDamage(m_power);
                        m_targetEnemy = null;
                });
                }
            }
        }
    }
//向敌人攻击
    protected override void OnAttack()
    {
        if(m_ani["attack"].time>=m_ani["attack"].length)
        {
            SetAction(Idle);
            m_ani.Stop("attack");
        }
    }
}
