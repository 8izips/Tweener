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
        tweenData.Reset();
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

        switch (tweenData.loopType) {
            case TweenData.LoopType.PlayOnce:
                if (curTime >= tweenData.duration) {
                    tweenData.End();
                    isPlaying = false;

                    completionCallback?.Invoke();
                }
                break;
            case TweenData.LoopType.Loop:
                if (curTime >= tweenData.duration) {
                    curTime -= tweenData.duration;
                    tweenData.Reset();
                }

                break;
        }

        tweenData.Update(curTime);
    }
}
