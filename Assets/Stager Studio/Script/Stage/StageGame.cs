﻿namespace StagerStudio.Stage {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using LinerCurve;
	using Saving;
	using Data;
	using Object;


	public class StageGame : MonoBehaviour {




		#region --- SUB ---


		public delegate void VoidHandler ();
		public delegate void VoidFloatHandler (float ratio);
		public delegate void VoidBoolHandler (bool value);
		public delegate void VoidIntBoolBoolHandler (int a, bool b, bool c);
		public delegate string StringStringHandler (string str);


		#endregion




		#region --- VAR ---


		// Handler
		public static StringStringHandler GetLanguage { get; set; } = null;
		public static VoidHandler OnStageObjectChanged { get; set; } = null;
		public static VoidBoolHandler OnUserDynamicSpeedChanged { get; set; } = null;
		public static VoidIntBoolBoolHandler OnAbreastChanged { get; set; } = null;
		public static VoidBoolHandler OnShowGridChanged { get; set; } = null;
		public static VoidFloatHandler OnRatioChanged { get; set; } = null;

		// API
		public float Ratio {
			get => _Ratio;
			set {
				_Ratio = Mathf.Clamp(value, 0.1f, 10f);
				OnRatioChanged(_Ratio);
			}
		}
		public float BPM { get; set; } = 120f;
		public float Shift { get; set; } = 0f;
		public float MapDropSpeed { get; set; } = 1f;
		public float GameDropSpeed { get; private set; } = 1f;
		public bool UseDynamicSpeed { get; private set; } = true;
		public bool UseAbreast { get; private set; } = false;
		public bool AllStageAbreast { get; private set; } = false;
		public int AbreastIndex => Mathf.Max(_AbreastIndex, 0);
		public bool UseGrid => ShowGrid;
		public bool PositiveScroll { get; set; } = true;

		// Short
		private StageProject Project => _Project != null ? _Project : (_Project = FindObjectOfType<StageProject>());
		private StageMusic Music => _Music != null ? _Music : (_Music = FindObjectOfType<StageMusic>());
		private LinerFloat SpeedCurve { get; } = new LinerFloat();
		private Camera Camera => _Camera != null ? _Camera : (_Camera = Camera.main);

		// Ser
		[SerializeField] private StageObject m_Prefab_Stage = null;
		[SerializeField] private StageObject m_Prefab_Track = null;
		[SerializeField] private StageObject m_Prefab_Note = null;
		[SerializeField] private Transform m_Prefab_Speed = null;
		[SerializeField] private Transform m_Prefab_Motion = null;
		[SerializeField] private StageObject m_Prefab_Luminous = null;
		[SerializeField] private Transform[] m_AntiMouseTF = null;
		[SerializeField] private Transform m_Level = null;
		[SerializeField] private RectTransform m_ZoneRT = null;

		// Data
		private StageProject _Project = null;
		private StageMusic _Music = null;
		private Camera _Camera = null;
		private Transform[] Containers = null;
		private float _Ratio = 1.5f;
		private int _AbreastIndex = 0;

		// Saving
		private SavingBool ShowGrid = new SavingBool("StageGame.ShowGrid", true);


		#endregion




#if UNITY_EDITOR
		[Header("Test"), SerializeField] private Beatmap m_TestBeatmap = null;
		public void SetTestBeatmap (Beatmap map) => m_TestBeatmap = map;
		public Beatmap GetTestBeatmap () => m_TestBeatmap;
#endif




		#region --- MSG ---


		private void Awake () {
			// Layer ID
			Stage.LayerID_Stage = SortingLayer.NameToID("Stage");
			Track.LayerID_Track = SortingLayer.NameToID("Track");
			Track.LayerID_Tray = SortingLayer.NameToID("Tray");
			Note.LayerID_Pole = SortingLayer.NameToID("Pole");
			Note.LayerID_Note = SortingLayer.NameToID("Note");
			Note.LayerID_Arrow = SortingLayer.NameToID("Arrow");
			SpeedNote.LayerID_Speed = SortingLayer.NameToID("Speed");
			MotionNote.LayerID_Motion = SortingLayer.NameToID("Motion");
			Luminous.LayerID_Lum = SortingLayer.NameToID("Luminous");
			// Misc
			Containers = new Transform[m_Level.childCount];
			for (int i = 0; i < Containers.Length; i++) {
				Containers[i] = m_Level.GetChild(i);
			}
		}


		private void Start () {
			SetShowGrid(ShowGrid);
		}


		private void Update () {
			BeatmapUpdate();
			CacheUpdate();
			MouseUpdate();
		}


		private void BeatmapUpdate () {
			if (Music.IsPlaying) { return; }
			var map = Project.Beatmap;
			if (!(map is null)) {
				// Has Beatmap
				FixObject(m_Prefab_Stage, null, Containers[0], map.Stages.Count);
				FixObject(m_Prefab_Track, null, Containers[1], map.Tracks.Count);
				FixObject(m_Prefab_Note, null, Containers[2], map.Notes.Count);
				FixObject(null, m_Prefab_Speed, Containers[3], map.SpeedNotes.Count);
				FixStageMotionObject(m_Prefab_Motion, Containers[4].GetChild(0), map.Stages);
				FixTrackMotionObject(m_Prefab_Motion, Containers[4].GetChild(1), map.Tracks);
				FixObject(m_Prefab_Luminous, null, Containers[5], map.Notes.Count);
			} else {
				// No Beatmap
				ClearAllContainers();
			}
			// Func
			void FixObject (StageObject prefab, Transform subPrefab, Transform container, int count) {
				int conCount = container.childCount;
				if (conCount > count) {
					container.FixChildcountImmediately(count);
					OnStageObjectChanged();
				} else if (conCount < count) {
					count -= conCount;
					if (prefab is null && subPrefab is null) {
						// Spawn Container
						for (int i = 0; i < count; i++) {
							var tf = new GameObject("").transform;
							tf.SetParent(container);
							tf.localPosition = Vector3.zero;
						}
					} else if (!(prefab is null)) {
						// Spawn Stage Object
						for (int i = 0; i < count; i++) {
							Instantiate(prefab, container).SetSkinData(StageSkin.Data.Data);
						}
					} else {
						// Spawn Transform Object
						for (int i = 0; i < count; i++) {
							Instantiate(subPrefab, container);
						}
					}
					OnStageObjectChanged();
				}
			}
			void FixStageMotionObject (Transform prefab, Transform container, List<Beatmap.Stage> stages) {
				int count = stages.Count;
				FixObject(null, null, container, count);
				for (int i = 0; i < count; i++) {
					FixObject(null, prefab, container.GetChild(i), stages[i].GetMotionCount());
				}
			}
			void FixTrackMotionObject (Transform prefab, Transform container, List<Beatmap.Track> tracks) {
				int count = tracks.Count;
				FixObject(null, null, container, count);
				for (int i = 0; i < count; i++) {
					FixObject(null, prefab, container.GetChild(i), tracks[i].GetMotionCount());
				}
			}
		}


		private void CacheUpdate () {
			if (Music.IsPlaying) { return; }
			// Speed Curve
			if (Project.Beatmap is null) {
				// No Map
				if (SpeedCurve.Count > 0) {
					SpeedCurve.Clear();
				}
			} else {
				var speedNotes = Project.Beatmap.SpeedNotes;
				if (speedNotes is null || speedNotes.Count == 0) {
					// No SpeedNote
					if (SpeedCurve.Count > 0) {
						SpeedCurve.Clear();
					}
					// Init Speed Note
					if (speedNotes is null) {
						Project.Beatmap.SpeedNotes = new List<Beatmap.SpeedNote>() { new Beatmap.SpeedNote(0, 0, 1), };
					} else {
						speedNotes.Add(new Beatmap.SpeedNote(0, 0, 1));
					}
				} else if (SpeedCurve.Count != speedNotes.Count * 2) {
					// Reset Speed Curve
					SpeedCurve.Clear();
					float value = 1f;
					foreach (var note in speedNotes) {
						float time = note.Time;
						while (SpeedCurve.ContainsKey(time)) { time += float.Epsilon; }
						SpeedCurve.Add(time, value);
						value = note.Speed;
						time += note.Duration;
						while (SpeedCurve.ContainsKey(time)) { time += float.Epsilon; }
						SpeedCurve.Add(time, value);
					}

				}
			}
		}


		private void MouseUpdate () {
			if (Music.IsPlaying) { return; }
			// Wheel
			if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.01f) {
				if (CheckAntiMouse()) {
					if (Input.GetKey(KeyCode.LeftControl)) {
						// Zoom


					} else {
						// Seek
						float delta = Input.mouseScrollDelta.y * (PositiveScroll ? 0.1f : -0.1f);
						if (Input.GetKey(KeyCode.LeftAlt)) {
							delta *= 0.1f;
						} else if (Input.GetKey(KeyCode.LeftShift)) {
							delta *= 10f;
						}
						Music.Seek(Music.Time + delta * (60f / BPM));
					}
				}
			}
			// Func
			bool CheckAntiMouse () {
				// Transform
				foreach (var tf in m_AntiMouseTF) {
					if (tf.gameObject.activeSelf) {
						return false;
					}
				}
				// Hover In Zone
				var pos01 = m_ZoneRT.Get01Position(Input.mousePosition, Camera);
				if (pos01.x < 0 || pos01.x > 1 || pos01.y < 0 || pos01.y > 1) { return false; }
				// Final
				return true;
			}
		}


		#endregion




		#region --- API ---


		public void ClearAllContainers () {
			for (int i = 0; i < Containers.Length; i++) {
				var container = Containers[i];
				if (i == 4) {
					container.GetChild(0).DestroyAllChildImmediately();
					container.GetChild(1).DestroyAllChildImmediately();
				} else {
					container.DestroyAllChildImmediately();
				}
			}
		}


		// Speed Curve
		public void SetSpeedCurveDirty () => SpeedCurve.Clear();


		public float FillDropTime (float time, float fill, float muti) => UseDynamicSpeed ? SpeedCurve.Fill(time, fill, muti) : time + fill / muti;


		public float AreaBetweenDrop (float time, float muti) => UseDynamicSpeed ? SpeedCurve.GetAreaBetween(0, time, muti) : time * muti;


		// Grid
		public float SnapTime (float time, int step) {
			float gap = 60f / BPM / step;
			float offset = Mathf.Repeat(Shift, 60f / BPM);
			return Mathf.Round((time - offset) / gap) * gap + offset;
		}


		// UI
		public void SwitchUseDynamicSpeed () => SetUseDynamicSpeed(!UseDynamicSpeed);


		public void SwitchUseAbreastView () => SetUseAbreastView(!UseAbreast);


		public void SwitchShowGrid () => SetShowGrid(!ShowGrid);


		public void SetUseDynamicSpeed (bool use) {
			UseDynamicSpeed = use;
			OnUserDynamicSpeedChanged(use);
		}


		public void SetUseAbreastView (bool abreast) {
			UseAbreast = abreast;
			OnAbreastChanged(AbreastIndex, UseAbreast, AllStageAbreast);
		}


		public void SetAllStageAbreast (bool abreast) {
			AllStageAbreast = abreast;
			OnAbreastChanged(AbreastIndex, UseAbreast, AllStageAbreast);

		}


		public void SetAbreastIndex (int newIndex) {
			_AbreastIndex = Mathf.Max(newIndex, 0);
			OnAbreastChanged(AbreastIndex, UseAbreast, AllStageAbreast);
		}


		public void SetShowGrid (bool show) {
			ShowGrid.Value = show;
			OnShowGridChanged(ShowGrid);
		}


		#endregion




	}


}


#if UNITY_EDITOR
namespace StagerStudio.Editor {
	using UnityEngine;
	using UnityEditor;
	using Stage;
	[CustomEditor(typeof(StageGame))]
	public class StageGameInspector : Editor {
		private readonly static string[] Exclude = new string[] { "m_TestBeatmap" };
		private void Awake () {
			if (EditorApplication.isPlaying) {
				(target as StageGame).SetTestBeatmap(FindObjectOfType<StageProject>().Beatmap);
			}
		}
		public override void OnInspectorGUI () {
			if (EditorApplication.isPlaying) {
				base.OnInspectorGUI();
				if (GUI.changed) {
					StageGame.OnStageObjectChanged();
				}
			} else {
				serializedObject.Update();
				DrawPropertiesExcluding(serializedObject, Exclude);
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
#endif