using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Tweener : MonoBehaviour
{
    [System.Serializable]
    public class TweenData
    {
        public float duration;

        public enum LoopType
        {
            PlayOnce,
            Loop,
            PingPongOnce,
            PingPongLoop
        }
        public LoopType loopType;

        [SerializeField]
        public SequenceData[] sequences;

        public void Init()
        {
            if (sequences != null && sequences.Length != 0) {
                for (int i = 0; i < sequences.Length; i++) {
                    sequences[i].Init();
                    sequences[i].Update(0f);
                }
            }
        }

        public void Update(float curTime)
        {
            if (sequences == null || sequences.Length == 0)
                return;

            for (int i = 0; i < sequences.Length; i++) {
                sequences[i].Update(curTime);
            }
        }

        public void End(bool isPingpong = false)
        {
            if (sequences == null || sequences.Length == 0)
                return;

            for (int i = 0; i < sequences.Length; i++) {
                sequences[i].End(isPingpong);
            }
        }

        public void Restore()
        {
            if (sequences == null || sequences.Length == 0)
                return;

            for (int i = 0; i < sequences.Length; i++) {
                sequences[i].Restore();
            }
        }
    }
}
