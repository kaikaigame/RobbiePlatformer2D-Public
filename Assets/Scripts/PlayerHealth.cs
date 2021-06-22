using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public GameObject deathVFXPrefab;//死亡特效
    public GameObject deathPose;//角色死亡残影

    int trapsLayer;//Traps的编号

    void Start()
    {
        trapsLayer = LayerMask.NameToLayer("Traps");//记录Traps的Layer编号
    }

    private void OnTriggerEnter2D(Collider2D collision)//2D触发器进入触发器
    {
        if (collision.gameObject.layer == trapsLayer)
        {
            //某项目放至特殊的位置和角度
            Instantiate(deathVFXPrefab, transform.position, transform.rotation);//当前角色位置和旋转角度

            Instantiate(deathPose, transform.position, Quaternion.Euler(0, 0, Random.Range(-45, 90)));//角色死亡残影，z轴范围内随机旋转

            gameObject.SetActive(false);//触碰到traps关闭游戏角色

            AudioManager.PlayDeathAudio();

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//当前激活场景重置

            GameManager.PlayerDied();
        }
    }
}
