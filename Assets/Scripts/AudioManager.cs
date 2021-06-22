using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager current;//当前静态实例，可被调用

    [Header("Environment Audio")]
    public AudioClip ambientClip;//环境音效
    public AudioClip musicClip;//背景音乐

    [Header("FX Audio")]
    public AudioClip deathFXClip;//死亡音效
    public AudioClip orbFXClip;//宝珠音效
    public AudioClip doorFXClip;//门声音
    public AudioClip startLevelClip;//进入游戏声音
    public AudioClip winClip;//胜利声音

    [Header("Robbie Audio")]
    public AudioClip[] walkStepClips;//走路声音随机片段
    public AudioClip[] crouchStepClips;//下蹲声音随机片段
    public AudioClip jumpClip;//跳跃音效
    public AudioClip deathClip;//角色死亡音效

    public AudioClip jumpVoiceClip;//跳跃人声
    public AudioClip deathVoiceClip;//死亡人声
    public AudioClip orbVoiceClip;//获得宝珠语音

    //存放AudioSource片段
    AudioSource ambientSource;//环境音效片段
    AudioSource musicSource;//背景音乐片段
    AudioSource fxSource;//特效片段
    AudioSource playerSource;//角色片段
    AudioSource voiceSource;//声音片段

    public AudioMixerGroup ambientGroup, musicGroup, FXGroup, playerGroup, voiceGroup;

    private void Awake()
    {
        if (current != null)
        {
            Destroy(gameObject);//把第二个以上的都销毁，避免重复
            return;
        }
        current = this;

        DontDestroyOnLoad(gameObject);//重新加载时保留

        //添加相应组件
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playerSource = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();

        //音源输出到AudioMixerGroup
        ambientSource.outputAudioMixerGroup = ambientGroup;
        musicSource.outputAudioMixerGroup = ambientGroup;
        playerSource.outputAudioMixerGroup = playerGroup;
        fxSource.outputAudioMixerGroup = FXGroup;
        voiceSource.outputAudioMixerGroup = voiceGroup;

        StartLevelAudio();
    }

    void StartLevelAudio()//场景启动音效
    {
        //环境音效
        current.ambientSource.clip = current.ambientClip;//指定clip片段
        current.ambientSource.loop = true;//开启循环播放
        current.ambientSource.Play();

        //背景音乐
        current.musicSource.clip = current.musicClip;
        current.musicSource.loop = true;
        current.musicSource.Play();

        //角色进入场景音乐
        current.fxSource.clip = current.startLevelClip;
        current.fxSource.Play();
    }

    public static void PlayerWonAudio()//角色胜利声音
    {
        current.fxSource.clip = current.winClip;
        current.fxSource.Play();
        current.playerSource.Stop();//获胜时禁止其他角色音源
    }

    public static void PlayDoorOpenAudio()//播放打开门的声音
    {
        current.fxSource.clip = current.doorFXClip;
        current.fxSource.PlayDelayed(1f);//门打开有延迟效果
    }

    public static void PlayFootstepAudio()//脚步声。静态保持不变
    {
        int index = Random.Range(0, current.walkStepClips.Length);//随机播放声音片段，数组0开始

        current.playerSource.clip = current.walkStepClips[index];//随机输送Clip
        current.playerSource.Play();
    }

    public static void PlayCrouchFootstepAudio()//下蹲声
    {
        int index = Random.Range(0, current.crouchStepClips.Length);

        current.playerSource.clip = current.crouchStepClips[index];
        current.playerSource.Play();
    }

    public static void PlayJumpAudio()//跳跃声音和跳跃人声
    {
        current.playerSource.clip = current.jumpClip;
        current.playerSource.Play();

        current.voiceSource.clip = current.jumpVoiceClip;
        current.voiceSource.Play();
    }

    public static void PlayDeathAudio()//角色死亡声音
    {
        current.playerSource.clip = current.deathClip;
        current.playerSource.Play();

        current.voiceSource.clip = current.deathVoiceClip;
        current.voiceSource.Play();

        current.fxSource.clip = current.deathFXClip;//归还宝珠
        current.fxSource.Play();
    }

    public static void PlayOrbAudio()//获得宝珠
    {
        current.fxSource.clip = current.orbFXClip;
        current.fxSource.Play();

        current.voiceSource.clip = current.orbVoiceClip;
        current.voiceSource.Play();
    }
}
