using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class Tweener : MonoBehaviour
{
	public int playerId;

	public float speed = 1f;
	public float duration = 2f;
	public bool looping = false;
	public float startDelay;
	public bool randomStartDelay;

	public bool ignoreTimeScale = false;
	public bool playOnAwake = true;

	[SerializeReference]
	public SequenceModule[] sequences;

	public bool isPlaying { get; private set; } = false;
	public bool isPaused { get; private set; } = false;
	public float curTime { get; set; }
	public float waitTime { get; set; }
	private void OnEnable()
	{
		if (!IsValidData())
			return;

		for (int i = 0; i < sequences.Length; i++) {
			var sequence = sequences[i];
			if (sequence == null || !sequence.isEnable)
				continue;
			sequence.InitModule();
		}

		if (playOnAwake)
			Play();
	}

	private void OnDisable()
	{
		if (isPlaying)
			Stop();
	}

	bool IsValidData()
	{
		if (sequences == null || sequences.Length == 0)
			return false;
		return true;
	}

	bool IsValidParameter()
	{
		if (duration <= 0f || speed <= 0f)
			return false;
		return true;
	}

	public void Play()
	{
		if (!IsValidData() || !IsValidParameter())
			return;

		isPlaying = true;
		isPaused = false;
		curTime = 0f;
		waitTime = randomStartDelay ? Random.Range(0f, startDelay) : startDelay;

		ResetSequences();
	}

	public void PlayDelayed(float delay)
	{
		if (!IsValidData() || !IsValidParameter())
			return;

		isPlaying = true;
		isPaused = false;
		curTime = 0f;
		waitTime = delay;

		ResetSequences();
	}

	public void Stop()
	{
		isPlaying = false;
		isPaused = false;
	}

	public void Pause()
	{
		isPaused = true;
	}

	public void Resume()
	{
		if (curTime < duration)
			isPaused = false;
	}

	public void Evaluate(float targetTime)
	{
		if (!IsValidData())
			return;
		curTime = Mathf.Clamp(targetTime, 0f, duration);
		waitTime = 0f;
		UpdateSequences(curTime);
	}

	public void Rewind()
	{
		if (!IsValidData())
			return;

		isPlaying = false;
		isPaused = false;
		curTime = 0f;
		waitTime = 0f;
		
		for (int i = 0; i < sequences.Length; i++) {
			var sequence = sequences[i];
			if (sequence == null || !sequence.isEnable)
				continue;

			sequence.ResetModule();
		}
	}

	public void Complete()
	{
		if (!IsValidData())
			return;
		isPlaying = false;
		isPaused = false;
		curTime = duration;
		waitTime = 0f;
		for (int i = 0; i < sequences.Length; i++) {
			var sequence = sequences[i];
			if (sequence == null || !sequence.isEnable)
				continue;
			sequence.cachedEvaluate(curTime);
		}
	}

	void Update()
	{
		if (!isPlaying || isPaused)
			return;

		var dt = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

		// start delay
		if (waitTime > 0f) {
			waitTime -= dt;
			if (waitTime > 0f)
				return;

			dt = -waitTime;
			waitTime = 0f;
		}	

		curTime += speed * dt;		
		if (curTime >= duration) {
			if (looping) {
				curTime %= duration;
				ResetSequences();
			}
			else {
				curTime = duration;
				isPlaying = false;
			}
		}

		UpdateSequences(curTime);
	}

	void ResetSequences()
	{
		if (!IsValidData())
			return;

		for (int i = 0; i < sequences.Length; i++) {
			var sequence = sequences[i];
			if (sequence == null || !sequence.isEnable)
				continue;

			sequence.ResetModule();
			sequence.Init();
		}
	}

	void UpdateSequences(float targetTime)
	{
		if (!IsValidData())
			return;

		for (int i = 0; i < sequences.Length; i++) {
			var sequence = sequences[i];
			if (sequence == null || !sequence.isEnable)
				continue;

			sequence.cachedEvaluate(targetTime);
		}
	}
}
