using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Tweener : MonoBehaviour
{
    public bool playOnAwake;
    public bool ignoreTimeScale;
    public float curTime;
    public bool isPlaying;

    [SerializeField]
    public TweenData tweenData;

    public System.Action completionCallback;

    void Awake()
    {
        if (playOnAwake)
            Play();
    }

    public void Play()
    {
        curTime = 0f;
        isPlaying = true;

        tweenData.Init();
    }

    public void Stop()
    {
        isPlaying = false;
    }

    public void Update()
    {
        if (!isPlaying || (object)tweenData == null)
            return;

        if (ignoreTimeScale)
            curTime += Time.unscaledDeltaTime;
        else
            curTime += Time.deltaTime;
        var playTime = curTime;

        switch (tweenData.loopType) {
            case TweenData.LoopType.PlayOnce:
                if (curTime >= tweenData.duration) {
                    tweenData.End(false);
                    isPlaying = false;

                    completionCallback?.Invoke();
                    return;
                }
                break;
            case TweenData.LoopType.Loop:
                if (curTime >= tweenData.duration) {
                    curTime -= tweenData.duration;
                }
                break;
            case TweenData.LoopType.PingPongOnce:
                if (curTime >= tweenData.duration * 2f) {
                    tweenData.End(true);
                    isPlaying = false;

                    completionCallback?.Invoke();
                    return;
                }
                else if (curTime >= tweenData.duration) {
                    playTime = tweenData.duration * 2f - curTime;
                }
                break;
            case TweenData.LoopType.PingPongLoop:
                if (curTime >= tweenData.duration * 2f) {
                    curTime -= tweenData.duration * 2f;
                    playTime = curTime;
                }
                else if (curTime >= tweenData.duration) {
                    playTime = tweenData.duration * 2f - curTime;
                }
                break;
        }

        tweenData.Update(playTime);
    }
}
