﻿namespace UIGadget {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;


	public class Rectangle : Image {


		// Const
		private float FLOAT_GAP = 0.0001f;

		// Ser
		[Header("Rect"), SerializeField] private int m_Detail = 12;
		[SerializeField] private float m_Round = 8f;
		[SerializeField] private Color m_FillColor = Color.white;
		[Header("Border"), SerializeField] private float m_Border = 4f;
		[SerializeField] private float m_BorderOffset = 0.5f;
		[SerializeField] private Color m_BorderColor = Color.black;


		// MSG
		protected override void OnPopulateMesh (VertexHelper toFill) {

			toFill.Clear();
			var rect = GetPixelAdjustedRect();
			if (color.a < FLOAT_GAP || rect.width <= FLOAT_GAP || rect.height <= FLOAT_GAP) { return; }
			float round = Mathf.Clamp(m_Round, 0f, Mathf.Min(rect.width / 2f, rect.height / 2f));
			float border = Mathf.Max(m_Border, 0f);
			float borderOffset = border > FLOAT_GAP ? Mathf.Max(m_BorderOffset, 1f - rect.height / border / 2f) : m_BorderOffset;
			int step = Mathf.Clamp(m_Detail, 1, 1024);
			float borderGap = (1f - borderOffset) * border;
			var min0 = rect.min + Vector2.one * borderGap;
			var min1 = rect.min + Vector2.one * round;
			var max1 = rect.max - Vector2.one * round;
			var max0 = rect.max - Vector2.one * borderGap;

			// Fill
			if (m_FillColor.a > FLOAT_GAP) {

				GadgetUtil.SetCacheColor(m_FillColor * color);

				// Mid
				var midMin = Vector2.Max(min0, min1);
				var midMax = Vector2.Min(max0, max1);
				if (midMin.x < midMax.x && midMin.y < midMax.y) {
					GadgetUtil.FillQuad(toFill, midMin.x, midMax.x, midMin.y, midMax.y);
				}

				if (round > FLOAT_GAP && borderGap < round) {
					// Edge
					GadgetUtil.FillQuad(toFill, min0.x, min1.x, min1.y, max1.y);
					GadgetUtil.FillQuad(toFill, max1.x, max0.x, min1.y, max1.y);
					GadgetUtil.FillQuad(toFill, min1.x, max1.x, min0.y, min1.y);
					GadgetUtil.FillQuad(toFill, min1.x, max1.x, max1.y, max0.y);
					// Corner
					GadgetUtil.FillDisc(toFill, 180, 270, step, round - borderGap, new Vector2(min1.x, min1.y));
					GadgetUtil.FillDisc(toFill, 270, 360, step, round - borderGap, new Vector2(min1.x, max1.y));
					GadgetUtil.FillDisc(toFill, 0, 90, step, round - borderGap, new Vector2(max1.x, max1.y));
					GadgetUtil.FillDisc(toFill, 90, 180, step, round - borderGap, new Vector2(max1.x, min1.y));
				}
			}

			// Border
			if (m_BorderColor.a > FLOAT_GAP && border > FLOAT_GAP) {
				GadgetUtil.SetCacheColor(m_BorderColor * color);
				float startRadius = round - borderGap;
				float endRadius = startRadius + border;
				var bMin = min0 - Vector2.one * border;
				var bMax = max0 + Vector2.one * border;
				// Edge
				GadgetUtil.FillQuad(toFill, bMin.x, min0.x, Mathf.Max(min1.y, bMin.y), Mathf.Min(max1.y, bMax.y));
				GadgetUtil.FillQuad(toFill, max0.x, bMax.x, Mathf.Max(min1.y, bMin.y), Mathf.Min(max1.y, bMax.y));
				GadgetUtil.FillQuad(toFill, Mathf.Max(min1.x, bMin.x), Mathf.Min(max1.x, bMax.x), bMin.y, min0.y);
				GadgetUtil.FillQuad(toFill, Mathf.Max(min1.x, bMin.x), Mathf.Min(max1.x, bMax.x), max0.y, bMax.y);
				// Corner
				if (border * borderOffset > -round) {
					if (startRadius > 0f) {
						GadgetUtil.FillDisc(toFill, 180, 270, step, startRadius, endRadius, new Vector2(min1.x, min1.y));
						GadgetUtil.FillDisc(toFill, 270, 360, step, startRadius, endRadius, new Vector2(min1.x, max1.y));
						GadgetUtil.FillDisc(toFill, 0, 90, step, startRadius, endRadius, new Vector2(max1.x, max1.y));
						GadgetUtil.FillDisc(toFill, 90, 180, step, startRadius, endRadius, new Vector2(max1.x, min1.y));
					} else {
						GadgetUtil.FillDisc(toFill, 180, 270, step, endRadius, new Vector2(min1.x, min1.y));
						GadgetUtil.FillDisc(toFill, 270, 360, step, endRadius, new Vector2(min1.x, max1.y));
						GadgetUtil.FillDisc(toFill, 0, 90, step, endRadius, new Vector2(max1.x, max1.y));
						GadgetUtil.FillDisc(toFill, 90, 180, step, endRadius, new Vector2(max1.x, min1.y));
					}
				}
			}

		}






	}
}
