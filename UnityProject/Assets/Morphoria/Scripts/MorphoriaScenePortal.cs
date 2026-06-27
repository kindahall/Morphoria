using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
    public sealed class MorphoriaScenePortal : MonoBehaviour
    {
        public string label = "Portail";
        public string targetScene;
        public string targetLevelId;
        public bool requireInteraction = true;

        private void OnTriggerStay(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            player.ShowFeedback(requireInteraction ? label + "  F" : label);
            if (!requireInteraction || Input.GetKeyDown(KeyCode.F))
            {
                MorphoriaGameSession session = MorphoriaGameSession.GetOrCreate();
                if (!string.IsNullOrEmpty(targetLevelId))
                {
                    session.LoadLevel(targetLevelId);
                }
                else
                {
                    session.LoadScene(targetScene);
                }
            }
        }
    }
}
