using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SCILL.Effects
{
    [RequireComponent(typeof(Image))]
    public class SCILLUIFlashAnimation : MonoBehaviour
    {
        [SerializeField] public AnimationCurve AlphaCurve;
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