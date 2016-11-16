using UnityEngine;

/// <summary>
/// Moves the camera to track a target game object.
/// Must be placed in the same GameObject as the Camera. 
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour {
    /// <summary>
    /// Transform of the GameObject that the camera should follow.
    /// </summary>
    public Transform Target;

    /// <summary>
    /// Controls how fast the camera relocates to the position of the Target.
    /// 0 means snap to Target's position (making camera jerky), large number
    /// means smoother motion.
    /// </summary>
    public float Smoothing = 5f;

    /// <summary>
    /// Camera moves to target's position + this offset
    /// Set automatically to the initial offset between the camera and target in Start().
    /// 
    /// Need this so that the player isn't forced to be in the exact center of the screen.
    /// </summary>
    private Vector3 offsetFromTarget;

    /// <summary>
    /// Initialize
    /// </summary>
	internal void Start () {
        // Zoom out when the score goes up
	    FindObjectOfType<Score>().ScoreUp += ZoomOut;
        offsetFromTarget = transform.position - Target.position;
	    FindObjectOfType<Runner>().FellIntoTheVoid += () => { enabled = false; };
	}

    /// <summary>
    /// Zoom out a little
    /// </summary>
    private void ZoomOut(){
        Camera.main.orthographicSize++;
    }

	internal void Update()
	{
		transform.position = Vector3.Lerp (transform.position, Target.position + offsetFromTarget, Smoothing * Time.deltaTime);
	}
}
