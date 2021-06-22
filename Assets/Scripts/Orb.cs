using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    int player;//记录player图层
    public GameObject explosionVFXPrefab;//爆炸效果

    void Start()
    {
        player = LayerMask.NameToLayer("Player");

        GameManager.RegisterOrb(this);//注册orb
    }

    private void OnTriggerEnter2D(Collider2D collision)//2D触发器进入触发器
    {
        if (collision.gameObject.layer == player)
        {
            //某项目放至特殊的位置和角度
            Instantiate(explosionVFXPrefab, transform.position, transform.rotation);

            gameObject.SetActive(false);//取消宝珠显示

            AudioManager.PlayOrbAudio();

            GameManager.PlayerGrabbedOrb(this);//角色获得宝珠
        }
    }

}
