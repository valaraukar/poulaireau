// Copyright © 2014 Pixelnest Studio
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package.
using UnityEngine;
using System.Linq;
using System.Collections;

namespace Pixelnest.BulletML
{
  /// <summary>
  /// Script making the link between Unity and BulletML
  /// </summary>
  public class BulletManagerScript : MonoBehaviour, BulletMLLib.IBulletManager
  {
    /// <summary>
    /// The player instance in the scene
    /// </summary>
    public GameObject player;

    /// <summary>
    /// The bullet definitions to use
    /// </summary>
    public BulletBank bulletBank;

	/// <summary>
	/// The bullets parent.
	/// </summary>
	public GameObject bulletsParent;

    /// <summary>
    /// If the bullet is nto found in the bank, use a defautl one
    /// </summary>
    public bool useDefaultBulletIfMissing = false;

    /// <summary>
    /// BulletML scale (between 0f and 1f)
    /// </summary>
    public float scale = 0.5f;

    /// <summary>
    /// BulletML time speed (between 0f and 1f)
    /// </summary>
    public float timeSpeed = 1f;

    /// <summary>
    /// BulletML current game difficulty (between 0f and 1f)
    /// </summary>
    public float gameDifficulty = 0.1f;

    /// <summary>
    /// Custom player position handler
    /// </summary>
    public event System.Func<GameObject, Vector2> GetPlayerPosition;

    /// <summary>
    /// Custom creation function
    /// </summary>
    public event System.Func<GameObject, BulletObject> OnBulletCreated;

    /// <summary>
    /// Custom spawn function
    /// </summary>
    public event System.Func<BulletObject, string, BulletScript> OnBulletSpawned;

    /// <summary>
    /// Custom remove function
    /// </summary>
    public event System.Action<BulletMLLib.Bullet> OnBulletDestroyed;

    /// <summary>
    /// For trigger nodes
    /// </summary>
    public event System.Action<GameObject, string> OnTrigger;

    /// <summary>
    /// We need to run the game at 60 FPS so BulletML runs smoothly. Disable it at your own risks!
    /// </summary>
    private bool forceFrameRate = true;

    private float previousTimeSpeed, previousScale;

    void Awake()
    {
      // Init BulletML
      BulletMLLib.GameManager.GameDifficulty += () =>
      {
        return this.gameDifficulty;
      };

      // On mobile, the framerate is at 30 FPS by default.
      // As BulletML is made to run at 60 FPS, we fore the framerate
      if (this.forceFrameRate)
      {
        Application.targetFrameRate = 60;
      }
    }

    void Start()
    {
      // Check prefabs
      if (this.player == null && this.GetPlayerPosition == null) 
				Debug.LogError("Missing 'player' reference for BulletManagerScript!");
      if (this.bulletBank == null && this.OnBulletSpawned == null) 
				Debug.LogError("Missing 'bulletBank' for BulletManagerScript!");
    }

    void Update()
    {
     	// Clamp
		this.gameDifficulty = Mathf.Clamp(this.gameDifficulty, 0f, 1f);

		if (this.timeSpeed != this.previousTimeSpeed || this.scale != this.previousScale)
        	SetBulletProperties(timeSpeed, scale);

		this.previousTimeSpeed = this.timeSpeed;
		this.previousScale = this.scale;
    }

    /// <summary>
    /// New bullet creation
    /// </summary>
    /// <remarks>Not the bullet spawn, just the container creation, we don't know the bullet name here</remarks>
    /// <returns></returns>
    public BulletMLLib.Bullet CreateBullet(BulletMLLib.Bullet source, bool top)
    {
      // Try to get the parent
      GameObject 			gameObject 	= null;
      BulletSourceScript 	emitter 	= null;

      if (source is TopBullet)
      {
        emitter = ((TopBullet)source).Parent;
        if (emitter != null)
        {
          gameObject = emitter.gameObject;
        }
      }
      else if (source is BulletObject)
      {
        gameObject = ((BulletObject)source).Parent;
      }

      // Create a top bullet (weird case)
      if (top)
      {
        return new TopBullet(this, emitter);
      }
      else
      {
        // Create a bullet
        BulletObject bullet = null;
        if (this.OnBulletCreated != null)
        {
			bullet = this.OnBulletCreated(gameObject);
        }
        else
        {
          bullet = new BulletObject(this, gameObject);
        }
        bullet.OnBulletSpawned += this.BulletSpawnedHandler;

        return bullet;
      }
    }

    /// <summary>
    /// Bullet spawn. Intantiate appropriate prefab.
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="bulletName"></param>
    void BulletSpawnedHandler(BulletObject bullet, string bulletName)
    {
      BulletScript bulletScript = null;

      if (this.OnBulletSpawned != null)
      {
        bulletScript = this.OnBulletSpawned(bullet, bulletName);
      }
      else
      {
        bulletScript = this.CreateBulletFromBank(bullet, bulletName);
      }

      // Bullet properties
      bulletScript.Bullet = bullet;
      bulletScript.Bullet.Scale = scale;
      bulletScript.Bullet.TimeSpeed = timeSpeed;
    }

