using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SCILL.Effects
{
    /// <summary>
    /// Helper class for animating the alpha value of a given <c>UnityEngine.UI.Image</c> script. Is used
    /// to animate a flash effect in the UI, e.g. when a Personal Challenge's score gets updated. 
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class SCILLUIFlashAnimation : MonoBehaviour
    {
        /// <summary>
        /// The curve with which the alpha value is animated. Should have x-Axis values in range [0,1], as the <see cref="Duration"/> parameter
        /// drives the actual animation duration. y-Axis values should also be in range [0,1].
        /// </summary>
        [SerializeField] public AnimationCurve AlphaCurve;
        /// <summary>
        /// The duration of the animation.
        /// </summary>
        [Min(0.01f)] [SerializeField] public float Duration = 0.5f;

        private Image _flashImage;

        private bool _isAnimationRunning = false;

        private Coroutine _animationRoutine;

        // between [0,1]
        private float _animationLerp;

        private void Awake()
        {
            _flashImage = GetComponent<Image>();
            Assert.IsNotNull(_flashImage,
                "SCILLUIFlashAnimation on object " + gameObject.name + " requires a UnityEngine.UI.Image");
            SetImageAlpha(0.0f);
            Assert.IsFalse(Duration < float.Epsilon,
                "SCILLUIFlashAnimation on object " + gameObject.name + " can not have a Duration of < 0.");
        }


        /// <summary>
        /// Starts playing the flash animation from start.
        /// </summary>
        public void Play()
        {
            _animationLerp = 0.0f;
            if (!_isAnimationRunning)
            {
                _animationRoutine = StartCoroutine(AnimationTick());
            }
        }

        private IEnumerator AnimationTick()
        {
            _isAnimationRunning = true;
            while (_animationLerp < 1.0f)
            {
                _animationLerp += Time.deltaTime / Duration;

                float newAlpha = AlphaCurve.Evaluate(_animationLerp);
                SetImageAlpha(newAlpha);

                yield return null;
            }

            _isAnimationRunning = false;
        }

        private void SetImageAlpha(float newAlpha)
        {
            if (_flashImage)
            {
                Color color = _flashImage.color;
                color.a = newAlpha;
                _flashImage.color = color;
            }
        }
    }
}