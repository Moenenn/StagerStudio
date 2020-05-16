﻿namespace StagerStudio.UI {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;


	public class BeatInputUI : MonoBehaviour {



		// Const
		private static readonly float[] BEAT_DIVs = { 4f, 8f, 16f, 32f, };

		// Handler
		public delegate void VoidHandler ();
		public VoidHandler OnEndEdit { get; set; } = null;

		// Ser
		[SerializeField] private InputField m_BeatIF = null;
		[SerializeField] private InputField m_BeatAmountIF = null;
		[SerializeField] private Button m_BeatDiv = null;
		[SerializeField] private Image m_BeatDivIcon = null;
		[SerializeField] private Sprite[] m_DivSPs = null;

		// Data
		private bool UIReady = true;
		private int DiVIndex = 1;
		private float Beat = 0f;


		// MSG
		private void Awake () {
			m_BeatIF.onEndEdit.AddListener((_) => {
				if (!UIReady) { return; }
				Beat = GetBeatFromUI();
				RefreshUI();
				OnEndEdit?.Invoke();
			});
			m_BeatAmountIF.onEndEdit.AddListener((_) => {
				if (!UIReady) { return; }
				Beat = GetBeatFromUI();
				OnEndEdit?.Invoke();
				RefreshUI();
			});
			m_BeatDiv.onClick.AddListener(() => {
				if (!UIReady) { return; }
				float beat = GetBeat();
				DiVIndex = (DiVIndex + 1) % m_DivSPs.Length;
				RefreshUI(beat);
			});
			RefreshUI();
		}


		// API
		public void SetBeatToUI (float beat) {
			Beat = beat;
			int beatCount = Mathf.FloorToInt(beat);
			int beatAmount = Mathf.RoundToInt(Mathf.Repeat(beat, 1f) * BEAT_DIVs[DiVIndex]);
			m_BeatIF.text = beatCount.ToString();
			m_BeatAmountIF.text = beatAmount.ToString();
		}


		public float GetBeat () => Beat;




		// LGC
		private void RefreshUI () => RefreshUI(GetBeat());
		private void RefreshUI (float beat) {
			UIReady = false;
			try {
				m_BeatDivIcon.sprite = m_DivSPs[DiVIndex];
				SetBeatToUI(beat);
			} catch { }
			UIReady = true;
		}


		private float GetBeatFromUI () {
			int beatCount = 0;
			int beatAmount = 0;
			if (m_BeatIF.text.TryParseIntForInspector(out int _beatCount)) {
				beatCount = _beatCount;
			}
			if (m_BeatAmountIF.text.TryParseIntForInspector(out int _beatAmount)) {
				beatAmount = (int)Mathf.Repeat(_beatAmount, BEAT_DIVs[DiVIndex]);
			}
			return beatCount + beatAmount / BEAT_DIVs[DiVIndex];
		}


	}
}