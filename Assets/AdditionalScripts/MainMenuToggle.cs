using MFPS.Runtime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuToggle : MonoBehaviour
{
    public GameObject HomeUI;

    void Start()
    {
        // Run after a short delay to override lobby settings
        StartCoroutine(InitializeAfterLobbyLoad());
    }

    IEnumerator InitializeAfterLobbyLoad()
    {
        // Delay to ensure all lobby scripts finish their initial setup
        yield return new WaitForSeconds(0.5f);

        // Ensure PlayerNameUI is deactivated and HomeUI is active
        if (HomeUI != null)
        {
            bl_LobbyLoadingScreenBase.Instance.HideIn(0.5f, true); //only if loading UI is turned on
            HomeUI.SetActive(true);
        }
    }
}
