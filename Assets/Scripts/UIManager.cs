using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    public TextMeshProUGUI orbText, timeText, deathText, gameOverText;//UI文本框

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);
    }

    public static void UpdateOrbUI(int orbCount)//更新宝珠
    {
        instance.orbText.text = orbCount.ToString();//数字变字符
    }

    public static void UpdateDeathUI(int deathCount)//更新死亡次数
    {
        instance.deathText.text = deathCount.ToString();
    }

    public static void UpdateTimeUI(float time)//界面更新时间
    {
        int minutes = (int)(time / 60);//分钟
        float seconds = time % 60;//秒 119 / 60 1余数59

        //界面时间显示2位数
        instance.timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public static void DisplayGameOver()//界面显示游戏结束
    {
        instance.gameOverText.enabled = true;
    }
}
