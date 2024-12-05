using System;
using UnityEngine;

namespace MFPS.Addon.WP2
{
    public class bl_KnifeAnimation : bl_WeaponAnimationBase
    {
        public Animator animator;
        public AnimationClip takeInAnimation;
        public AnimationClip takeOutAnimation;
        public AnimationClip[] fireAnimations;

        private int currentAnimation = 0;

        public override void CancelAnimation(WeaponAnimationType animationType)
        {

        }

        public override float GetAnimationDuration(WeaponAnimationType animationType, float[] data = null)
        {
            if (animationType == WeaponAnimationType.TakeIn) return takeInAnimation.length;
            else return fireAnimations[currentAnimation].length;
        }

        public override float PlayFire(AnimationFlags flags = AnimationFlags.None)
        {
            animator.Play(fireAnimations[currentAnimation].name, 0, 0);
            float length = fireAnimations[currentAnimation].length;
            currentAnimation = (currentAnimation + 1) % fireAnimations.Length;
            return length;
        }

        public override void PlayReload(float reloadDuration, int[] data, AnimationFlags flags = AnimationFlags.None, Action onFinish = null)
        {

        }

        public override float PlayTakeIn()
        {
            animator.SetFloat("DrawSpeed", 1);
            animator.Play("Draw", 0, 0);
            return takeInAnimation.length;
        }

        public override float PlayTakeOut()
        {
            animator.SetFloat("HideSpeed", 1);
            animator.CrossFade("Hide", 0.2f, 0);
            return takeOutAnimation.length;
        }
    }
}