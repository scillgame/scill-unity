// Set an off-center projection, where perspective's vanishing
// point is not necessarily in the center of the screen.
//
// left/right/top/bottom define near plane size, i.e.
// how offset are corners of camera's near plane.
// Tweak the values and you can see camera's frustum change.

using System;
using UnityEngine;
using System.Collections;

namespace SCILL
{
	[ExecuteInEditMode]
	public class SCILLCameraOffset : MonoBehaviour
	{
		public float xOffset = 0.0F;
		public float yOffset = 0.0F;

		public void UpdateProjectionMatrix()
		{
			Camera cam = GetComponent<Camera>();
			Matrix4x4 m = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);

			//cam.projectionMatrix = m;
			m[0, 2] = xOffset;
			m[1, 2] = yOffset;

			cam.projectionMatrix = m;
		}

		void LateUpdate()
		{
			UpdateProjectionMatrix();
		}

		private void OnDisable()
		{
			Camera cam = GetComponent<Camera>();
			cam.ResetProjectionMatrix();
		}
	}
}

