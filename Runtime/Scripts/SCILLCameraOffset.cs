// Set an off-center projection, where perspective's vanishing
// point is not necessarily in the center of the screen.
//
// left/right/top/bottom define near plane size, i.e.
// how offset are corners of camera's near plane.
// Tweak the values and you can see camera's frustum change.

using UnityEngine;

namespace SCILL
{
	/// <summary>
	///     <para>
	///         In many UIs for challenges and battle passes you will want to render 3d models on the left or right side of
	///         the screen. This is also often the case for inventory and many other things. One solution to that is to create
	///         another camera, rendering to a render texture and using a <c>RawImage</c> element in the UI. We think the
	///         better approach is just to render a scene with a camera placing objects on the left or right.
	///     </para>
	///     <para>
	///         There is one caveat to this approach: perspective distortion.
	///     </para>
	///     <para>
	///         The solution to this is to change the projection matrix of the camera to offset the center of the screen. You
	///         can attach this script to your camera rendering the reward and change the Offset to adjust the objects position
	///         on the screen.
	///     </para>
	///     <para>
	///         Please note, the object should be in line with the camera (i.e. should have the same X value as the camera) to
	///         prevent perspective distortion.
	///     </para>
	/// </summary>
	[ExecuteInEditMode]
    public class SCILLCameraOffset : MonoBehaviour
    {
	    /// <summary>
	    ///     Sets the offset of the X-axis (left or right) to shift the perspective center to the left or right. Play around
	    ///     with this value it will be directly visible in the Unity Editor.
	    /// </summary>
	    public float xOffset;

	    /// <summary>
	    ///     Sets the offset of the Y-axis (up or down) to shift the perspective center to the left or right. Play around with
	    ///     this value it will be directly visible in the Unity Editor.
	    /// </summary>
	    public float yOffset;

        private void LateUpdate()
        {
            UpdateProjectionMatrix();
        }

        private void OnDisable()
        {
            var cam = GetComponent<Camera>();
            cam.ResetProjectionMatrix();
        }

        private void UpdateProjectionMatrix()
        {
            var cam = GetComponent<Camera>();
            var m = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);

            //cam.projectionMatrix = m;
            m[0, 2] = xOffset;
            m[1, 2] = yOffset;

            cam.projectionMatrix = m;
        }
    }
}