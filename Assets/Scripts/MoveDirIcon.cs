using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Sheep
{
    public class MoveDirIcon : MonoBehaviour
    {
        [SerializeField] Transform dirIcon;
        [SerializeField] MoveController moveController;

        public void ShowIcon(bool showIcon)
        {
            dirIcon.gameObject.SetActive(showIcon);
        }

        private void Update()
        {
            if (!dirIcon.gameObject.activeSelf) return;

            dirIcon.rotation = Quaternion.LookRotation(moveController.LookDir, Vector3.up);
        }
    }
}
