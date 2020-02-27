using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Completed
{
    public class FreezeMage : MovingObject
    {
        Rigidbody2D rigid;

        public string horizontalControl = "P1RightHorizontal";
        public string verticalControl = "P1RightVertical";
        public string actionControl = "leftBumper";
        private float lastX, lastY;
        private List<string> arrayOfFreezableTags = new List<string> { "button", "spike", "Player" };
        private HashSet<GameObject> frozenObjects = new HashSet<GameObject>();
        private bool freezeAbilityOn = false;
        private bool needRefreeze = false;

        //Start overrides the Start function of MovingObject
        protected override void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            base.Start();
        }



        private void Update()
        {


            int horizontal = 0;     //Used to store the horizontal move direction.
            int vertical = 0;		//Used to store the vertical move direction.
            float joyStickHorizontal = 0;     //Used to store the horizontal move direction.
            float joyStickVertical = 0;       //Used to store the vertical move direction.
            bool action = false;

            //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
            //Ranges from -1 to 1
            joyStickHorizontal = Input.GetAxisRaw(horizontalControl);

            //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
            //Ranges from -1 to 1
            joyStickVertical = Input.GetAxisRaw(verticalControl);

            // If 1, then action button is pressed
            action = Input.GetButtonDown(actionControl);

            // Checks which direction the joysticks are in
            if (joyStickHorizontal < -0.15)
            {
                horizontal = -1;
            }
            else if (joyStickHorizontal > 0.15)
            {
                horizontal = 1;
            }

            if (joyStickVertical < -0.15)
            {
                vertical = -1;
            }
            else if (joyStickVertical > 0.15)
            {
                vertical = 1;
            }

            //Check if moving horizontally, if so set vertical to zero.
            if (horizontal != 0)
            {
                vertical = 0;
            }

            //Check if we have a non-zero value for horizontal or vertical
            if (horizontal != 0 || vertical != 0)
            {
                // If the joystick is between two directions
                if (horizontal != 0 && vertical != 0)
                {
                    if (joyStickHorizontal > joyStickVertical)
                    {
                        vertical = 0;
                    }
                    else
                    {
                        horizontal = 0;
                    }
                }

                //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one
                //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                AttemptMove<Wall>(horizontal, vertical);
            }

            // If refreeze required
            if (needRefreeze && freezeAbilityOn)
            {
                needRefreeze = false;

                // Do this only when they are stablelized
                if (rigid.position.x == Mathf.Floor(rigid.position.x) && rigid.position.y == Mathf.Floor(rigid.position.y))
                {

                }
            }

            // Checks if the action button is pressed
            if (action)
            {
                Ability();
            }
        }

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        //Return true means player is able to move, false means player can't move
        protected override bool AttemptMove<T>(int xDir, int yDir)
        {

            //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
            if (!base.AttemptMove<T>(xDir, yDir))
            {
                return true;
            } else
            {
                // Refreeze required
                needRefreeze = true;
            }

            //Set the playersTurn boolean of GameManager to false now that players turn is over.
            GameManager.instance.playersTurn = false;
            return false;
        }
        //Ignore the function
        protected override void OnCantMove<T>(T component)
        {

        }

        protected override void Ability(int horizontal = 0, int vertical = 0)
        {
            if (!freezeAbilityOn)
            {
                // This gets a list of all the objects that has the Collider2D component attached to it (players, spikes, buttons, unpassible tiles, etc...)
                Collider2D[] list = Physics2D.OverlapAreaAll(new Vector2(rigid.transform.position.x - 1, rigid.transform.position.y + 1), new Vector2(rigid.transform.position.x + 1, rigid.transform.position.y - 1));
                // Loops through all of the collidable objects around the time wizard
                foreach (Collider2D col2d in list)
                {
                    // If the object is in the list of freezable objects that was defined near the top
                    if (arrayOfFreezableTags.Contains(col2d.tag))
                    {
                        // Gets the game object
                        GameObject gameObj = col2d.gameObject;
                        // Adds the game object to the current list of frozen game objects that the player froze
                        frozenObjects.Add(gameObj);
                        
                        ActionObject actionObjScript;
                        // Checks which object is being frozen by checking the tag
                        switch (col2d.tag)
                        {
                            case "spike":
                                Debug.Log("froze spike");

                                actionObjScript = gameObj.GetComponent<ActionObject>();
                                actionObjScript.freezeObj(true);
                                break;
                            case "button":
                                Debug.Log("froze button");
                                actionObjScript = gameObj.GetComponent<ActionObject>();
                                actionObjScript.freezeObj(true);
                                break;
                            case "Player":
                                // If its not the freeze mage
                                if (gameObj != this.gameObject)
                                {
                                    MovingObject movingObjScript = gameObj.GetComponent<MovingObject>();
                                    movingObjScript.freeze();
                                } else
                                {
                                    // Remove freeze mage from frozen objects
                                    frozenObjects.Remove(gameObj);
                                }
                                // Freeze movement
                                break;
                        }
                    }
                }
                freezeAbilityOn = true;
            }
            else // Handles the unfreezing of objects
            {

                freezeAbilityOn = false;
                
                foreach (GameObject gameObj in frozenObjects)
                {
                    ActionObject actionObjScript;
                    // Checks which object is being unfrozen
                    switch (gameObj.tag)
                    {
                        case "spike":
                            Debug.Log("unfroze spike");
                            actionObjScript = gameObj.GetComponent<ActionObject>();
                            actionObjScript.freezeObj(false);
                            break;
                        case "button":
                            Debug.Log("unfroze button");
                            actionObjScript = gameObj.GetComponent<ActionObject>();
                            actionObjScript.freezeObj(false);
                            break;
                        case "Player":
                            // unfreeze movement
                            MovingObject movingObjScript = gameObj.GetComponent<MovingObject>();
                            movingObjScript.unFreeze();
                            break;
                    }
                }
                // Clears the list of frozen objects
                frozenObjects.Clear();
            }
        }

        private void ReAbility()
        {
            // This gets a list of all the objects that has the Collider2D component attached to it (players, spikes, buttons, unpassible tiles, etc...)
            HashSet<Collider2D> farObjects = new HashSet<Collider2D>(Physics2D.OverlapAreaAll(new Vector2(rigid.transform.position.x - 2, rigid.transform.position.y + 2), new Vector2(rigid.transform.position.x + 2, rigid.transform.position.y - 2)));
            HashSet<Collider2D> nearObjects = new HashSet<Collider2D>(Physics2D.OverlapAreaAll(new Vector2(rigid.transform.position.x - 1, rigid.transform.position.y + 1), new Vector2(rigid.transform.position.x + 1, rigid.transform.position.y - 1)));

            // Loops through all of the collidable objects around the time wizard
            foreach (Collider2D col2d in farObjects)
            {
                // If the object is in the list of freezable objects that was defined near the top
                if (arrayOfFreezableTags.Contains(col2d.tag))
                {
                    // Gets the game object
                    GameObject gameObj = col2d.gameObject;

                    // Based on the object location, we either freeze, unfreeze, or do nothing.
                    if (nearObjects.Contains(col2d))
                    {
                        // Freeze if not yet frozen
                        // Unfreeze if frozen
                        if (!frozenObjects.Contains(gameObj))
                        {
                            // Adds the game object to the current list of frozen game objects that the player froze
                            frozenObjects.Add(gameObj);

                            ActionObject actionObjScript;
                            // Checks which object is being frozen by checking the tag
                            switch (col2d.tag)
                            {
                                case "spike":
                                    Debug.Log("froze spike");

                                    actionObjScript = gameObj.GetComponent<ActionObject>();
                                    actionObjScript.freezeObj(true);
                                    break;
                                case "button":
                                    Debug.Log("froze button");
                                    actionObjScript = gameObj.GetComponent<ActionObject>();
                                    actionObjScript.freezeObj(true);
                                    break;
                                case "Player":
                                    // If its not the freeze mage
                                    if (gameObj != this.gameObject)
                                    {
                                        MovingObject movingObjScript = gameObj.GetComponent<MovingObject>();
                                        movingObjScript.freeze();
                                    }
                                    else
                                    {
                                        // Remove freeze mage from frozen objects
                                        frozenObjects.Remove(gameObj);
                                    }
                                    // Freeze movement
                                    break;
                            }
                        }
                    } else
                    {
                        // Unfreeze if frozen
                        if (frozenObjects.Contains(gameObj))
                        {
                            frozenObjects.Remove(gameObj);

                            ActionObject actionObjScript;
                            // Checks which object is being unfrozen
                            switch (gameObj.tag)
                            {
                                case "spike":
                                    Debug.Log("unfroze spike");
                                    actionObjScript = gameObj.GetComponent<ActionObject>();
                                    actionObjScript.freezeObj(false);
                                    break;
                                case "button":
                                    Debug.Log("unfroze button");
                                    actionObjScript = gameObj.GetComponent<ActionObject>();
                                    actionObjScript.freezeObj(false);
                                    break;
                                case "Player":
                                    // unfreeze movement
                                    MovingObject movingObjScript = gameObj.GetComponent<MovingObject>();
                                    movingObjScript.unFreeze();
                                    break;
                            }
                        }
                    }
                }
            }
        }

        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
        protected override IEnumerator SmoothMovement(Vector3 end)
        {
            //The object is now moving.
            isMoving = true;

            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                rb2D.MovePosition(newPostion);

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }

            //Make sure the object is exactly at the end of its movement.
            rb2D.MovePosition(end);

            // Refreeze objects if necessary
            if (freezeAbilityOn)
            {
                ReAbility();
            }

            //The object is no longer moving.
            isMoving = false;
        }
    }
}