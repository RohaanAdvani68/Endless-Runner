using UnityEngine;
using System;

/// <summary>
/// Controller for the player
/// </summary>
public class Runner : MonoBehaviour {
    #region Fields that can be adjusted in the editor
    /// <summary>
    /// Rate at which character accelerates horizontally when running
    /// </summary>
    public float RunningAccelerationRate = 5f;
    /// <summary>
    /// Speed character at which rises when jumping
    /// </summary>
    public float JumpSpeed = 20f;
    /// <summary>
    /// Speed at which character falls
    /// (platformers don't usually implement acceleration due to gravity)
    /// </summary>
    public float Gravity = 0.5f;
    /// <summary>
    /// If the player drops beneath this Y coordinate, they're considered to have fallen into the void.
    /// The right way to do this is to compute the Y coordinate of the bottom of the screen, but we
    /// haven't shown you how to do that yet.
    /// </summary>
    public int BottomOfTheWorld = -60;
    #endregion

    #region Event notifications for other components
    /// <summary>
    /// Called when the character land on a platform
    /// </summary>
    public event Action LandedOnPlatform = delegate { };
    /// <summary>
    /// Called when the character falls into the void
    /// </summary>
    public event Action FellIntoTheVoid = delegate { };
    /// <summary>
    /// True if we've already called FellIntoTheVoid.
    /// </summary>
    private bool notifiedGameEnd;
    #endregion

    #region Internal state variables
    /// <summary>
    /// Kluge factor for aligning character with platform when they land.
    /// Its value is determined by the sizes of the character and the platform and where
    /// the origins of their coordinate systems are relative to their collider edges.
    /// Someday, this should get refactored to something cleaner.
    /// </summary>
    private float magicHeightOffset;

    /// <summary>
    /// Player's current velocity
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// True when player is on a platform
    /// </summary>
    private bool isTouchingPlatform;

    /// <summary>
    /// Iniitialization
    /// </summary>
    internal void Awake() {
        magicHeightOffset = (GetComponent<BoxCollider2D>().size.y + 1f) / 2f;
        velocity = new Vector3 (10f, 0f, 0f);
    }
    #endregion

    /// <summary>
    /// Snap character to the top of the platform at the specified position.
    /// We use this so that if a character hits the platform from the side, they
    /// "climb up" the platform.  It makes the game somewhat easier.
    /// </summary>
    /// <param name="platformPosition"></param>
    private void AlignToPlatform(Vector3 platformPosition)
    {
        var myPos = transform.position;
        myPos.y = platformPosition.y + magicHeightOffset;
        transform.position = myPos;
    }

	internal void Update()
	{
		if (isTouchingPlatform) {
			if (Input.GetButtonDown ("Jump"))
				velocity = velocity + new Vector3 (0, JumpSpeed, 0);
			else if (Input.GetButtonDown ("Short Jump"))
				velocity = velocity + new Vector3 (0, JumpSpeed / 2, 0);
			else {//touching platform but not jumping
				Vector3 sideways = new Vector3 (RunningAccelerationRate*Time.deltaTime, 0,0);
				velocity += sideways;
			}
		}
		else {
			Vector3 downward = new Vector3 (0, -Gravity*Time.deltaTime, 0);
			velocity = velocity + downward;
		}

		transform.position += (velocity * Time.deltaTime);//updating position

		if (transform.position.y<BottomOfTheWorld) {
			if (!notifiedGameEnd) {
				FellIntoTheVoid ();
				notifiedGameEnd = true;
			} else if (transform.position.y < (2 * BottomOfTheWorld)) {
				gameObject.SetActive (false);
			}
		}
	}

	internal void OnTriggerEnter2D(Collider2D obj){
		isTouchingPlatform = true;
		LandedOnPlatform ();
		AlignToPlatform (obj.transform.position);
		velocity.y = 0;
	}

	internal void OnTriggerExit2D(Collider2D obj){
		isTouchingPlatform = false;
	}
}
