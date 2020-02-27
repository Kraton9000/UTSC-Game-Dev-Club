using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Completed
{
    public abstract class ActionObject : MonoBehaviour
    {
        protected bool frozen = false;
        private Material material;
        protected SpriteRenderer spriteRend;
        protected GameObject callerObj;
        public bool isActive = false;

        //Protected, virtual functions can be overridden by inheriting classes.
        protected virtual void Start()
        {
            spriteRend = GetComponent<SpriteRenderer>();
            // makes a new instance of the material for runtime changes
            material = spriteRend.material;
        }

        public virtual void Activate(GameObject caller) {
            // Gets the game object that called this function
            if (callerObj == null)
            {
                callerObj = caller;
            }
        }

        // To be overwritten
        public abstract void Deactivate();

        /**
         * Sets the object to be frozen or unfrozen
         */
        public void freezeObj(bool isFrozen)
        {
            bool prevFrozen = frozen;
            frozen = isFrozen;
            // If the object is getting unfrozen
            if (prevFrozen == true && isFrozen == false)
            {
                spriteRend.color = Color.white;
                if (callerObj != null)
                {
                    NodeController nodeScript = callerObj.GetComponent<NodeController>() as NodeController;
                    ActionObject actionObjScript = callerObj.GetComponent<ActionObject>() as ActionObject;
                    if (nodeScript != null)
                    {
                        // if caller is active
                        if (nodeScript.IsActive())
                        {
                            // From the caller's script, call the activate function with the caller as the caller argument
                            nodeScript.TryActivate();
                        }
                        else
                        {
                            // From the caller's script, call the deactivate function
                            nodeScript.TryDeactivate();
                        }
                    } else if (actionObjScript != null)
                    {

                        // if caller is active
                        Debug.Log(callerObj);
                        Debug.Log(actionObjScript.status());
                        if (actionObjScript.status())
                        {
                            // From the caller's script, call the activate function with the caller as the caller argument
                            actionObjScript.Activate(callerObj);
                        }
                        else
                        {
                            // From the caller's script, call the deactivate function
                            actionObjScript.Deactivate();
                        }
                    }
                    
                } /*else
                {
                    Debug.Log("Error: caller obj not instanciated!");
                }*/
            }
            if (isFrozen)
            {
                spriteRend.color = Color.cyan;
            }
            
        }

        /**
         * Returns if the action object is currently active
         */ 
        public bool status()
        {
            return isActive; 
        }
    }
}