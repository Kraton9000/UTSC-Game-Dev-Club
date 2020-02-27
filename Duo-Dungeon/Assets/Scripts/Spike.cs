using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Completed
{
    public class Spike : ActionObject
    {
        List<GameObject> onSpike = new List<GameObject>();
        public Animator anim;
        public Sprite pressed;
        public int damage = 10;
        private Sprite defaultSprite;
        public bool spikeUp = false;



        protected override void Start()
        {
            base.Start();
            anim = GetComponent<Animator>();
            defaultSprite = spriteRend.sprite;
        }

        public override void Activate(GameObject caller)
        {
            // saves the game object caller 
            base.Activate(caller);
            if (frozen)
            {
                return;
            }
            spikeUp = true;
            StartCoroutine(DamagePlayers());

            this.gameObject.layer = LayerMask.NameToLayer("BlockingLayer");
            spriteRend.sprite = pressed;
            isActive = true;
            // Saves the caller game object
        }

        public override void Deactivate()
        {
            if (frozen)
            {
                return;
            }

            spikeUp = false;

            this.gameObject.layer = LayerMask.NameToLayer("Default");
            spriteRend.sprite = defaultSprite;
            isActive = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                onSpike.Add(collision.gameObject);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                onSpike.Remove(collision.gameObject);
            }
        }

        IEnumerator DamagePlayers()
        {
            while (spikeUp)
            {
                // Print the entire list to the console.
                foreach (GameObject gObject in onSpike)
                {
                    MovingObject gObjectScript = gObject.GetComponent<MovingObject>();
                    gObjectScript.takeDamage(damage);
                }
                yield return new WaitForSeconds(0.8f);
            }
        }


    }
}