    /// <summary>
    /// Create a bullet using the bullet bank
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="bulletName"></param>
    /// <returns></returns>
    public BulletScript CreateBulletFromBank(BulletObject bullet, string bulletName)
    {
      // Look in the bank
      BulletBankEntry bankEntry = null;

      if (string.IsNullOrEmpty(bulletName) == false)
      {
        // Match bullet names
        foreach (var entry in bulletBank.bullets)
        {
          if (entry.name.ToLower() == bulletName.ToLower())
          {
            bankEntry = entry;
          }
        }
      }

      if (bankEntry == null)
      {
        if (this.useDefaultBulletIfMissing || string.IsNullOrEmpty(bulletName))
        {
          bankEntry = bulletBank.bullets.FirstOrDefault();
        }

        if (bankEntry == null)
        {
          Debug.LogError("Missing bank entry for bullet: " + bulletName + "!");
          return null;
        }
      }

      // Instatiate a prefab
      GameObject bulletGameObject = Instantiate(bankEntry.prefab, bullet.position, Quaternion.identity) as GameObject;

	  // Set the bullet in a parent
	  if (this.bulletsParent != null)
	  {
			bulletGameObject.transform.parent	=	this.bulletsParent.transform;
	  }

      // Change sprite if the field has been filled
      if (bankEntry.sprite != null)
      {
        SpriteRenderer spriteRenderer = bulletGameObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
          spriteRenderer.sprite = bankEntry.sprite;
        }
      }

      // Make sure we have the appropriate script
      BulletScript bulletScript = bulletGameObject.GetComponent<BulletScript>();
      if (bulletScript == null)
      {
        bulletScript = bulletGameObject.AddComponent<BulletScript>();
      }

      bulletScript.DestroyWhenOutOfScreen = bankEntry.DestroyWhenOutOfScreen;

      // Auto destruction
      if (bankEntry.TimeToLiveInSeconds > 0)
      {
        StartCoroutine(DestroyAfterTimeOut(bullet, bankEntry.TimeToLiveInSeconds));
      }

      return bulletScript;
    }

    private IEnumerator DestroyAfterTimeOut(BulletObject bullet, float duration)
    {
      yield return new WaitForSeconds(duration);
      if (bullet != null)
      {
        RemoveBullet(bullet);
      }
    }

    /// <summary>
    /// Player current position for the bullet
    /// </summary>
    /// <param name="sourceBullet"></param>
    /// <returns></returns>
    public Vector2 PlayerPosition(BulletMLLib.Bullet sourceBullet)
    {
      if (GetPlayerPosition != null)
      {
        // Find the parent
        GameObject requestSource = GetGameObjectFromBullet(sourceBullet);

        // Call the handler
        if (requestSource == null)
        {
          Debug.LogError("Unable to get the parent from " + sourceBullet);
        }
        return GetPlayerPosition(requestSource);
      }
      else if (player != null)
      {
        return player.transform.position;
      }
      else
      {
        return Vector2.zero;
      }
    }

    /// <summary>
    /// Clean bullet
    /// </summary>
    /// <param name="deadBullet"></param>
    public void RemoveBullet(BulletMLLib.Bullet deadBullet)
    {
      if (OnBulletDestroyed != null)
      {
        OnBulletDestroyed(deadBullet);
        return;
      }

      if (deadBullet is BulletObject)
      {
        BulletObject b = (BulletObject)deadBullet;

        if (b != null && b.Parent != null && b.Parent != null)
        {
          Destroy(b.Parent);
        }
      }
    }

    /// <summary>
    /// Update ALL bullets on screen with the given properties.
    /// </summary>
    /// <param name="timeSpeed"></param>
    /// <param name="scale"></param>
    public void SetBulletProperties(float timeSpeed, float scale)
    {
      BulletScript[] bullets = FindObjectsOfType<BulletScript>();
      foreach (BulletScript b in bullets)
      {
        b.Bullet.Scale = scale;
        b.Bullet.TimeSpeed = timeSpeed;
      }
    }

    /// <summary>
    /// Trigger anything from its name
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    public void Trigger(BulletMLLib.Bullet source, string name)
    {
      GameObject sourceObject = GetGameObjectFromBullet(source);

      if (OnTrigger != null)
      {
        OnTrigger(sourceObject, name);
      }
    }

    /// <summary>
    /// Look what kind if bullet it is and how to retrieve the game object from it
    /// </summary>
    /// <param name="sourceBullet"></param>
    /// <returns></returns>
    private static GameObject GetGameObjectFromBullet(BulletMLLib.Bullet sourceBullet)
    {
      GameObject requestSource = null;

      if (sourceBullet is TopBullet)
      {
        var parent = ((TopBullet)sourceBullet).Parent;
        if (parent != null)
        {
          requestSource = parent.gameObject;
        }
      }
      else if (sourceBullet is BulletObject)
      {
        requestSource = ((BulletObject)sourceBullet).Parent;
      }
      return requestSource;
    }
  }
}