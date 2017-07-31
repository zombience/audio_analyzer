
using UnityEngine;
using UnityEditor;
using AudioAnalyzer.EditorUtilities;

namespace AudioAnalyzer.Inspector
{

	[CustomEditor(typeof(FilterBands))]
	public class EQBandsEditor : InspectorBase<FilterBands>
	{

		SerializedProperty bands;
		SerializedProperty mute;
		SerializedProperty audioCurve;
		
		Color bgColor;

		Texture2D[] barTextures;

		protected FilterBands.BandPassFilter[] filters;

		protected override void OnEnable()
		{
			base.OnEnable();
			showDefaultInspector = false;

			bands = serializedObject.FindProperty("bands");
			mute = serializedObject.FindProperty("muteAudio");
			audioCurve = serializedObject.FindProperty("audioCurve");
			bgColor = GUI.backgroundColor;
			filters = (target as FilterBands).editorBands;
		}


		void OnDisable()
		{
			if (barTextures != null)
			{
				foreach (var texture in barTextures)
				{
					DestroyImmediate(texture);
				}
				barTextures = null;
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			serializedObject.Update();
			EditorGUILayout.PropertyField(mute);
			EditorGUILayout.PropertyField(audioCurve);

			int bandCount = bands.arraySize;

			Context.NOTPLAYING.Action(() =>
			{
				GUILayout.BeginHorizontal();
				GUI.backgroundColor = Color.green;
				// limit max bands to 4 for now. could potentially have many more, but 
				// so far best results are with 3-4 bands 
				if(bandCount < 4)
				{
					if (GUILayout.Button("add band", GUILayout.Width(80), GUILayout.ExpandWidth(true)))
					{
						bands.InsertArrayElementAtIndex(bands.arraySize);
						bands.GetArrayElementAtIndex(bands.arraySize - 1).FindPropertyRelative("order").intValue = bands.arraySize - 1;
					}
				}
				if(bandCount > 0)
				{
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button("remove band", GUILayout.Width(80), GUILayout.ExpandWidth(true)))
					{
						bands.DeleteArrayElementAtIndex(bands.arraySize - 1);
					}
				}
				GUILayout.EndHorizontal();
			});

			GUI.backgroundColor = bgColor;
			// make sure that bands get displayed in the correct order - arrays are not guaranteed to be saved in order
			int[] order = new int[bands.arraySize];
			for (int i = 0; i < bands.arraySize; i++)
			{
				order[i] = bands.GetArrayElementAtIndex(i).FindPropertyRelative("order").intValue;
			}

			for (int i = 0; i < bands.arraySize; i++)
			{
				SerializedProperty band = bands.GetArrayElementAtIndex(order[i]);
				SerializedProperty listen = band.FindPropertyRelative("listen");
				SerializedProperty cutoff = band.FindPropertyRelative("cutoff");
				SerializedProperty q = band.FindPropertyRelative("q");
				SerializedProperty gain = band.FindPropertyRelative("bandGain");

				SerializedProperty headroom = band.FindPropertyRelative("headroom");
				SerializedProperty dynamicRange = band.FindPropertyRelative("dynamicRange");
				SerializedProperty lowerBound = band.FindPropertyRelative("lowerBound");
				SerializedProperty sensitivity = band.FindPropertyRelative("sensitivity");

				GUI.backgroundColor = Color.yellow;
				EditorGUILayout.PropertyField(listen);

				GUI.backgroundColor = Color.cyan;
				GUILayout.BeginHorizontal();
				GUILayout.Label("cutoff in hz: ", GUILayout.Width(80));
				EditorGUILayout.FloatField(Mathf.Pow(2, 10 * cutoff.floatValue - 10) * 15000);
				GUILayout.EndHorizontal();

				GUI.backgroundColor = Color.magenta;
				EditorGUILayout.PropertyField(cutoff);
				EditorGUILayout.PropertyField(q);
				EditorGUILayout.PropertyField(gain);

				EditorGUILayout.PropertyField(headroom);
				EditorGUILayout.PropertyField(dynamicRange);
				EditorGUILayout.PropertyField(lowerBound);

				GUILayout.Space(20);

				if (Application.isPlaying) DrawInputLevelBars(filters[i]);

				if (i < bands.arraySize - 1) GUILayout.Space(40);
			}
			EditorUtility.SetDirty(target);
			serializedObject.ApplyModifiedProperties();
		}

		// Make a texture which contains only one pixel.
		Texture2D NewBarTexture(Color color)
		{
			var texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, color);
			texture.Apply();
			return texture;
		}

		// Draw the input level bar.
		void DrawInputLevelBars(FilterBands.BandPassFilter band)
		{
			if (barTextures == null)
			{
				// Make textures for drawing level bars.
				barTextures = new Texture2D[]
				{
					NewBarTexture(new Color(55.0f / 255, 53.0f / 255, 45.0f / 255)),
					NewBarTexture(new Color(250.0f / 255, 249.0f / 255, 248.0f / 255)),
					NewBarTexture(new Color(110.0f / 255, 192.0f / 255, 91.0f / 255, 0.8f)),
					NewBarTexture(new Color(226.0f / 255, 0, 7.0f / 255, 0.8f)),
					NewBarTexture(new Color(249.0f / 255, 185.0f / 255, 22.0f / 255))
				};
			}

			// Get a rectangle as a text field and fill it.
			var rect = GUILayoutUtility.GetRect(18, 16, "TextField");
			GUI.DrawTexture(rect, barTextures[0]);

			// Draw the raw input bar.
			var temp = rect;
			temp.width *= Mathf.Clamp01((band.RawInput - band.LowerBound) / (3 - band.LowerBound));
			GUI.DrawTexture(temp, barTextures[1]);

			// Draw the dynamic range.
			temp.x += rect.width * (band.Peak - band.LowerBound - band.DynamicRange - band.Headroom) / (3 - band.LowerBound);
			temp.width = rect.width * band.DynamicRange / (3 - band.LowerBound);
			GUI.DrawTexture(temp, barTextures[2]);

			// Draw the headroom.
			temp.x += temp.width;
			temp.width = rect.width * band.Headroom / (3 - band.LowerBound);
			GUI.DrawTexture(temp, barTextures[3]);

			// Display the peak level value.
			EditorGUI.LabelField(rect, "Peak: " + band.Peak.ToString("0.0") + " dB");

			// Draw the gain level.
			DrawLevelBar("Gain", band.Gain, barTextures[0], barTextures[1]);

			// Draw the offset level.
			DrawLevelBar("Offset", band.Offset, barTextures[0], barTextures[1]);

			// Draw the output level.
			DrawLevelBar("Out", band.level, barTextures[0], barTextures[4]);
		}

		void DrawLevelBar(string label, float value, Texture bg, Texture fg)
		{
			// Get a rectangle as a text field and fill it.
			var rect = GUILayoutUtility.GetRect(18, 16, "TextField");
			GUI.DrawTexture(rect, bg);

			// Draw a level bar.
			var temp = rect;
			temp.width *= value;
			GUI.DrawTexture(temp, fg);

			// Display the level value in percentage.
			EditorGUI.LabelField(rect, label + ": " + (value * 100).ToString("0.0") + " %");
		}
	}
}