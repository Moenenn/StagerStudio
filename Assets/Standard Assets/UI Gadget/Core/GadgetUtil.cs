﻿namespace UIGadget {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;


	public static class GadgetUtil {



		// Cache
		private readonly static UIVertex[] VertexCache = {
			new UIVertex(){ position = Vector3.zero, },
			new UIVertex(){ position = Vector3.zero, },
			new UIVertex(){ position = Vector3.zero, },
			new UIVertex(){ position = Vector3.zero, },
		};



		// Fill
		public static void FillDisc (VertexHelper toFill, float startAngle, float endAngle, int step, float radius, Vector2 pivot) {
			if (startAngle > endAngle) {
				float temp = startAngle;
				startAngle = endAngle;
				endAngle = temp;
			}
			VertexCache[0].position = pivot;
			VertexCache[1].position = GetPos(startAngle);
			for (int i = 0; i < step; i++) {
				float angleR = Mathf.Lerp(startAngle, endAngle, (float)(i + 1) / step);
				VertexCache[2].position = GetPos(angleR);
				// Fill
				int vCount = toFill.currentVertCount;
				toFill.AddVert(VertexCache[0]);
				toFill.AddVert(VertexCache[1]);
				toFill.AddVert(VertexCache[2]);
				toFill.AddTriangle(vCount, vCount + 1, vCount + 2);
				// Final
				VertexCache[1].position = VertexCache[2].position;
			}
			// Func
			Vector3 GetPos (float angle) => new Vector3(
				Mathf.Sin(angle * Mathf.Deg2Rad) * radius + pivot.x,
				Mathf.Cos(angle * Mathf.Deg2Rad) * radius + pivot.y,
				0f
			);
		}


		public static void FillDisc (VertexHelper toFill, float startAngle, float endAngle, int step, float startRadius, float endRadius, Vector2 pivot) {
			VertexCache[0].position = GetPos(startAngle, endRadius);
			VertexCache[3].position = GetPos(startAngle, startRadius);
			for (int i = 0; i < step; i++) {
				float angleR = Mathf.Lerp(startAngle, endAngle, (float)(i + 1) / step);
				VertexCache[1].position = GetPos(angleR, endRadius);
				VertexCache[2].position = GetPos(angleR, startRadius);
				// Fill
				toFill.AddUIVertexQuad(VertexCache);
				// Final
				VertexCache[0].position = VertexCache[1].position;
				VertexCache[3].position = VertexCache[2].position;
			}
			// Func
			Vector3 GetPos (float angle, float radius) => new Vector3(
				Mathf.Sin(angle * Mathf.Deg2Rad) * radius + pivot.x,
				Mathf.Cos(angle * Mathf.Deg2Rad) * radius + pivot.y,
				0f
			);
		}


		public static void FillTriangle (VertexHelper toFill, Vector3 a, Vector3 b, Vector3 c) {
			VertexCache[0].position = a;
			VertexCache[1].position = b;
			VertexCache[2].position = c;
			int vert = toFill.currentVertCount;
			toFill.AddVert(VertexCache[0]);
			toFill.AddVert(VertexCache[1]);
			toFill.AddVert(VertexCache[2]);
			toFill.AddTriangle(vert, vert + 1, vert + 2);
		}


		public static void FillQuad (VertexHelper toFill, float left, float right, float down, float up) {
			VertexCache[0].position.x = left;
			VertexCache[1].position.x = left;
			VertexCache[2].position.x = right;
			VertexCache[3].position.x = right;
			VertexCache[0].position.y = down;
			VertexCache[1].position.y = up;
			VertexCache[2].position.y = up;
			VertexCache[3].position.y = down;
			toFill.AddUIVertexQuad(VertexCache);
		}


		public static void FillLine (VertexHelper toFill, Vector3 a, Vector3 b, float size) {
			Vector3 cross = Vector3.Cross(a - b, Vector3.forward).normalized * (size * 0.5f);
			VertexCache[0].position = a - cross;
			VertexCache[1].position = a + cross;
			VertexCache[2].position = b + cross;
			VertexCache[3].position = b - cross;
			toFill.AddUIVertexQuad(VertexCache);
		}


		// Set Cache
		public static void SetCacheColor (Color color) => VertexCache[0].color = VertexCache[1].color = VertexCache[2].color = VertexCache[3].color = color;


		public static void SetCacheColor (Color color0, Color color1, Color color2, Color color3) {
			VertexCache[0].color = color0;
			VertexCache[1].color = color1;
			VertexCache[2].color = color2;
			VertexCache[3].color = color3;
		}



	}
}