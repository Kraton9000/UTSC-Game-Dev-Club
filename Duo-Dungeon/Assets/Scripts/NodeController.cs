using System.Linq;
using UnityEngine;
namespace Completed
{
    public class NodeController : MonoBehaviour
    {
        public Button[] inputs;
        public Spike[] outputs;

        public void TryActivate()
        {
            // If all tthe inputs are on
            if (IsActive())
            {
                // stabby stabby
                foreach (Spike nextSpike in outputs)
                {
                    nextSpike.Activate(this.gameObject);
                }
            }
        }

        public void TryDeactivate()
        {
            // If all tthe inputs are off
            if (!IsActive())
            {
                Debug.Log("t2");
                // stabby stabby
                foreach (Spike nextSpike in outputs)
                {
                    nextSpike.Deactivate();
                }
            }
        }

        public bool IsActive()
        {
            string debugtxt = "";
            foreach (Button nb in inputs)
            {
                debugtxt += nb.isActive + " : "
;
            }
            Debug.Log(debugtxt);
            return inputs.All(nextInput => nextInput.isActive);
        }
    }
}