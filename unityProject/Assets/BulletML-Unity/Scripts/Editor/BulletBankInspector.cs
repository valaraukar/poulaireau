using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace Pixelnest.BulletML
{
//	[CustomPropertyDrawer ( typeof ( BulletBankEntry ) )]
	public class BulletBankEntryDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			position.y += 2;

			EditorGUI.BeginProperty (position, label, property);
			{
				SerializedProperty nameProp						=	property.FindPropertyRelative ("name");
				SerializedProperty prefabProp					=	property.FindPropertyRelative ("prefab");
				SerializedProperty spriteProp					=	property.FindPropertyRelative ("sprite");
				SerializedProperty TimeToLiveInSecondsProp 		= 	property.FindPropertyRelative ("TimeToLiveInSeconds");
				SerializedProperty DestroyWhenOutOfScreenProp 	= 	property.FindPropertyRelative ("DestroyWhenOutOfScreen");

				if (nameProp != null)
				{
					EditorGUI.PropertyField (position, nameProp);
					position.y += 17;
				}
				if (prefabProp != null)
				{
					EditorGUI.PropertyField (position, prefabProp);
					position.y += 17;
				}
				if (spriteProp != null)
				{
					EditorGUI.PropertyField (position, spriteProp);
					position.y += 17;
				}
				if (TimeToLiveInSecondsProp != null)
				{
					EditorGUI.PropertyField (position, TimeToLiveInSecondsProp);
					position.y += 17;
				}
				if (DestroyWhenOutOfScreenProp != null)
				{
					EditorGUI.PropertyField (position, DestroyWhenOutOfScreenProp);
					position.y += 17;
				}
			}
			EditorGUI.EndProperty ();
		}

		override public float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return 100;
		}

	}

	[CustomEditor ( typeof (BulletBank) )]
	public class BulletBankInspector : Editor
	{
		private GUIStyle title 								= new GUIStyle ("OL Title");
		private SerializedProperty	entries;
		private	Dictionary <string, bool>	entryFoldOut	=	new Dictionary<string, bool>();

		private void OnEnable ()
		{
			this.entries	=	serializedObject.FindProperty ("bullets");
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			EditorGUILayout.LabelField ("Bullets", EditorStyles.whiteBoldLabel);

			for (int i = 0; i < this.entries.arraySize; i++)
			{
				this.BulletEntryGUI (this.entries.GetArrayElementAtIndex (i) );
				EditorGUILayout.Space ();
			}

			serializedObject.ApplyModifiedProperties ();
		}

		private void BulletEntryGUI (SerializedProperty property)
		{
			SerializedProperty nameProp						=	property.FindPropertyRelative ("name");
			SerializedProperty prefabProp					=	property.FindPropertyRelative ("prefab");
			SerializedProperty spriteProp					=	property.FindPropertyRelative ("sprite");
			SerializedProperty TimeToLiveInSecondsProp 		= 	property.FindPropertyRelative ("TimeToLiveInSeconds");
			SerializedProperty DestroyWhenOutOfScreenProp 	= 	property.FindPropertyRelative ("DestroyWhenOutOfScreen");

			string 				entryName 					=	nameProp.stringValue;
			if (!this.entryFoldOut.ContainsKey ( entryName ) )
				this.entryFoldOut.Add ( entryName, true );

			if (this.entryFoldOut[entryName] )
				EditorGUILayout.BeginVertical ( "OL Box", GUILayout.Height (100f) );

			// Reserve room for title
			Rect headerRect = GUILayoutUtility.GetRect (new GUIContent (entryName), title);

			Event evt = Event.current;
			if (evt.type == EventType.MouseDown && evt.clickCount == 2 && headerRect.Contains (evt.mousePosition))
			{
				if (this.entryFoldOut[entryName] )
					EditorGUILayout.EndVertical ();
				this.entryFoldOut[entryName]	=	!this.entryFoldOut[entryName];
				Repaint ();
				return;
			}

			if (this.entryFoldOut[entryName] )
			{
				EditorGUILayout.PropertyField (prefabProp);
				EditorGUILayout.PropertyField (spriteProp);
				EditorGUILayout.PropertyField (TimeToLiveInSecondsProp);
				EditorGUILayout.PropertyField (DestroyWhenOutOfScreenProp);
			}

			GUI.Label (headerRect, new GUIContent (entryName), title);

			if (this.entryFoldOut[entryName] )
				EditorGUILayout.EndVertical ();
		}

	}
}
