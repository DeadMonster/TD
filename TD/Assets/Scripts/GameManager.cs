using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    //波数
    public int m_wave = 1;
    public int m_waveMax = 10;
    //生命
    public int  m_life=10;
    //铜钱数量
    public int m_point = 50;
    //文字控件
    Text m_txt_wave;
    Text m_txt_life;
    Text m_txt_point;

    //重新游戏按钮
    Button m_but_try;
    //当前是否选中的创建防守单位的按钮
    bool m_isSelectedButton = false;

    void Awake()
    {
        Instance = this;
    }
	// Use this for initialization
	void Start () {
	//定义UnityAction 在OnbutCreateDefendrDown函数中响应按钮按下事件
        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnbutCreateDefendrDown);
    //定义UnityAction 在OnbutCreateDefendrDown函数中响应按钮抬起事件
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnbutCreateDefendrUp);
    //定义按钮按下事件Entry
        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);

    //定义按钮抬起事件Entry
        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);

    //查找所有子物体，根据名称获取UI控件
        foreach(Transform t in this.GetComponentsInChildren<Transform>())
        {
            if(t.name.CompareTo("wave")==0) //找到文字控件“波数”
            {
                m_txt_wave = t.GetComponent<Text>();
                SetWave(1);
            }
            else if(t.name.CompareTo("life")==0)//找到文字控件生命
            {
                m_txt_life = t.GetComponent<Text>();
                m_txt_life.text = string.Format("生命：<color=yellow>{0}</color>", m_life);
            }
            else if (t.name.CompareTo("point") == 0)//找到文字控件铜钱
            {
                m_txt_point = t.GetComponent<Text>();
                m_txt_life.text = string.Format("铜钱:<color=yellow>{0}</color>", m_point);
            }
            else if (t.name.CompareTo("but_try") == 0)//找到按键重新游戏
            {
                m_but_try = t.GetComponent<Button>();
                //添加按钮单击回调
             //   m_but_try.onClick.AddListener( OnButRetry);
                //隐藏重新游戏按钮
                m_but_try.gameObject.SetActive (false);

            }
            else if (t.name.Contains("but_player"))
            {
                //给创建防守单位按钮添加EventTrigger，并添加前面定义的按钮事件
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.delegates = new List<EventTrigger.Entry>();
                trigger.delegates.Add(down);
                trigger.delegates.Add(up);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //更新文字控件波数
    public void SetWave(int wave)
    {
        m_wave = wave;
        m_txt_wave.text = string.Format("波数：<color=yellow>{0}/{1}</color>", m_wave, m_waveMax);
    }

    //更新文字控件生命
    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            m_life = 0;
            m_but_try.gameObject.SetActive(true);//如果生命值为0，则显示重新开始游戏按钮
        }
        m_txt_life.text = string.Format("生命:<color=yellow>{0}</color>", m_life);
    }

    //更新文字控件铜钱
    public bool SetPoint(int point)
    {
        if (m_point + point < 0)//铜钱不可能小于0
            return false;
        m_point += point;
        m_txt_point.text = string.Format("铜钱:<color=yellow>{0}</color>", m_point);
        return true;
    }
    //重新响应游戏按钮
    void OnButRetry(GameObject go)
    {
        Application.LoadLevel(Application.loadedLevelName);
    }
    //按下传教防守单位按钮
   void OnbutCreateDefendrDown(BaseEventData data)
   {
       m_isSelectedButton = true;
   }
    //抬起创建防守单位按钮
   void OnbutCreateDefendrUp(BaseEventData data)
   {

   }
}
