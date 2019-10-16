using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JuiceLib
{
    public static class TimeFX
    {
        /// <summary>
        /// Bends time to the targetValue and then returns to the original timeScale.
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="duration"></param>
        /// <param name="easingType"></param>
        public static void BendTimeInOut(float targetValue, float duration, Ease easingType = Ease.InOutCubic)
        {
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, targetValue, duration).SetEase(easingType).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
        }

        public static void BendTime(float targetValue, float duration, Ease easingType = Ease.InOutCubic, System.Action onComplete = null)
        {
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, targetValue, duration).SetEase(easingType).SetUpdate(true).OnComplete(() =>
            {
                if (onComplete != null) onComplete.Invoke();
            });
        }
    }

}