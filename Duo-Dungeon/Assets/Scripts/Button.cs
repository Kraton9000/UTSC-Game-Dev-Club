using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Completed
{
    public class Button : ActionObject
    {
        public GameObject actionObj;
        public Sprite pressed;
        private Sprite defaultSprite;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            spriteRend = GetComponent<SpriteRenderer>();
            defaultSprite = spriteRend.sprite;
            callerObj = this.gameObject;
        }

        public override void Activate(GameObject caller)
        {
            if (frozen)
            {
                return;
            }

            NodeController nodeScript = actionObj.GetComponent<NodeController>() as NodeController;
            ActionObject actionObjScript = actionObj.GetComponent<ActionObject>() as ActionObject;
            if (nodeScript != null)
            {
                nodeScript.TryActivate();
            }
            else if (actionObjScript != null)
            {
                actionObjScript.Activate(this.gameObject);
            }
            spriteRend.sprite = pressed;
        }

        public override void Deactivate()
        {
            if (frozen)
            {
                return;
            }

            NodeController nodeScript = actionObj.GetComponent<NodeController>() as NodeController;
            ActionObject actionObjScript = actionObj.GetComponent<ActionObject>() as ActionObject;
            if (nodeScript != null)
            {
                nodeScript.TryDeactivate();
            }
            else if (actionObjScript != null)
            {
                actionObjScript.Deactivate();
            }
            spriteRend.sprite = defaultSprite;
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            isActive = true;
            Activate(this.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            isActive = false;
            Deactivate();
        }
    }
}