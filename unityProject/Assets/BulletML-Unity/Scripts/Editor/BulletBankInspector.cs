using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace Pixelnest.BulletML
{
	[CustomEditor ( typeof (BulletBank) )]
	public class BulletBankInspector : Editor
	{
		private SerializedProperty			entries;
		private Vector2 					scroll;
		private	bool						showCreate;
		private	List<bool>					isEditing		=	new List<bool>();
		private	GUIStyle					style			=	new GUIStyle ();

		private void OnEnable ()
		{
			this.entries		=	serializedObject.FindProperty ("bullets");
			this.style.richText	=	true;
		}

		private	void	CheckEditing ()
		{
			if (this.isEditing.Count != this.entries.arraySize)
			{
				this.isEditing.Clear ();
				for (int i = 0; i < this.entries.arraySize; i++)
					this.isEditing.Add (false);
			}
		}

		private void CheckDelete (int index)
		{
			if (index != -1)
				this.entries.DeleteArrayElementAtIndex (index);
			Repaint ();
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			EditorGUILayout.BeginHorizontal ();
			{
				GUILayout.FlexibleSpace ();
				EditorGUILayout.LabelField ("<size=30><color=white>Bullets Bank</color></size>", style);
				GUILayout.FlexibleSpace ();
			}
			EditorGUILayout.EndHorizontal ();

			GUILayout.Space (30);

			this.CheckEditing ();

			int	index	=	-1;

			this.scroll = EditorGUILayout.BeginScrollView (this.scroll);
			{
				for (int i = 0; i < this.entries.arraySize; i++)
				{
					if ( this.BulletEntryGUI (this.entries.GetArrayElementAtIndex (i), i ) )
					{
						index	=	i;
					}
					EditorGUILayout.Space ();
				}
			}
			EditorGUILayout.EndScrollView ();

			this.CheckDelete (index);

			GUILayout.Space (20);
			GUILayout.FlexibleSpace ();

			if (GUILayout.Button ("Add New Bullet") )
			{
				BulletBank bb	=	this.target as BulletBank;
				bb.CreateBullet ();
			}

			serializedObject.ApplyModifiedProperties ();
		}

		private bool BulletEntryGUI (SerializedProperty property, int index)
		{
			SerializedProperty nameProp						=	property.FindPropertyRelative ("name");
			SerializedProperty prefabProp					=	property.FindPropertyRelative ("prefab");
			SerializedProperty spriteProp					=	property.FindPropertyRelative ("sprite");
			SerializedProperty TimeToLiveInSecondsProp 		= 	property.FindPropertyRelative ("TimeToLiveInSeconds");
			SerializedProperty DestroyWhenOutOfScreenProp 	= 	property.FindPropertyRelative ("DestroyWhenOutOfScreen");

			string 				entryName 					=	nameProp.stringValue;

			EditorGUILayout.BeginVertical ( "OL Box", GUILayout.Height (100f) );
			{
				// Reserve room for title
				Rect headerRect = GUILayoutUtility.GetRect (new GUIContent (entryName), "OL Title");

				EditorGUILayout.PropertyField (prefabProp);
				EditorGUILayout.PropertyField (spriteProp);
				EditorGUILayout.PropertyField (TimeToLiveInSecondsProp);
				EditorGUILayout.PropertyField (DestroyWhenOutOfScreenProp);


				Event evt = Event.current;
				if (!this.isEditing[index] && evt.type == EventType.MouseDown && evt.clickCount == 2 && 
				    headerRect.Contains (evt.mousePosition) )
				{
					this.isEditing[index] = true;
				}
				else if (this.isEditing[index] && evt.type == EventType.KeyDown && 
				         (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return) )
				{
					if (nameProp.stringValue != "")
					{
						this.isEditing[index]	=	false;
						Repaint ();
					}
				}

				if (this.isEditing[index])
				{
					nameProp.stringValue	=	 GUI.TextField (headerRect, nameProp.stringValue, "OL Title");
				}
				else
				{
					GUI.Label (headerRect, new GUIContent (entryName), "OL Title");
				}
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.BeginHorizontal ();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button ("Remove") )
				{
					return true;
				}
			}
			EditorGUILayout.EndHorizontal ();

			return false;
		}

	}
}
