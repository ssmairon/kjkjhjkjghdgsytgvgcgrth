using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_SoldierPreviewAnimation : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != 1 || bl_SoldierPreview.Instance.ActiveWeapon == null) return;
            if (bl_SoldierPreview.Instance.ActiveWeapon.leftHandIK == null) return;

            animator.SetIKRotation(AvatarIKGoal.LeftHand, bl_SoldierPreview.Instance.ActiveWeapon.leftHandIK.rotation);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, bl_SoldierPreview.Instance.ActiveWeapon.leftHandIK.position);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        }
    }
}