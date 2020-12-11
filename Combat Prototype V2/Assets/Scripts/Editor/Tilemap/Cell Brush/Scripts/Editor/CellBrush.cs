using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor
{
    [CreateAssetMenu(fileName = "Cell brush", menuName = "Brushes/Cell brush")]
	[CustomGridBrush(false, false, true, "Cell Brush")]
	public class CellBrush : UnityEditor.Tilemaps.GridBrush {
		private const float k_PerlinOffset = 100000f;
		public float m_PerlinScale = 0.5f;
		public int m_Z;
		public bool allowOverlap = true;

		public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position) {
			// Do not allow editing palettes
			if (brushTarget.layer == 31) {
				return;
			}

			var placementPosition = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, m_Z) + new Vector3(.5f, .5f, .5f)));

			var cell = GetCellAtPoint(placementPosition);

			// Don't place cell prefab if overlap not allowed and one already exists
			if (!allowOverlap && cell) {
				return;
			}

			// If overlap is allowed, then destroy the previous cell before placing prefab
			if (cell) {
				Erase(grid, brushTarget, position);
			}

			//int index = Mathf.Clamp(Mathf.FloorToInt(GetPerlinValue(position, m_PerlinScale, k_PerlinOffset)*m_Prefabs.Length), 0, m_Prefabs.Length - 1);

			PrefabTile tile = (PrefabTile) base.cells[0].tile;
			GameObject prefab = tile.TileAssociatedPrefab;
			GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);

			if (instance != null) {
				Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
				Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
				instance.transform.SetParent(brushTarget.transform);
				instance.transform.position = placementPosition + new Vector3(0f, 0f, m_Z);
			}
		}

		public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds) {
			foreach (var position in bounds.allPositionsWithin) {
				Paint(gridLayout, brushTarget, position);
			}
		}

		public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position) {
			// Do not allow editing palettes
			if (brushTarget.layer == 31) {
				base.Erase(grid, brushTarget, position);
				return;
			}

			var cell = GetCellAtPoint(new Vector2(position.x, position.y) + new Vector2(.5f, .5f));

			if (cell != null)
				Undo.DestroyObjectImmediate(cell.gameObject);
		}

		private static Collider2D GetCellAtPoint(Vector2 point) {
			int layerID = 8;
			int layerMask = 1 << layerID;

			return Physics2D.OverlapPoint(point, layerMask);
		}

		private static float GetPerlinValue(Vector3Int position, float scale, float offset) {
			return Mathf.PerlinNoise((position.x + offset)*scale, (position.y + offset)*scale);
		}
	}

	[CustomEditor(typeof(CellBrush))]
	public class CellBrushEditor : UnityEditor.Tilemaps.GridBrushEditorBase {
		private CellBrush cellBrush { get { return target as CellBrush; } }

		private SerializedObject m_SerializedObject;

		protected void OnEnable() {
			m_SerializedObject = new SerializedObject(target);
		}

		public override void OnPaintInspectorGUI() {
			m_SerializedObject.UpdateIfRequiredOrScript();
			cellBrush.m_PerlinScale = EditorGUILayout.Slider("Perlin Scale", cellBrush.m_PerlinScale, 0.001f, 0.999f);
			cellBrush.m_Z = EditorGUILayout.IntField("Position Z", cellBrush.m_Z);
			cellBrush.allowOverlap = EditorGUILayout.Toggle("Allow Overlap", cellBrush.allowOverlap);
				
			m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
	}
}
