using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;//当前静态实例，可被调用

    SceneFader fader;

    List<Orb> orbs;//宝珠

    Door lockedDoor;//门

    float gameTime;//游戏时间
    bool gameIsOver;//游戏结束

    //public int orbNum;//宝珠数量
    public int deathNum;//死亡次数

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);//把第二个以上的都销毁，避免重复
            return;
        }

        instance = this;

        orbs = new List<Orb>();//新的orb列表

        DontDestroyOnLoad(this);//场景加载时一直存在
    }

    private void Update()
    {
        if (gameIsOver)//游戏直接结束
            return;

        //orbNum = instance.orbs.Count;//统计orb数量

        gameTime += Time.deltaTime;//实时游戏时间

        //UImanager
        UIManager.UpdateTimeUI(gameTime);
    }

    public static void RegisterDoor(Door door)//注册 门
    {
        instance.lockedDoor = door;
    }

    public static void RegisterSceneFader(SceneFader obj)//注册SceneFader
    {
        instance.fader = obj;
    }

    public static void RegisterOrb(Orb orb)//注册宝珠
    {
        if (instance == null)
            return;

        if (!instance.orbs.Contains(orb))//当前不包含orb
            instance.orbs.Add(orb);//添加注册orb

        UIManager.UpdateOrbUI(instance.orbs.Count);//一开始就显示宝珠
    }

    public static void PlayerGrabbedOrb(Orb orb)//角色获得宝珠
    {
        if (!instance.orbs.Contains(orb))//当前列表不包含宝珠
            return;

        instance.orbs.Remove(orb);

        if (instance.orbs.Count == 0)//当前宝珠数量为0开门
            instance.lockedDoor.Open();

        UIManager.UpdateOrbUI(instance.orbs.Count);//宝珠界面计数
    }

    public static void PlayerWon()//角色胜利游戏结束
    {
        instance.gameIsOver = true;

        //ui game over
        UIManager.DisplayGameOver();

        AudioManager.PlayerWonAudio();
    }

    public static bool GameOver()//游戏已经结束
    {
        return instance.gameIsOver;
    }

    public static void PlayerDied()//角色死亡
    {
        instance.fader.FadeOut();
        instance.deathNum++;

        UIManager.UpdateDeathUI(instance.deathNum);//死亡界面计数

        instance.Invoke("RestartScene", 1.5f);//重新加载延迟
    }

    public void RestartScene()
    {
        instance.orbs.Clear();//清空宝珠列表

        //重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()//退出游戏
    {
        Application.Quit();
    }

    //public void GoToMainMenu()//回主菜单
    //{
    //    SceneManager.LoadScene(0);
    //}
}
