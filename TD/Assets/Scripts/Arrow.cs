using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
    //当打击到目标时执行的动作
    System.Action<Enemy> onAttack;
    //目标对象
    Transform m_target;
    //目标对象的模型边框
    Bounds m_targetCenter;
    //静态函数 创建弓箭
    public static void Creat(Transform target,Vector3 pos,System.Action<Enemy>onAttack)
    {
        //创建新的游戏体
        GameObject go = new GameObject();
        //添加弓箭脚本组件
        Arrow ar = go.AddComponent<Arrow>();
        //设置弓箭目标
        ar.m_target = target;
        //获得目标模型边框
        ar.m_targetCenter = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        //取得Action
        ar.onAttack = onAttack;
        //初始化位置
        ar.transform.position = pos;
        ar.transform.LookAt(target);
        //读取弓箭模型
        GameObject arrowmodel = Resources.Load<GameObject>("arrow");
        //实例化弓箭模型
        GameObject arrowObj = (GameObject)Instantiate(arrowmodel,ar.transform.position,ar.transform.rotation);
        //将弓箭模型设为当前游戏体的子物体
        arrowObj.transform.parent = go.transform;
        //3秒之后销毁
        Destroy(go,3.0f);
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	//瞄准目标
        if (m_target != null)
        {
            this.transform.LookAt(m_targetCenter.center);
        }
        //向目标前进
        this.transform.Translate(new Vector3(0,0,10*Time.deltaTime));
        if(m_target!=null)
        {
            //检查是否打击到目标
            if (Vector3.Distance(this.transform.position, m_targetCenter.center) < 0.5f)
            {
                //通知弓箭发射者
                onAttack(m_target.GetComponent<Enemy>());
                Destroy(this.gameObject);
            }
        }
	}
}
