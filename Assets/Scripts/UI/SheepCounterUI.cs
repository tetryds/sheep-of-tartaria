using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sheep
{
    public class SheepCounterUI : NetworkBehaviour
    {
        [SerializeField] TextMeshProUGUI countText;
        [SerializeField] TextMeshProUGUI requiredText;

        [ClientRpc]
        public void UpdateCount(int count)
        {
            countText.text = count.ToString();
        }

        [ClientRpc]
        public void SetRequired(int count)
        {
            requiredText.text = count.ToString();
        }

        [ClientRpc]
        public void EnableUI()
        {
            gameObject.SetActive(true);
        }
    }
}
