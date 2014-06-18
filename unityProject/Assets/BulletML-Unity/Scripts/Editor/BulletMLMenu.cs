#if UNITY_EDITOR
// Copyright © 2014 Pixelnest Studio
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package.
using UnityEditor;
using UnityEngine;
using Pixelnest;

namespace Pixelnest.BulletML
{
  public class BulletBankMenu : MonoBehaviour
  {
    [MenuItem("Assets/Create/BulletML/Bullet Bank")]
    public static void CreateAsset()
    {
      ScriptableObjectUtility.CreateAsset<BulletBank>();
    }
  }
}
#endif