using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Pixelnest.BulletML
{
	[CanEditMultipleObjects]
	[CustomEditor ( typeof (BulletManagerScript) )]
	public class BulletManagerInspector : Editor 
	{
		SerializedProperty player;
		SerializedProperty bulletBank;
		SerializedProperty bulletsParent;
		SerializedProperty useDefault;
		SerializedProperty scale;
		SerializedProperty timeSpeed;
		SerializedProperty difficulty;

		private void OnEnable ()
		{
			this.player			=	serializedObject.FindProperty ("player");
			this.bulletBank		=	serializedObject.FindProperty ("bulletBank");
			this.bulletsParent	=	serializedObject.FindProperty ("bulletsParent");
			this.useDefault		=	serializedObject.FindProperty ("useDefaultBulletIfMissing");
			this.scale			=	serializedObject.FindProperty ("scale");
			this.timeSpeed		=	serializedObject.FindProperty ("timeSpeed");
			this.difficulty		=	serializedObject.FindProperty ("gameDifficulty");
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			EditorGUILayout.PropertyField (this.player);
			EditorGUILayout.PropertyField (this.bulletBank);
			EditorGUILayout.PropertyField (this.bulletsParent);
			EditorGUILayout.PropertyField (this.useDefault);
			EditorGUILayout.Slider (this.scale, 0f, 1f);
			EditorGUILayout.Slider (this.timeSpeed, 0f, 1f);
			EditorGUILayout.Slider (this.difficulty, 0f, 1f);

			serializedObject.ApplyModifiedProperties ();
		}
	}
}
