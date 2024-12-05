using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Addon
{
    public class bl_CasingReloadHandler : MonoBehaviour
    {
        [Serializable]
        public class Shell
        {
            public Transform AnimShell;
            public Transform PhysicShell;

            public void SetPhysicToOrigin()
            {
                if (PhysicShell == null) return;

                PhysicShell.position = AnimShell.position;
                PhysicShell.rotation = AnimShell.rotation;
            }
        }

        public GameObject physicRoot;
        public Shell[] shells;

        /// <summary>
        /// 
        /// </summary>
        public void StartPhysic()
        {
            foreach (var shell in shells)
            {
                shell.SetPhysicToOrigin();
                shell.AnimShell.gameObject.SetActive(false);
                if (shell.PhysicShell) shell.PhysicShell.gameObject.SetActive(true);
            }
            if (physicRoot) physicRoot.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shellId"></param>
        public void ActiveShell(int shellId)
        {
            shells[shellId].AnimShell.gameObject.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Finish()
        {
            foreach (var shell in shells)
            {
                shell.SetPhysicToOrigin();
                shell.AnimShell.gameObject.SetActive(false);
                if (shell.PhysicShell) shell.PhysicShell.gameObject.SetActive(false);
            }
            if (physicRoot) physicRoot.SetActive(false);
        }
    }
}