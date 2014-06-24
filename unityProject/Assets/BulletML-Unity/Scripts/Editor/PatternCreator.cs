using UnityEngine;
using UnityEditor;
using System.Collections;

public class PatternCreator : EditorWindow 
{

	class Styles
	{
		public GUIContent m_WarningContent = new GUIContent (string.Empty, EditorGUIUtility.LoadRequired("Builtin Skins/Icons/console.warnicon.sml.png") as Texture2D);
		public GUIStyle m_PreviewBox = new GUIStyle ("OL Box");
		public GUIStyle m_PreviewTitle = new GUIStyle ("OL Title");
		public GUIStyle m_LoweredBox = new GUIStyle ("TextField");
		public GUIStyle m_HelpBox = new GUIStyle ("helpbox");
		public Styles ()
		{
			m_LoweredBox.padding = new RectOffset (1, 1, 1, 1);
		}
	}
	private static Styles m_Styles;
	private const int kButtonWidth = 120;

	private bool 	m_ClearKeyboardControl = false;
	private Vector2 m_PreviewScroll;


	#region Menu
	[MenuItem ("Assets/Create/Pattern...", false, 100)]
	private static void OpenFromAssetsMenu ()
	{
		Init ();
	}
	#endregion

	#region Init
	private static void Init ()
	{
		GetWindow<PatternCreator> (true, "Create Pattern");
	}
	
	public PatternCreator ()
	{
		// Large initial size
		position = new Rect (50, 50, 770, 500);
		// But allow to scale down to smaller size
		minSize = new Vector2 (550, 400);
	}
	#endregion

	private void OnEnable ()
	{

	}

	private void OnGUI ()
	{
		if (m_Styles == null)
			m_Styles = new Styles ();

		EditorGUIUtility.labelWidth = 85;
		
//		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && CanCreate ())
//			Create ();
		
		EditorGUILayout.BeginHorizontal ();
		{
			GUILayout.Space (10);
			
			this.PreviewGUI ();
			GUILayout.Space (10);
			
			EditorGUILayout.BeginVertical ();
			{
				this.OptionsGUI ();
				
				GUILayout.Space (10);
				
				this.CreateAndCancelButtonsGUI ();
			} 
			EditorGUILayout.EndVertical ();
			
			GUILayout.Space (10);
		} 
		EditorGUILayout.EndHorizontal ();
		
		GUILayout.Space (10);
		
		// Clear keyboard focus if clicking a random place inside the dialog,
		// or if ClearKeyboardControl flag is set.
		if (m_ClearKeyboardControl || (Event.current.type == EventType.MouseDown && Event.current.button == 0))
		{
			GUIUtility.keyboardControl = 0;
			m_ClearKeyboardControl = false;
			Repaint ();
		}
	}

	private void PreviewGUI ()
	{
		EditorGUILayout.BeginVertical (GUILayout.Width (Mathf.Max (position.width * 0.4f, position.width - 380f)));
		{
			// Reserve room for preview title
			Rect previewHeaderRect = GUILayoutUtility.GetRect (new GUIContent ("Preview"), m_Styles.m_PreviewTitle);
			
			// Secret! Toggle curly braces on new line when double clicking the script preview title
			Event evt = Event.current;
			if (evt.type == EventType.MouseDown && evt.clickCount == 2 && previewHeaderRect.Contains (evt.mousePosition))
			{
//				EditorPrefs.SetBool ("CurlyBracesOnNewLine", !EditorPrefs.GetBool ("CurlyBracesOnNewLine"));
				Repaint ();
			}
			
			// Preview scroll view
			m_PreviewScroll = EditorGUILayout.BeginScrollView (m_PreviewScroll, m_Styles.m_PreviewBox);
			{
				EditorGUILayout.BeginHorizontal ();
				{
					// Tiny space since style has no padding in right side
					GUILayout.Space (5);
					
					// Preview text itself
					TextAsset select = Selection.activeObject as TextAsset;
					if (select != null)
					{
						EditorGUILayout.TextArea (select.text, EditorStyles.label);
					}
				} EditorGUILayout.EndHorizontal ();
			} EditorGUILayout.EndScrollView ();
			
			// Draw preview title after box itself because otherwise the top row
			// of pixels of the slider will overlap with the title
			GUI.Label (previewHeaderRect, new GUIContent ("Preview"), m_Styles.m_PreviewTitle);
			
			GUILayout.Space (4);
		} EditorGUILayout.EndVertical ();
	}

	private void OptionsGUI ()
	{
	}

	private void CreateAndCancelButtonsGUI ()
	{	
		// Create string to tell the user what the problem is
		string blockReason = string.Empty;
		
		// Warning about why the script can't be created
		if (blockReason != string.Empty)
		{
			m_Styles.m_WarningContent.text = blockReason;
			GUILayout.BeginHorizontal (m_Styles.m_HelpBox);
			{
				GUILayout.Label (m_Styles.m_WarningContent, EditorStyles.wordWrappedMiniLabel);
			} GUILayout.EndHorizontal ();
		}
		
		// Cancel and create buttons
		GUILayout.BeginHorizontal ();
		{
			GUILayout.FlexibleSpace ();
			
			if (GUILayout.Button ("Cancel", GUILayout.Width (kButtonWidth)))
			{
				Close ();
				GUIUtility.ExitGUI ();
			}
			
			bool guiEnabledTemp = GUI.enabled;
			if (GUILayout.Button ("Create", GUILayout.Width (kButtonWidth)))
			{
			}
			GUI.enabled = guiEnabledTemp;
		} GUILayout.EndHorizontal ();
	}
}
