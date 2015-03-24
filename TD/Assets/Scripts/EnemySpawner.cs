using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
public class EnemySpawner : MonoBehaviour {

//XML数据
    public class SpawnData
    {
        //波数
        public int wave = 1;
        public string enemyname = "";
        public int level = 1;
        public float wait = 1.0f;
    }
    //起始路点
    public PathNode m_startNode;

    //存储敌人出场顺序的XML 
    public TextAsset xmldata;

    //保存所有的从xml读取的数据
    List<SpawnData> m_spawnlist;

    //距离下一个敌人出场的时间
    float m_timer = 0;

    //出场敌人的序号
    int m_index = 0;

    void Start()
    {
        //读取XML 
        ReadXML();
        //获取初始敌人
        SpawnData data = m_spawnlist[m_index];
        m_timer = data.wait;
    }

    //读取XML 
    void ReadXML()
    {
        m_spawnlist = new List<SpawnData>();
        XMLParser xmlparse = new XMLParser();
        XMLNode node = xmlparse.Parse(xmldata.text);
        XMLNodeList list = node.GetNodeList("ROOT>0>table");
        for (int i = 0; i < list.Count; i++)
        {

            string wave = node.GetValue("ROOT>0>table>" + i + ">@wave");
            string enemyname = node.GetValue("ROOT>0>table>" + i + ">@enemyname");
            string level = node.GetValue("ROOT>0>table>" + i + ">@level");
            string wait = node.GetValue("ROOT>0>table>" + i + ">@wait");

            SpawnData data = new SpawnData();
            data.wave = int.Parse(wave);
            data.enemyname = enemyname;
            data.level = int.Parse(level);
            data.wait = float.Parse(wait);

            m_spawnlist.Add(data);
        }

    }

    void Update()
    {
        SpawnEnemy();
    }

    //每隔一定时间生成一个敌人
    void SpawnEnemy()
    {
        //如果已经生成所有敌人
        if (m_index >= m_spawnlist.Count)
        {
            return;
        }
        //更新时间，等待下一个敌人
        m_timer -= Time.deltaTime;
        if (m_timer > 0)
        {
            return;
        }

        //获取下一个敌人的数据
        SpawnData data = m_spawnlist[m_index];
        //如果下一个敌人是下一波，需要等待前一波的敌人全部消亡
        if(GameManager.Instance.m_wave<data.wave)
        {
            if (GameManager.Instance.m_EnemyList.Count > 0)
            {
                return;
            }
            else
            {
                GameManager.Instance.SetWave(data.wave);
            }
        }
        //更新敌人序号
        m_index++;
        if (m_index < m_spawnlist.Count)
        {
            m_timer = m_spawnlist[m_index].wait;  //更新等待时间
        }

        //读取敌人模型
        GameObject enemymodel = Resources.Load<GameObject>(data.enemyname + "@skin");
        //读取敌人动画
        GameObject enemyani = Resources.Load<GameObject>(data.enemyname+"@run");
        //实例化敌人模型 并转向第一个路点
        Vector3 dir = m_startNode.transform.position - this.transform.position;
        GameObject enemyObj = (GameObject)Instantiate(enemymodel,this.transform.position,Quaternion.LookRotation(dir));
        enemyObj.GetComponent<Animation>().AddClip(enemyani.GetComponent<Animation>().GetClip("run"), "run");
        //设置循环动画
        enemyObj.GetComponent<Animation>()["run"].wrapMode = WrapMode.Loop;
        //播放跑步动画
        enemyObj.GetComponent<Animation>().CrossFade("run");
        //添加Enemy
        Enemy enemy = enemyObj.AddComponent<Enemy>();
        //设置敌人的出发点
        enemy.m_currentNode = m_startNode;
        //设置敌人数值
        enemy.m_life = data.level * 3;
        enemy.m_maxlife = data.level * 3;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position,"spawner.tif");
    }
}
