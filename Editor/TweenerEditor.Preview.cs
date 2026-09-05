using UnityEditor;
using UnityEngine;

public partial class TweenerEditor : Editor
{
	float easeInterval = 0.033f;
	float easeIntervalTimer = 0f;
	float easeRate;
	float easeSpeed = 0.5f;

	bool isPreviewing = false;
	float previewTime = 0f;

	private void Awake()
	{
		EditorApplication.update += UpdateMethod;
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	private void OnDestroy()
	{
		EditorApplication.update -= UpdateMethod;
		EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
	}

	public void UpdateMethod()
	{
		easeIntervalTimer += Time.fixedDeltaTime;
		if (easeIntervalTimer > easeInterval) {
			easeIntervalTimer -= easeInterval;
			
			easeRate += easeInterval * easeSpeed;
			if (easeRate > 1f)
				easeRate -= 1f;
		}
		
		if (selectedIndex >= 0)
			Repaint();

		if (!isPreviewing)
			return;

		previewTime += instance.speed * Time.fixedDeltaTime;
		if (previewTime > instance.duration) {
			if (instance.looping) {
				previewTime -= instance.duration;
				ResetPreview();
			}
			else {
				previewTime = instance.duration;
				StopPreview();
			}
		}

		EvaluatePreview(previewTime);
	}

	public void OnPlayModeStateChanged(PlayModeStateChange mode)
	{
		if (mode == PlayModeStateChange.EnteredEditMode) {
			if (instance.sequences == null)
				return;

			for (int i = 0; i < instance.sequences.Length; i++) {
				var sequence = instance.sequences[i];
				if (sequence == null || !sequence.isEnable)
					continue;
				sequence.InitModule();
			}

			ResetPreview();
			EvaluatePreview(0f);
		}	
	}

	void StartPreview()
	{
		if (instance.sequences == null)
			return;

		isPreviewing = true;
		previewTime = 0f;
		ResetPreview();

		for (int i = 0; i < instance.sequences.Length; i++) {
			var sequence = instance.sequences[i];
			if (sequence == null || !sequence.isEnable)
				continue;
			sequence.ResetModule();
			sequence.Init();
		}	
	}

	void EvaluatePreview(float targetTime)
	{
		if (instance.sequences == null)
			return;

		for (int i = 0; i < instance.sequences.Length; i++) {
			var sequence = instance.sequences[i];
			if (sequence == null || !sequence.isEnable)
				continue;

			sequence.cachedEvaluate(targetTime);
		}
	}

	void ResetPreview()
	{
		if (instance.sequences == null)
			return;

		for (int i = 0; i < instance.sequences.Length; i++) {
			var sequence = instance.sequences[i];
			if (sequence == null || !sequence.isEnable)
				continue;

			// まず必ず初期状態に戻す
			sequence.ResetModule();
		}

		// その後、previewTime に応じて進める
		EvaluatePreview(previewTime);
	}

	void StopPreview()
	{	
		isPreviewing = false;
		previewTime = 0f;

		ResetPreview();
	}
}
