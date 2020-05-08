﻿namespace StagerStudio.UI {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;


	public class NoteInspectorUI : MonoBehaviour {

		// Api
		public InputField TimeIF => m_TimeIF;
		public InputField BeatIF => m_BeatIF;
		public InputField TypeIF => m_TypeIF;
		public InputField DurationIF => m_DurationIF;
		public InputField PosXIF => m_PosXIF;
		public InputField WidthIF => m_WidthIF;
		public InputField IndexIF => m_IndexIF;
		public InputField LinkIF => m_LinkIF;
		public InputField ClickIF => m_ClickIF;
		public InputField PosZIF => m_PosZIF;
		public InputField SfxIF => m_SfxIF;
		public InputField SfxParamAIF => m_SfxParamAIF;
		public InputField SfxParamBIF => m_SfxParamBIF;
		public Text[] LanguageLabels => m_LanguageLabels;

		// Ser
		[SerializeField] private InputField m_TimeIF = null;
		[SerializeField] private InputField m_BeatIF = null;
		[SerializeField] private InputField m_TypeIF = null;
		[SerializeField] private InputField m_DurationIF = null;
		[SerializeField] private InputField m_PosXIF = null;
		[SerializeField] private InputField m_WidthIF = null;
		[SerializeField] private InputField m_IndexIF = null;
		[SerializeField] private InputField m_LinkIF = null;
		[SerializeField] private InputField m_PosZIF = null;
		[SerializeField] private InputField m_ClickIF = null;
		[SerializeField] private InputField m_SfxIF = null;
		[SerializeField] private InputField m_SfxParamAIF = null;
		[SerializeField] private InputField m_SfxParamBIF = null;
		[SerializeField] private Text[] m_LanguageLabels = null;


		// API
		public float GetTime () => m_TimeIF.text.TryParseFloatForInspector(out float result) ? Mathf.Max(result, 0f) : 0f;
		public float GetBeat () => m_BeatIF.text.TryParseFloatForInspector(out float result) ? result : 0f;
		public int GetItemType () => m_TypeIF.text.TryParseIntForInspector(out int result) ? Mathf.Max(result, 0) : 0;
		public float GetDuration () => m_DurationIF.text.TryParseFloatForInspector(out float result) ? Mathf.Max(result, 0f) : 0f;
		public float GetPosX () => m_PosXIF.text.TryParseFloatForInspector(out float result) ? result : 0f;
		public float GetWidth () => m_WidthIF.text.TryParseFloatForInspector(out float result) ? Mathf.Max(result, 0f) : 0f;
		public int GetIndex () => m_IndexIF.text.TryParseIntForInspector(out int result) ? Mathf.Max(result, 0) : 0;
		public int GetLink () => m_LinkIF.text.TryParseIntForInspector(out int result) ? Mathf.Max(result, -1) : -1;
		public float GetPosZ () => m_PosZIF.text.TryParseFloatForInspector(out float result) ? result : 0;
		public short GetClick () => short.TryParse(m_ClickIF.text, out short result) ? (short)Mathf.Max(result, -1) : (short)0;
		public byte GetSfx () => byte.TryParse(m_SfxIF.text, out byte result) ? (byte)Mathf.Max(result, 0) : (byte)0;
		public int GetSfxParamA () => m_SfxParamAIF.text.TryParseIntForInspector(out int result) ? Mathf.Max(result, 0) : 0;
		public int GetSfxParamB () => m_SfxParamBIF.text.TryParseIntForInspector(out int result) ? Mathf.Max(result, 0) : 0;


		public void SetTime (float value) => m_TimeIF.text = value.ToString();
		public void SetBeat (float value) => m_BeatIF.text = value.ToString();
		public void SetItemType (int value) => m_TypeIF.text = value.ToString();
		public void SetDuration (float value) => m_DurationIF.text = value.ToString();
		public void SetPosX (float value) => m_PosXIF.text = value.ToString();
		public void SetWidth (float value) => m_WidthIF.text = value.ToString();
		public void SetIndex (int value) => m_IndexIF.text = value.ToString();
		public void SetLink (int value) => m_LinkIF.text = value.ToString();
		public void SetPosZ (float value) => m_PosZIF.text = value.ToString();
		public void SetClick (short value) => m_ClickIF.text = value.ToString();
		public void SetSfx (byte value) => m_SfxIF.text = value.ToString();
		public void SetSfxParamA (int value) => m_SfxParamAIF.text = value.ToString();
		public void SetSfxParamB (int value) => m_SfxParamBIF.text = value.ToString();


	}
}