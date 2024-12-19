using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Addon.PlayerPack
{
    public class PlayerPrefabInfo : MonoBehaviour
    {
        public List<PlayerInfo> players = new List<PlayerInfo>();

        [Serializable]
        public class PlayerInfo
        {
            public string Name;
            public GameObject Prefab;
            public Sprite Preview;
            public Team PreferedTeam = Team.All;
        }
    }
}