﻿namespace StagerStudio.Data {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;




	[System.Serializable]
	public class Beatmap {



		// Comparer
		public class StageComparer : IComparer<Stage> {
			public int Compare (Stage x, Stage y) => x.Time.CompareTo(y.Time);
		}
		public class TrackComparer : IComparer<Track> {
			public int Compare (Track x, Track y) => x.Time.CompareTo(y.Time);
		}
		public class NoteComparer : IComparer<Note> {
			public int Compare (Note x, Note y) => x.Time.CompareTo(y.Time);
		}
		public class SpeedNoteComparer : IComparer<Timing> {
			public int Compare (Timing x, Timing y) => x.Time.CompareTo(y.Time);
		}


		// SUB
		[System.Serializable]
		public struct TimeFloatTween {

			public float Time {
				get => m_Time / 1000f;
				set {
					m_Time = Mathf.RoundToInt(value * 1000f);
				}
			}
			public float Value {
				get => m_Value / 1000f;
				set {
					m_Value = Mathf.RoundToInt(value * 1000f);
				}
			}
			public int Tween {
				get => m_Tween;
				set {
					m_Tween = value;
				}
			}

			public int m_Time;
			public int m_Value;
			public int m_Tween;

			public static int Search (List<TimeFloatTween> data, float time) {
				int start = 0;
				int end = data.Count - 1;
				int mid;
				while (start <= end) {
					mid = (start + end) / 2;
					if (data[mid].Time < time) {
						start = mid + 1;
					} else if (data[mid].Time > time) {
						end = mid - 1;
					} else {
						return mid;
					}
				}
				return (start + end) / 2;
			}

		}


		[System.Serializable]
		public struct TimeIntTween {

			public float Time {
				get => m_Time / 1000f;
				set {
					m_Time = Mathf.RoundToInt(value * 1000f);
				}
			}
			public int Value {
				get => m_Value;
				set => m_Value = value;
			}
			public int Tween {
				get => m_Tween;
				set {
					m_Tween = value;
				}
			}

			public int m_Time;
			public int m_Value;
			public int m_Tween;

			public static int Search (List<TimeIntTween> data, float time) {
				int start = 0;
				int end = data.Count - 1;
				int mid;
				while (start <= end) {
					mid = (start + end) / 2;
					if (data[mid].Time < time) {
						start = mid + 1;
					} else if (data[mid].Time > time) {
						end = mid - 1;
					} else {
						return mid;
					}
				}
				return (start + end) / 2;
			}

		}


		[System.Serializable]
		public struct TimeFloatFloatTween {

			public float Time {
				get => m_Time / 1000f;
				set {
					m_Time = Mathf.RoundToInt(value * 1000f);
				}
			}
			public float A {
				get => m_A / 1000f;
				set {
					m_A = Mathf.RoundToInt(value * 1000f);
				}
			}
			public float B {
				get => m_B / 1000f;
				set {
					m_B = Mathf.RoundToInt(value * 1000f);
				}
			}
			public int Tween {
				get => m_Tween;
				set {
					m_Tween = (byte)value;
				}
			}

			public int m_Time;
			public int m_A;
			public int m_B;
			public byte m_Tween;

			public static int Search (List<TimeFloatFloatTween> data, float time) {
				int start = 0;
				int end = data.Count - 1;
				int mid;
				while (start <= end) {
					mid = (start + end) / 2;
					if (data[mid].Time < time) {
						start = mid + 1;
					} else if (data[mid].Time > time) {
						end = mid - 1;
					} else {
						return mid;
					}
				}
				return (start + end) / 2;
			}

		}


		[System.Serializable]
		public class MapItem {


			// Api
			public float Time {
				get => m_Time / 1000f;
				set => m_Time = Mathf.RoundToInt(value * 1000f);
			}
			public float Duration {
				get => m_Duration / 1000f;
				set => m_Duration = Mathf.RoundToInt(Mathf.Clamp(value, 0f, int.MaxValue / 1000f - 1f) * 1000f);
			}
			public float X {
				get => m_X / 1000f;
				set => m_X = Mathf.RoundToInt(value * 1000f);
			}

			// Api_Ser
			public int ItemType = 0;
			public int m_Time = 0;
			public int m_Duration = 0;
			public int m_X = 0;

			// Cache
			[System.NonSerialized] public bool Active = false;
			[System.NonSerialized] public bool TimerActive = false;
			[System.NonSerialized] public float SpeedMuti = 1f;


		}




		[System.Serializable]
		public class Stage : MapItem {


			// API
			public float Speed {
				get => m_Speed / 1000f;
				set => m_Speed = Mathf.RoundToInt(value * 1000f);
			}

			public float Y {
				get => m_Y / 1000f;
				set => m_Y = Mathf.RoundToInt(value * 1000f);
			}
			public float Width {
				get => m_Width / 1000f;
				set => m_Width = Mathf.RoundToInt(value * 1000f);
			}
			public float Height {
				get => m_Height / 1000f;
				set => m_Height = Mathf.RoundToInt(value * 1000f);
			}
			public float Rotation {
				get => m_Rotation;
				set => m_Rotation = Mathf.RoundToInt(value);
			}
			public float PivotY {
				get => m_PivotY / 1000f;
				set => m_PivotY = Mathf.RoundToInt(value * 1000f);
			}

			// SER-API
			public int m_Speed = 1000;
			public int m_Y = 0;
			public int m_Width = 1000;
			public int m_Height = 1000;
			public int m_Rotation = 0;
			public int m_PivotY = 0;
			public int Color = 0;
			public List<TimeFloatFloatTween> Positions;
			public List<TimeFloatTween> Rotations;
			public List<TimeFloatTween> Widths;
			public List<TimeFloatTween> Heights;
			public List<TimeIntTween> Colors;

			// Cache
			[System.NonSerialized] public int TrackCount = 0;


		}



		[System.Serializable]
		public class Track : MapItem {



			// API
			public float Width {
				get => m_Width / 1000f;
				set => m_Width = Mathf.RoundToInt(value * 1000f);
			}
			public float Angle {
				get => m_Angle;
				set => m_Angle = Mathf.RoundToInt(value);
			}

			// API - Ser
			public int m_Width = 1000;
			public int m_Angle = 0;
			public int StageIndex = -1;
			public int Color = 0;
			public bool HasTray = false;
			public List<TimeFloatTween> Xs;
			public List<TimeFloatTween> Angles;
			public List<TimeFloatTween> Widths;
			public List<TimeIntTween> Colors;

			// Cache
			[System.NonSerialized] public (float min, float max) TrayX = (0.5f, 0.5f);
			[System.NonSerialized] public float TrayTime = float.MaxValue;
			[System.NonSerialized] public Color Tint = UnityEngine.Color.white;

		}



		[System.Serializable]
		public class Note : MapItem {

			// API
			public float Z {
				get => m_Z / 1000f;
				set => m_Z = Mathf.RoundToInt(value * 1000f);
			}
			public float Width {
				get => m_Width / 1000f;
				set => m_Width = Mathf.RoundToInt(value * 1000f);
			}

			// SER-API
			public int m_Z = 0;
			public int m_Width = 1000;
			public int TrackIndex = -1;
			public int LinkedNoteIndex = -1;
			public short ClickSoundIndex = -1;
			public byte SoundFxIndex = 0;
			public int SoundFxParamA = 0;
			public int SoundFxParamB = 0;

			// Cache
			[System.NonSerialized] public float AppearTime = 0f;
			[System.NonSerialized] public float SpeedOnDrop = 1f;
			[System.NonSerialized] public float NoteDropStart = -1f;
			[System.NonSerialized] public float NoteDropEnd = -1f;
			[System.NonSerialized] public float CacheTime = -1f;
			[System.NonSerialized] public float CacheDuration = -1f;
			[System.NonSerialized] public static int CacheDirtyID = 1;
			[System.NonSerialized] public int LocalCacheDirtyID = 0;

		}



		[System.Serializable]
		public class Timing : MapItem {

			// API
			public float Speed {
				get => m_X / 100f;
				set => m_X = Mathf.RoundToInt(value * 100f);
			}

			// SER
			public byte SoundFxIndex = 0;
			public int SoundFxParamA = 0;
			public int SoundFxParamB = 0;

			// Cache
			[System.NonSerialized] public float AppearTime = -1f;
			[System.NonSerialized] public float NoteDropPos = -1f;
			[System.NonSerialized] public static int CacheDirtyID = 1;
			[System.NonSerialized] public int LocalCacheDirtyID = 0;
			[System.NonSerialized] public float CacheTime = -1f;

			// API
			public Timing (float time, float speed) {
				Time = time;
				Speed = speed;
			}

		}


		// API-SER
		public float Shift {
			get => m_Shift / 1000f;
			set {
				m_Shift = Mathf.RoundToInt(value * 1000f);
			}
		}
		public float Ratio {
			get => m_Ratio / 1000f;
			set {
				m_Ratio = Mathf.RoundToInt(value * 1000f);
			}
		}

		public int BPM = 120;
		public int Level = 1;
		public string Tag = "Normal";
		public long CreatedTime = 0;
		public List<Stage> Stages = new List<Stage>();
		public List<Track> Tracks = new List<Track>();
		public List<Note> Notes = new List<Note>();
		public List<Timing> Timings = new List<Timing>();

		// SER
		public int m_Shift = 0;
		public int m_Ratio = 1500;

		// Beatmap
		public static Beatmap NewBeatmap () {
			var map = new Beatmap() {
				CreatedTime = System.DateTime.Now.Ticks,
			};
			map.FixEmpty();
			return map;
		}


		public void FixEmpty () {
			if (Stages is null) {
				Stages = new List<Stage>();
			}
			if (Tracks is null) {
				Tracks = new List<Track>();
			}
			if (Notes is null) {
				Notes = new List<Note>();
			}
			if (Timings is null) {
				Timings = new List<Timing>();
			}
		}


		public void LoadFromOtherMap (Beatmap map, bool loadCreatedTime = true) {
			if (map == null) { return; }
			Tag = map.Tag;
			Level = map.Level;
			BPM = map.BPM;
			Shift = map.Shift;
			Ratio = map.Ratio;
			if (loadCreatedTime) {
				CreatedTime = map.CreatedTime;
			}
			Stages = map.Stages;
			Tracks = map.Tracks;
			Notes = map.Notes;
			Timings = map.Timings;
		}


		public void SortNotesByTime () {
			// OldID / Note Map
			var oldID_Note = new Dictionary<int, Note>();
			int noteCount = Notes.Count;
			for (int i = 0; i < noteCount; i++) {
				oldID_Note.Add(i, Notes[i]);
			}
			// Sort
			Notes.Sort(new NoteComparer());
			// Note / NewID Map
			var note_NewID = new Dictionary<Note, int>();
			for (int i = 0; i < noteCount; i++) {
				var note = Notes[i];
				if (note_NewID.ContainsKey(note)) { continue; }
				note_NewID.Add(note, i);
			}
			for (int i = 0; i < noteCount; i++) {
				var note = Notes[i];
				if (!oldID_Note.ContainsKey(note.LinkedNoteIndex)) { continue; }
				var linkedNote = oldID_Note[note.LinkedNoteIndex];
				if (!note_NewID.ContainsKey(linkedNote)) { continue; }
				note.LinkedNoteIndex = note_NewID[linkedNote];
			}
		}


		// Get
		public MapItem GetItem (int type, int index) {
			switch (type) {
				case 0:
					return index >= 0 && index < Stages.Count ? Stages[index] : null;
				case 1:
					return index >= 0 && index < Tracks.Count ? Tracks[index] : null;
				case 2:
					return index >= 0 && index < Notes.Count ? Notes[index] : null;
				case 3:
					return index >= 0 && index < Timings.Count ? Timings[index] : null;
			}
			return null;
		}
		public bool GetActive (int type, int index) {
			if (type == 4 || type == 5) {
				// Timer
				var item = GetItem(type - 4, index);
				return item != null ? item.TimerActive : false;
			} else {
				// Item
				var item = GetItem(type, index);
				return item != null ? item.Active : false;
			}
		}
		public float GetTime (int type, int index) {
			var item = GetItem(type, index);
			return item != null ? item.Time : 0f;
		}
		public float GetDuration (int type, int index) {
			var item = GetItem(type, index);
			return item != null ? item.Duration : 0f;
		}
		public int GetParentIndex (int type, int index) {
			switch (type) {
				case 1:
					if (index >= 0 && index < Tracks.Count) {
						return Tracks[index].StageIndex;
					}
					break;
				case 2:
					if (index >= 0 && index < Notes.Count) {
						return Notes[index].TrackIndex;
					}
					break;
			}
			return -1;
		}


		// Set
		public void SetX (int type, int index, float x) {
			var item = GetItem(type, index);
			if (item != null) {
				item.X = x;
			}
		}
		public void SetTime (int type, int index, float time) {
			var item = GetItem(type, index);
			if (item != null) {
				item.Time = time;
			}
		}
		public void SetItemType (int type, int index, int itemType) {
			var item = GetItem(type, index);
			if (item != null) {
				item.ItemType = itemType;
			}
		}
		public void SetItemIndex (int type, int index, int newIndex) {
			switch (type) {
				case 0: // Stage
					if (index >= 0 && index < Stages.Count && newIndex >= 0 && newIndex < Stages.Count) {
						var temp = Stages[index];
						Stages[index] = Stages[newIndex];
						Stages[newIndex] = temp;
						for (int i = 0; i < Tracks.Count; i++) {
							var track = Tracks[i];
							if (track.StageIndex == index) {
								track.StageIndex = newIndex;
							} else if (track.StageIndex == newIndex) {
								track.StageIndex = index;
							}
						}
					}
					break;
				case 1: // Track
					if (index >= 0 && index < Tracks.Count && newIndex >= 0 && newIndex < Tracks.Count) {
						var temp = Tracks[index];
						Tracks[index] = Tracks[newIndex];
						Tracks[newIndex] = temp;
						for (int i = 0; i < Notes.Count; i++) {
							var note = Notes[i];
							if (note.TrackIndex == index) {
								note.TrackIndex = newIndex;
							} else if (note.TrackIndex == newIndex) {
								note.TrackIndex = index;
							}
						}
					}
					break;
				case 2: // Note
					if (index >= 0 && index < Notes.Count && newIndex >= 0 && newIndex < Notes.Count) {
						var temp = Notes[index];
						Notes[index] = Notes[newIndex];
						Notes[newIndex] = temp;
						for (int i = 0; i < Notes.Count; i++) {
							var note = Notes[i];
							if (note.LinkedNoteIndex == index) {
								note.LinkedNoteIndex = newIndex;
							} else if (note.LinkedNoteIndex == newIndex) {
								note.LinkedNoteIndex = index;
							}
						}
					}
					break;
				case 3: // Timing
					if (index >= 0 && index < Timings.Count && newIndex >= 0 && newIndex < Timings.Count) {
						var temp = Timings[index];
						Timings[index] = Timings[newIndex];
						Timings[newIndex] = temp;
					}
					break;
			}




		}
		public void SetDuration (int type, int index, float duration) {
			var item = GetItem(type, index);
			if (item != null) {
				item.Duration = duration;
			}
		}
		public float GetSpeedMuti (int type, int index) {
			var item = GetItem(type, index);
			return item != null ? item.SpeedMuti : 1f;
		}

		public void SetStageY (int index, float y) {
			if (index >= 0 && index < Stages.Count) {
				Stages[index].Y = y;
			}
		}
		public void SetStageSpeed (int index, float speed) {
			if (index >= 0 && index < Stages.Count) {
				Stages[index].Speed = speed;
			}
		}
		public void SetStagePivot (int index, float pivot) {
			if (index >= 0 && index < Stages.Count) {
				Stages[index].PivotY = pivot;
			}
		}
		public void SetStageRotation (int index, float rot) {
			if (index >= 0 && index < Stages.Count) {
				Stages[index].Rotation = rot;
			}
		}
		public void SetStageWidth (int index, float width) {
			if (index >= 0 && index < Stages.Count) {
				Stages[index].Width = width;
			}
		}
		public void SetStageHeight (int index, float height) {
			if (index >= 0 && index < Stages.Count) {
				Stages[index].Height = height;
			}
		}
		public void SetStageColor (int index, int color) {
			if (index >= 0 && index < Stages.Count) {
				Stages[index].Color = color;
			}
		}

		public void SetTrackWidth (int index, float width) {
			if (index >= 0 && index < Tracks.Count) {
				Tracks[index].Width = width;
			}
		}
		public void SetTrackAngle (int index, float angle) {
			if (index >= 0 && index < Tracks.Count) {
				Tracks[index].Angle = angle;
			}
		}
		public void SetTrackColor (int index, int color) {
			if (index >= 0 && index < Tracks.Count) {
				Tracks[index].Color = color;
			}
		}
		public void SetTrackTray (int index, bool tray) {
			if (index >= 0 && index < Tracks.Count) {
				Tracks[index].HasTray = tray;
			}
		}
		public void SetTrackStageIndex (int index, int stageIndexForTrack) {
			if (index >= 0 && index < Tracks.Count) {
				Tracks[index].StageIndex = stageIndexForTrack;
			}
		}

		public void SetNoteWidth (int index, float width) {
			if (index >= 0 && index < Notes.Count) {
				Notes[index].Width = width;
			}
		}
		public void SetNoteTrackIndex (int index, int trackIndexForNote) {
			if (index >= 0 && index < Notes.Count) {
				Notes[index].TrackIndex = trackIndexForNote;
			}
		}
		public void SetNoteLinkedIndex (int index, int linkedIndex) {
			if (index >= 0 && index < Notes.Count) {
				Notes[index].LinkedNoteIndex = linkedIndex;
			}
		}
		public void SetNoteZ (int index, float z) {
			if (index >= 0 && index < Notes.Count) {
				Notes[index].Z = z;
			}
		}
		public void SetNoteClickIndex (int index, short click) {
			if (index >= 0 && index < Notes.Count) {
				Notes[index].ClickSoundIndex = click;
			}
		}
		public void SetNoteSfxIndex (int index, byte sfx) {
			if (index >= 0 && index < Notes.Count) {
				Notes[index].SoundFxIndex = sfx;
			}
		}
		public void SetNoteParamA (int index, int param) {
			if (index >= 0 && index < Notes.Count) {
				Notes[index].SoundFxParamA = param;
			}
		}
		public void SetNoteParamB (int index, int param) {
			if (index >= 0 && index < Notes.Count) {
				Notes[index].SoundFxParamB = param;
			}
		}

		public void SetTimingSpeed (int index, int speed) {
			if (index >= 0 && index < Timings.Count) {
				Timings[index].m_X = speed;
			}
		}
		public void SetTimingSfxIndex (int index, byte sfx) {
			if (index >= 0 && index < Timings.Count) {
				Timings[index].SoundFxIndex = sfx;
			}
		}
		public void SetTimingParamA (int index, int param) {
			if (index >= 0 && index < Timings.Count) {
				Timings[index].SoundFxParamA = param;
			}
		}
		public void SetTimingParamB (int index, int param) {
			if (index >= 0 && index < Timings.Count) {
				Timings[index].SoundFxParamB = param;
			}
		}

		// Add
		public void AddStage (
			float time, float duration,
			float x = 0f, float y = 0f,
			float width = 1f, float height = 1f,
			int itemType = 0, float pivotY = 0f,
			float rotation = 0f, float speed = 1f,
			int color = 0
		) => Stages.Add(new Stage() {
			Time = time,
			Duration = duration,
			X = x,
			Y = y,
			Width = width,
			Height = height,
			ItemType = itemType,
			PivotY = pivotY,
			Rotation = rotation,
			Speed = speed,
			Color = color,
			Rotations = { },
			Widths = { },
			Heights = { },
			Colors = { },
			Positions = { },
		});


		public void AddTrack (
			int stageIndex, float time, float duration,
			float x = 0f, float width = 0f, float angle = 0f,
			int color = 0, int itemType = 0,
			bool hasTray = false
		) => Tracks.Add(new Track() {
			StageIndex = stageIndex,
			Time = time,
			Duration = duration,
			X = x,
			Width = width,
			Angle = angle,
			Color = color,
			ItemType = itemType,
			Widths = { },
			Xs = { },
			HasTray = hasTray,
			Colors = { },
			Angles = { },
		});


		public void AddNote (
			int trackIndex, float time, float duration,
			float x = 0f, float width = 0f,
			int linkedNoteIndex = -1, int itemType = 0,
			float z = 0f, byte clickSoundIndex = 0
		) => Notes.Add(new Note() {
			TrackIndex = trackIndex,
			Time = time,
			Duration = duration,
			X = x,
			Z = z,
			Width = width,
			ItemType = itemType,
			ClickSoundIndex = clickSoundIndex,
			LinkedNoteIndex = linkedNoteIndex,
			SoundFxIndex = 0,
			SoundFxParamA = 0,
			SoundFxParamB = 0,
		});


		public void AddTiming (
			float time, float speed, float duration = 0f
		) => Timings.Add(new Timing(time, speed) {
			Duration = duration,
			ItemType = 0,
			SoundFxIndex = 0,
			SoundFxParamA = 0,
			SoundFxParamB = 0,
		});


		// Motion
		private IList GetMotionList (int itemType, int itemIndex, int motionType) {
			switch (itemType) {
				case 0:
					if (itemIndex < 0 || itemIndex >= Stages.Count) { return null; }
					var stage = Stages[itemIndex];
					switch (motionType) {
						case 0:
							return stage.Positions;
						case 1:
							return stage.Rotations;
						case 2:
							return stage.Widths;
						case 3:
							return stage.Heights;
						case 4:
							return stage.Colors;
						default:
							return null;
					}
				case 1:
					if (itemIndex < 0 || itemIndex >= Tracks.Count) { return null; }
					var track = Tracks[itemIndex];
					switch (motionType) {
						case 0:
							return track.Xs;
						case 1:
							return track.Angles;
						case 2:
							return track.Widths;
						case 3:
							return track.Colors;
						default:
							return null;
					}
				default:
					return null;
			}
		}


		public int GetMotionCount (int itemType, int itemIndex, int motionType) {
			var list = GetMotionList(itemType, itemIndex, motionType);
			return list != null ? list.Count : 0;
		}


		private object GetMotionItem (int itemType, int itemIndex, int motionType, int motionIndex) {
			var list = GetMotionList(itemType, itemIndex, motionType);
			return list != null && motionIndex >= 0 && motionIndex < list.Count ? list[motionIndex] : null;
		}


		private void SetMotionItem (int itemType, int itemIndex, int motionType, int motionIndex, object item) {
			var list = GetMotionList(itemType, itemIndex, motionType);
			if (list != null && motionIndex >= 0 && motionIndex < list.Count) {
				list[motionIndex] = item;
			}
		}


		public float? GetMotionTime (int itemType, int itemIndex, int motionType, int motionIndex) {
			var item = GetMotionItem(itemType, itemIndex, motionType, motionIndex);
			if (item == null) { return null; }
			if (item is TimeIntTween) {
				return ((TimeIntTween)item).Time;
			} else if (item is TimeFloatTween) {
				return ((TimeFloatTween)item).Time;
			} else if (item is TimeFloatFloatTween) {
				return ((TimeFloatFloatTween)item).Time;
			} else {
				return null;
			}
		}


		public void SetMotionTime (int itemType, int itemIndex, int motionType, int motionIndex, float time) {
			var item = GetMotionItem(itemType, itemIndex, motionType, motionIndex);
			if (item == null) { return; }
			if (item is TimeIntTween tiItem) {
				tiItem.Time = time;
				SetMotionItem(itemType, itemIndex, motionType, motionIndex, tiItem);
			} else if (item is TimeFloatTween tfItem) {
				tfItem.Time = time;
				SetMotionItem(itemType, itemIndex, motionType, motionIndex, tfItem);
			} else if (item is TimeFloatFloatTween tffItem) {
				tffItem.Time = time;
				SetMotionItem(itemType, itemIndex, motionType, motionIndex, tffItem);
			}
		}





	}

}