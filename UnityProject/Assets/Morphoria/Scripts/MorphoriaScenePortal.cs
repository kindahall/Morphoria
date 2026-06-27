using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaScenePortal : MonoBehaviour, IFormInteractable
    {
        public string label = "Portail";
        public string targetScene;
        public string targetLevelId;
        public bool requireInteraction = true;

        private bool loading;

        private void OnTriggerStay(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            if (!requireInteraction)
            {
                LoadTarget();
            }
        }

        public bool CanInteract(FormDefinition form)
        {
            return requireInteraction;
        }

        public string Hint(FormDefinition form)
        {
            return label;
        }

        public void Interact(MorphoriaPlayer player)
        {
            if (!requireInteraction)
            {
                return;
            }

            LoadTarget();
        }

        private void LoadTarget()
        {
            if (loading)
            {
                return;
            }

            loading = true;
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
