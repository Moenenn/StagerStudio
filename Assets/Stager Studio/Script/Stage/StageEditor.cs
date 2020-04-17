﻿namespace StagerStudio.Stage {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using Data;
	using Rendering;
	using Saving;



	public class StageEditor : MonoBehaviour {




		#region --- SUB ---


		public delegate (Vector3 min, Vector3 max, float size, float ratio) ZoneHandler ();
		public delegate void VoidHandler ();
		public delegate Beatmap BeatmapHandler ();
		public delegate (int type, int index) IntIntHandler ();
		public delegate bool BoolHandler ();
		public delegate Beatmap.Stage StageHandler ();
		public delegate Beatmap.Track TrackHandler ();
		public delegate List<Beatmap.Note> NotesIntHandler (int i);
		public delegate void VoidStringBoolHandler (string str, bool b);
		public delegate string StringStringHandler (string str);

		#endregion




		#region --- VAR ---


		// Const
		private readonly static string[] ITEM_LAYER_NAMES = { "Stage", "Track", "Note", "Speed", "Motion", };
		private const string HINT_GlobalBrushScale = "Editor.Hint.GlobalBrushScale";

		// Handle
		public static ZoneHandler GetZoneMinMax { get; set; } = null;
		public static BeatmapHandler GetBeatmap { get; set; } = null;
		public static IntIntHandler GetBrushTypeIndex { get; set; } = null;
		public static BoolHandler GetEditorActive { get; set; } = null;
		public static VoidHandler OnSelectionChanged { get; set; } = null;
		public static VoidHandler OnLockEyeChanged { get; set; } = null;
		public static BoolHandler GetUseDynamicSpeed { get; set; } = null;
		public static BoolHandler GetUseAbreast { get; set; } = null;
		public static StageHandler GetDefaultStageBrush { get; set; } = null;
		public static TrackHandler GetDefaultTrackBrush { get; set; } = null;
		public static NotesIntHandler GetNotesBrushAt { get; set; } = null;
		public static VoidStringBoolHandler LogHint { get; set; } = null;
		public static StringStringHandler GetLanguage { get; set; } = null;

		// Api
		public int SelectingType { get; private set; } = -1;
		public int SelectingCount => SelectingObjectIndexs.Count;

		// Short
		private Camera Camera => _Camera != null ? _Camera : (_Camera = Camera.main);
		private Transform AxisMoveX => m_MoveAxis.GetChild(0);
		private Transform AxisMoveY => m_MoveAxis.GetChild(1);
		private Transform AxisMoveXY => m_MoveAxis.GetChild(2);

		// Ser
		[SerializeField] private Toggle[] m_EyeTGs = null;
		[SerializeField] private Toggle[] m_LockTGs = null;
		[SerializeField] private Transform[] m_Containers = null;
		[SerializeField] private Transform[] m_AntiTargets = null;
		[SerializeField] private RectTransform m_FocusCancel = null;
		[SerializeField] private Animator m_FocusAni = null;
		[SerializeField] private GridRenderer m_Grid = null;
		[SerializeField] private SpriteRenderer m_Hover = null;
		[SerializeField] private LoopUvRenderer m_Ghost = null;
		[SerializeField] private SpriteRenderer m_GhostPivot = null;
		[SerializeField] private Transform m_MoveAxis = null;
		[SerializeField] private Transform[] m_AxisHovers = null;
		[SerializeField] private string m_NoteHoldLayerName = "HoldNote";
		[SerializeField] private string m_FocusKey = "Focus";
		[SerializeField] private string m_UnfocusKey = "Unfocus";
		[SerializeField] private float m_Duration = 0.5f;

		// Data
		private readonly static Dictionary<int, int> LayerToTypeMap = new Dictionary<int, int>();
		private readonly static bool[] ItemLock = { false, false, false, false, false, };
		private readonly static int[] ItemLayers = { -1, -1, -1, -1, -1 };
		private readonly static LayerMask[] ItemMasks = { -1, -1, -1, -1, -1, };
		private bool FocusMode = false;
		private bool UIReady = true;
		private int SortingLayerID_UI = -1;
		private Coroutine FocusAniCor = null;
		private Camera _Camera = null;
		private LayerMask UnlockedMask = default;

		// Mouse
		private readonly static List<(int index, float x, float y_time)> SelectingObjectIndexs = new List<(int, float, float)>();
		private readonly static RaycastHit[] CastHits = new RaycastHit[64];
		private bool ClickStartInsideSelection = false;
		private bool UndoDirtySetted_Move = false;
		private bool AxisHovering = false;
		private int HoldNoteLayer = -1;
		private Ray? MouseRayDown = null;
		private Vector2 HoverScaleMuti = Vector2.one;
		private Vector2 GhostPivotScaleMuti = Vector2.one;

		// Saving
		public SavingBool UseGlobalBrushScale { get; set; } = new SavingBool("StageEditor.UseGlobalBrushScale", true);


		#endregion




		#region --- MSG ---


		private void Awake () {

			// Init Layer
			for (int i = 0; i < ITEM_LAYER_NAMES.Length; i++) {
				ItemLayers[i] = LayerMask.NameToLayer(ITEM_LAYER_NAMES[i]);
				LayerToTypeMap.Add(ItemLayers[i], i);
			}
			HoldNoteLayer = LayerMask.NameToLayer(m_NoteHoldLayerName);
			LayerToTypeMap.Add(HoldNoteLayer, 2);
			SortingLayerID_UI = SortingLayer.NameToID("UI");


			// Unlock Mask
			RefreshUnlockedMask();

			// ItemMasks
			for (int i = 0; i < ITEM_LAYER_NAMES.Length; i++) {
				ItemMasks[i] = LayerMask.GetMask(ITEM_LAYER_NAMES[i]);
			}

			// Eye TGs
			for (int i = 0; i < m_EyeTGs.Length; i++) {
				int index = i;
				var tg = m_EyeTGs[index];
				tg.isOn = false;
				tg.onValueChanged.AddListener((isOn) => {
					if (!UIReady) { return; }
					SetEye(index, !isOn);
				});
			}

			// Lock TGs
			for (int i = 0; i < m_LockTGs.Length; i++) {
				int index = i;
				var tg = m_LockTGs[index];
				tg.isOn = ItemLock[index];
				tg.onValueChanged.AddListener((locked) => {
					if (!UIReady) { return; }
					SetLock(index, locked);
				});
			}

			// UI
			HoverScaleMuti = m_Hover.transform.localScale;
			GhostPivotScaleMuti = m_GhostPivot.transform.localScale;

		}


		private void LateUpdate () {
			// Check
			if (!AntiTargetAllow()) {
				MouseRayDown = null;
				if (m_Grid.GridEnabled) {
					m_Grid.SetGridTransform(false);
				}
				if (m_Hover.gameObject.activeSelf) {
					SetTargetActive(m_Hover.gameObject, false);
				}
				LateUpdate_Axis();
				return;
			}
			var map = GetBeatmap();
			// Axis Hovering
			AxisHovering = false;
			foreach (var hover in m_AxisHovers) {
				if (hover.gameObject.activeSelf) {
					AxisHovering = true;
					break;
				}
			}
			// Misc
			LateUpdate_Selection(map);
			// Mouse
			if (Input.GetMouseButton(0)) {
				if (!MouseRayDown.HasValue) {
					// Down
					MouseRayDown = GetMouseRay();
					OnMouseLeftDown(map);
				} else {
					// Drag
					OnMouseLeftDrag();
				}
			} else {
				// Normal
				if (MouseRayDown.HasValue) {
					MouseRayDown = null;
				}
			}
			OnMouseHover(map);
			// Final
			LateUpdate_Axis();
		}


		// Mouse Left
		private void OnMouseLeftDown (Beatmap map) {
			if (AxisHovering) { return; }
			var (_, brushIndex) = GetBrushTypeIndex();
			if (brushIndex < 0) {
				// Select or Move
				bool ctrl = Input.GetKey(KeyCode.LeftControl);
				bool alt = Input.GetKey(KeyCode.LeftAlt);
				var (overlapType, overlapIndex, _) = GetCastTypeIndex(GetMouseRay(), UnlockedMask, true);
				ClickStartInsideSelection = SelectingCount > 0 && overlapType == SelectingType && map.GetSelect(overlapType, overlapIndex);
				if (ClickStartInsideSelection && !alt && !ctrl) {
					// Move




				} else {
					// Select
					if (!ctrl && !alt) { ClearSelection(); }
					if (overlapType >= 0 && overlapIndex >= 0) {
						AddSelection(overlapType, overlapIndex, !alt, map);
						m_MoveAxis.gameObject.SetActive(SelectingCount > 0);
						OnSelectionChanged();
					}
				}
			} else {
				// Paint






			}
		}


		private void OnMouseLeftDrag () {
			if (AxisHovering) { return; }
			var (_, brushIndex) = GetBrushTypeIndex();
			if (brushIndex < 0 && ClickStartInsideSelection) {
				// Moving Selection





			}
		}


		// Mouse Hover
		private void OnMouseHover (Beatmap map) {

			var (brushType, brushIndex) = GetBrushTypeIndex();

			if (AxisHovering || Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
				SetTargetActive(m_Hover.gameObject, false);
				SetTargetActive(m_Ghost.gameObject, false);
				return;
			}

			// --- Painting ---
			bool brushUnlocked = !GetItemLock(brushType);
			if (brushIndex >= 0 && !brushUnlocked) {
				SetTargetActive(m_Hover.gameObject, false);
				SetTargetActive(m_Ghost.gameObject, false);
			}
			if (brushIndex >= 0 && brushUnlocked) {
				var hoverTarget = OnMouseHover_Painting_Grid(map);
				OnMouseHover_Ghost(hoverTarget);
			}

			// --- Normal Select ---
			if (brushIndex < 0) {
				OnMouseHover_NormalSelect();
			}

		}


		private Transform OnMouseHover_Painting_Grid (Beatmap map) {

			var (brushType, brushIndex) = GetBrushTypeIndex();

			// Hover
			SetTargetActive(m_Hover.gameObject, false);

			// Paint Grid
			bool gridEnable = false;
			Vector3 pos = default;
			Quaternion rot = default;
			Vector3 scl = default;
			var ray = GetMouseRay();
			var (zoneMin, zoneMax, zoneSize, zoneRatio) = GetZoneMinMax();
			Transform hoverTarget = null;
			m_Grid.ObjectSpeedMuti = 1f;
			switch (brushType) {
				case 0: // Stage
					gridEnable = true;
					pos = Util.Vector3Lerp3(zoneMin, zoneMax, 0.5f, 0f);
					rot = Quaternion.identity;
					scl = new Vector3(zoneSize, zoneSize / zoneRatio, 1f);
					m_Grid.Mode = 0;
					m_Grid.UseDynamicSpeed = true;
					break;
				case 1: // Track
				case 2: // Note
					int hoverItemType;
					int hoverItemIndex;
					(hoverItemType, hoverItemIndex, hoverTarget) = GetCastTypeIndex(ray, ItemMasks[brushType - 1], true);
					if (hoverTarget != null) {
						gridEnable = true;
						pos = hoverTarget.position;
						rot = hoverTarget.rotation;
						scl = hoverTarget.GetChild(0).localScale;
						m_Grid.ObjectSpeedMuti = hoverItemType == 1 && GetUseDynamicSpeed() ? map.GetSpeedMuti(1, hoverItemIndex) : 1f;
					}
					m_Grid.Mode = brushType;
					m_Grid.UseDynamicSpeed = true;
					break;
				case 3: // Timing
					gridEnable = true;
					scl = new Vector3(zoneSize, zoneSize / zoneRatio, 1f);
					pos = new Vector3((zoneMin.x + zoneMax.x) / 2f, zoneMin.y, zoneMin.z);
					rot = Quaternion.identity;
					m_Grid.ObjectSpeedMuti = 1f;
					m_Grid.Mode = 3;
					m_Grid.UseDynamicSpeed = false;
					break;
			}
			m_Grid.SetGridTransform(gridEnable, pos, rot, scl);
			return hoverTarget;
		}


		private void OnMouseHover_Ghost (Transform hoverTarget) {

			var (brushType, brushIndex) = GetBrushTypeIndex();

			// Ghost
			var ray = GetMouseRay();
			var (zoneMin, zoneMax, zoneSize, zoneRatio) = GetZoneMinMax();
			bool ghostEnable = false;
			float ghostPivotX = 0.5f;
			Vector2 ghostSize = default;
			Vector3 ghostPos = default;
			Quaternion ghostRot = Quaternion.identity;
			const float GHOST_NOTE_Y = 0.032f;
			switch (brushType) {
				case 0: // Stage
					var stage = GetDefaultStageBrush();
					if (stage != null && !GetUseAbreast()) {
						var mousePos = Util.GetRayPosition(ray, zoneMin, zoneMax, null, true);
						ghostEnable = mousePos.HasValue;
						if (mousePos.HasValue) {
							ghostSize.x = zoneSize * stage.Width;
							ghostSize.y = zoneSize * stage.Height / zoneRatio;
							ghostPos = mousePos.Value;
							ghostPos = m_Grid.SnapWorld(ghostPos);
							ghostPivotX = 0.5f;
						}
					}
					break;
				case 1: // Track
					var track = GetDefaultTrackBrush();
					if (track != null && hoverTarget != null) {
						var mousePos = Util.GetRayPosition(ray, zoneMin, zoneMax, null, true);
						ghostEnable = mousePos.HasValue;
						if (mousePos.HasValue) {
							if (UseGlobalBrushScale) {
								ghostSize.x = track.Width * zoneSize;
							} else {
								ghostSize.x = track.Width * hoverTarget.GetChild(0).localScale.x;
							}
							ghostSize.y = hoverTarget.GetChild(0).localScale.y;
							ghostPos = mousePos.Value;
							ghostPos = m_Grid.SnapWorld(ghostPos, true);
							ghostRot = hoverTarget.transform.rotation;
							ghostPivotX = 0.5f;
						}
					}
					break;
				case 2: // Note
					var notes = GetNotesBrushAt(brushIndex);
					if (notes != null && notes.Count > 0 && hoverTarget != null) {
						var note = notes[0];
						var mousePos = Util.GetRayPosition(ray, zoneMin, zoneMax, hoverTarget, false);
						ghostEnable = mousePos.HasValue;
						if (mousePos.HasValue) {
							if (UseGlobalBrushScale) {
								ghostSize.x = note.Width * zoneSize;
							} else {
								ghostSize.x = note.Width * hoverTarget.GetChild(0).localScale.x;
							}
							ghostSize.y = Mathf.Max(note.Duration * m_Grid.SpeedMuti * m_Grid.ObjectSpeedMuti * hoverTarget.GetChild(0).localScale.y, GHOST_NOTE_Y / zoneSize);
							ghostPos = mousePos.Value;
							ghostPos = m_Grid.SnapWorld(ghostPos, false, true);
							ghostRot = hoverTarget.transform.rotation;
							ghostPivotX = 0.5f;
						}
					}
					break;
				case 3: { // Speed
						var mousePos = Util.GetRayPosition(ray, zoneMin, zoneMax, null, false);
						ghostEnable = mousePos.HasValue;
						if (mousePos.HasValue) {
							ghostSize.x = 0.12f / zoneSize;
							ghostSize.y = GHOST_NOTE_Y / zoneSize;
							ghostPos.x = zoneMin.x;
							ghostPos.y = m_Grid.SnapWorld(mousePos.Value, false, false, false).y;
							ghostPos.z = zoneMin.z;
							ghostRot = Quaternion.identity;
							ghostPivotX = 0f;
						}
					}
					break;
			}
			SetTargetActive(m_Ghost.gameObject, ghostEnable);
			if (ghostEnable) {
				m_Ghost.Size = ghostSize;
				m_Ghost.transform.localScale = ghostSize;
				m_Ghost.transform.position = ghostPos;
				m_Ghost.transform.rotation = ghostRot;
				m_Ghost.Pivot = new Vector3(ghostPivotX, 0f, 0f);
				m_Ghost.SetSortingLayer(SortingLayerID_UI, short.MaxValue - 1);
				m_GhostPivot.transform.localScale = new Vector3(GhostPivotScaleMuti.x / ghostSize.x, GhostPivotScaleMuti.y / ghostSize.y, 1f);
			}
		}


		private void OnMouseHover_NormalSelect () {
			SetTargetActive(m_Ghost.gameObject, false);
			// Grid
			if (m_Grid.GridEnabled) {
				m_Grid.SetGridTransform(false);
			}
			// Hover
			var ray = GetMouseRay();
			var (_, _, target) = GetCastTypeIndex(ray, UnlockedMask, true);
			if (target != null) {
				SetTargetActive(m_Hover.gameObject, true);
				m_Hover.transform.position = target.GetChild(0).position;
				m_Hover.transform.rotation = target.GetChild(0).rotation;
				m_Hover.size = target.GetChild(0).localScale / HoverScaleMuti;
			} else {
				SetTargetActive(m_Hover.gameObject, false);
			}
		}


		// Selection
		private void LateUpdate_Selection (Beatmap map) {
			if (SelectingCount == 0) { return; }
			bool changed = false;
			for (int i = 0; i < SelectingObjectIndexs.Count; i++) {
				int index = SelectingObjectIndexs[i].index;
				if (!map.GetSelect(SelectingType, index)) {
					SelectingObjectIndexs.RemoveAt(i);
					i--;
					changed = true;
				}
			}
			if (changed) {
				if (SelectingCount == 0) {
					m_MoveAxis.gameObject.SetActive(false);
				}
				OnSelectionChanged();
			}
		}


		// Axis
		private void LateUpdate_Axis () {
			if (!m_MoveAxis.gameObject.activeSelf) { return; }
			if (SelectingCount == 0) {
				m_MoveAxis.gameObject.SetActive(false);
				return;
			}
			var pos = m_Containers[SelectingType].GetChild(SelectingObjectIndexs[0].index).position;
			if (m_MoveAxis.position != pos) {
				m_MoveAxis.position = pos;
			}
			bool editorActive = GetEditorActive();
			bool xActive = editorActive && SelectingType != 3;
			bool yActive = editorActive && SelectingType != 1;
			bool xyActive = xActive || yActive;
			if (AxisMoveX.gameObject.activeSelf != xActive) {
				AxisMoveX.gameObject.SetActive(xActive);
			}
			if (AxisMoveY.gameObject.activeSelf != yActive) {
				AxisMoveY.gameObject.SetActive(yActive);
			}
			if (AxisMoveXY.gameObject.activeSelf != xyActive) {
				AxisMoveXY.gameObject.SetActive(xyActive);
			}
		}


		public void OnMoveAxisDrag (Vector3 pos, Vector3? downPos, int axis) {
			if (SelectingCount == 0) { return; }
			var map = GetBeatmap();
			if (map == null) { return; }
			if (!downPos.HasValue) {
				// Down
				for (int i = 0; i < SelectingCount; i++) {
					SelectingObjectIndexs[i] = (
						SelectingObjectIndexs[i].index,
						map.GetX(SelectingType, i),
						map.GetTime(SelectingType, i)
					);
				}
				UndoDirtySetted_Move = false;
			} else {
				// Drag
				var (zoneMin, zoneMax, _, _) = GetZoneMinMax();
				var downPos01 = Util.Vector3InverseLerp3(zoneMin, zoneMax, downPos.Value.x, downPos.Value.y);
				var pos01 = Util.Vector3InverseLerp3(zoneMin, zoneMax, pos.x, pos.y);
				for (int i = 0; i < SelectingCount; i++) {
					var (index, startX, startY_Time) = SelectingObjectIndexs[i];




				}
			}
		}


		#endregion




		#region --- API ---


		public void SwitchUseGlobalBrushScale () {
			UseGlobalBrushScale.Value = !UseGlobalBrushScale;
			try {
				LogHint(
					string.Format(GetLanguage(HINT_GlobalBrushScale), UseGlobalBrushScale.Value ? "ON" : "OFF"),
					true
				);
			} catch { }
		}


		// Selection
		public void AddSelection (int type, int index, bool select, Beatmap map) {
			// Selecting Index List
			if (select) {
				// Add
				if (SelectingType >= 0 && SelectingCount > 0 && SelectingType != type) {
					ClearSelection();
				}
				SelectingType = type;
				SelectingObjectIndexs.Add((index, -1f, -1f));
			} else if (type == SelectingType) {
				// Remove
				for (int i = 0; i < SelectingCount; i++) {
					if (SelectingObjectIndexs[i].index == index) {
						SelectingObjectIndexs.RemoveAt(i);
						break;
					}
				}
			}
			// Set Cache
			map.SetSelect(type, index, select);
		}


		public void ClearSelection () {
			var map = GetBeatmap();
			if (!(map is null) && SelectingCount > 0) {
				// Clear Cache
				foreach (var (index, _, _) in SelectingObjectIndexs) {
					map.SetSelect(SelectingType, index, false);
				}
				// Clear List
				SelectingObjectIndexs.Clear();
				SelectingType = -1;
				// Final
				m_MoveAxis.gameObject.SetActive(false);
				OnSelectionChanged();

			}
		}


		public void ForAddSelectingObjects (System.Action<int> action) {
			foreach (var (index, _, _) in SelectingObjectIndexs) {
				action(index);
			}
		}


		// Container
		public void UI_SwitchContainerActive (int index) => SetEye(index, !GetContainerActive(index));


		public bool GetContainerActive (int index) => m_Containers[index].gameObject.activeSelf;


		public void SetContainerActive (int index, bool active) => SetEye(index, active);


		// Item Lock
		public bool GetItemLock (int item) => item >= 0 ? ItemLock[item] : false;


		public void UI_SwitchLock (int index) => SetLock(index, !GetItemLock(index));


		// Focus
		public void UI_SetFocus (bool focus) {
			if (!(FocusAniCor is null)) { return; }
			if (focus != FocusMode) {
				FocusMode = focus;
				m_FocusAni.enabled = true;
				m_FocusAni.SetTrigger(focus ? m_FocusKey : m_UnfocusKey);
				if (!(FocusAniCor is null)) {
					StopCoroutine(FocusAniCor);
					FocusAniCor = null;
				}
				FocusAniCor = StartCoroutine(AniCheck());
			}
			// Func
			IEnumerator AniCheck () {
				if (focus) {
					m_FocusCancel.gameObject.SetActive(true);
				}
				yield return new WaitForSeconds(m_Duration);
				yield return new WaitUntil(() =>
					m_FocusAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !m_FocusAni.IsInTransition(0)
				);
				if (!focus) {
					m_FocusCancel.gameObject.SetActive(false);
				}
				m_FocusAni.enabled = false;
				FocusAniCor = null;
			}
		}


		public void UI_SwitchFocus () => UI_SetFocus(!FocusMode);


		#endregion



		#region --- LGC ---


		private bool AntiTargetAllow () {
			if (!GetEditorActive()) { return false; }
			foreach (var t in m_AntiTargets) {
				if (t.gameObject.activeSelf) {
					return false;
				}
			}
			return true;
		}


		private Ray GetMouseRay () => Camera.ScreenPointToRay(Input.mousePosition);


		private void SetLock (int index, bool locked) {
			// Set Logic
			ItemLock[index] = locked;
			// Refresh Unlock Mask
			RefreshUnlockedMask();
			// Refresh UI
			UIReady = false;
			try {
				m_LockTGs[index].isOn = locked;
			} catch { }
			UIReady = true;
			OnLockEyeChanged();
		}


		private void SetEye (int index, bool see) {
			m_Containers[index].gameObject.SetActive(see);
			// UI
			UIReady = false;
			try {
				m_EyeTGs[index].isOn = !see;
			} catch { }
			UIReady = true;
			// MSG
			OnLockEyeChanged();
		}


		private (int type, int index, Transform target) GetCastTypeIndex (Ray ray, LayerMask mask, bool insideZone) {
			int count = Physics.RaycastNonAlloc(ray, CastHits, float.MaxValue, mask);
			int overlapType = -1;
			int overlapIndex = -1;
			Transform tf = null;
			if (!insideZone || RayInsideZone(ray)) {
				for (int i = 0; i < count; i++) {
					var hit = CastHits[i];
					int layer = hit.transform.gameObject.layer;
					int type = LayerToTypeMap.ContainsKey(layer) ? LayerToTypeMap[layer] : -1;
					int itemIndex = hit.transform.parent.GetSiblingIndex();
					if (type >= overlapType) {
						if (type != overlapType) {
							overlapIndex = -1;
						}
						if (itemIndex >= overlapIndex) {
							tf = CastHits[i].transform.parent;
						}
						overlapIndex = Mathf.Max(itemIndex, overlapIndex);
						overlapType = type;
					}
				}
			}
			return (overlapType, overlapIndex, tf);
		}


		private bool RayInsideZone (Ray ray) {
			var (zoneMin, zoneMax, _, _) = GetZoneMinMax();
			if (new Plane(Vector3.back, zoneMin).Raycast(ray, out float enter)) {
				var point = ray.GetPoint(enter);
				return point.x > zoneMin.x && point.x < zoneMax.x && point.y > zoneMin.y && point.y < zoneMax.y;
			}
			return false;
		}


		private void RefreshUnlockedMask () {
			var list = new List<string>();
			for (int i = 0; i < ItemLock.Length; i++) {
				if (!ItemLock[i]) {
					list.Add(ITEM_LAYER_NAMES[i]);
				}
			}
			// Hold Note Layer
			if (!ItemLock[2]) {
				list.Add(m_NoteHoldLayerName);
			}
			UnlockedMask = LayerMask.GetMask(list.ToArray());
		}


		private void SetTargetActive (GameObject target, bool active) {
			if (target.activeSelf != active) {
				target.SetActive(active);
			}
		}


		#endregion



	}
}