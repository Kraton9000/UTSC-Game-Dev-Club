using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Completed {
    public class LevelController : MonoBehaviour
    {
        public Teleporter char1;
        public Dasher char2;
        public Grabber char3;
        public FreezeMage char4;
        public GameObject respawnPad;
        public GameObject endPad1;
        public GameObject endPad2;
        public GameObject endPad3;
        public GameObject endPad4;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            
            if (char1.checkIfDead())
            {
                    char1.respawn(respawnPad.transform.position);
            }
            if (char2.checkIfDead())
            {
                    char2.respawn(respawnPad.transform.position);
            }
            if (char3.checkIfDead())
            {
                    char3.respawn(respawnPad.transform.position);
            }
            if (char4.checkIfDead())
            {
                char4.respawn(respawnPad.transform.position);
            }
        }
    }
}