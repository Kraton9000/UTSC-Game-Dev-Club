using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Completed
{
    // Copied over from freeze mage
    public class Teleporter : MovingObject
    {
        Rigidbody2D rigid;

        // which move to teleport to
        private int spacesMoved = 0;
        private float[] pastX, pastY;
        public string horizontalControl = "P2RightHorizontal";
        public string verticalControl = "P2RightVertical";
        public string actionControl = "leftBumper";
        private float lastX, lastY;
        public GameObject childSquare; // Teleporter square
        //Start overrides the Start function of MovingObject
        protected override void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            // Keep track of where we've been.
            pastX = new float[] { rigid.position.x, rigid.position.x, rigid.position.x, rigid.position.x, rigid.position.x };
            pastY = new float[] { rigid.position.y, rigid.position.y, rigid.position.y, rigid.position.y, rigid.position.y };
            childSquare.transform.position = new Vector2(pastX[mod(spacesMoved, pastX.Length)], pastY[mod(spacesMoved, pastY.Length)]);

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
                return false;
            } else
            {
                // Mark the square when they are stablelized
                if (rigid.position.x == Mathf.Floor(rigid.position.x) && rigid.position.y == Mathf.Floor(rigid.position.y))
                {
                    // Make sure they have actually moved
                    if (rigid.position.x != pastX[mod(spacesMoved - 1, pastX.Length)] || rigid.position.y != pastY[mod(spacesMoved - 1, pastY.Length)])
                    {
                        pastX[mod(spacesMoved, pastX.Length)] = rigid.position.x;
                        pastY[mod(spacesMoved, pastY.Length)] = rigid.position.y;
                        ++spacesMoved;
                    }
                    childSquare.transform.position = new Vector2(pastX[mod(spacesMoved, pastX.Length)], pastY[mod(spacesMoved, pastY.Length)]);
                }
            }

            //Set the playersTurn boolean of GameManager to false now that players turn is over.
            GameManager.instance.playersTurn = false;
            return true;
        }
        //Ignore the function
        protected override void OnCantMove<T>(T component)
        {

        }

        protected override void Ability(int horizontal = 0, int vertical = 0)
        {
            if (isFrozen)
            {
                return;
            }
            rigid.MovePosition(new Vector2(pastX[mod(spacesMoved, pastX.Length)], pastY[mod(spacesMoved, pastY.Length)]));

            // Save location
            pastX[mod(spacesMoved, pastX.Length)] = rigid.position.x;
            pastY[mod(spacesMoved, pastY.Length)] = rigid.position.y;
            ++spacesMoved;
            childSquare.transform.position = new Vector2(pastX[mod(spacesMoved, pastX.Length)], pastY[mod(spacesMoved, pastY.Length)]);

            StartCoroutine(TeleportBlink());
        }

        IEnumerator TeleportBlink()
        {
            spriteRend.enabled = false;
            childSquare.GetComponent<SpriteRenderer>().enabled = false;
            // wait half a second
            yield return new WaitForSeconds(0.5f);
            spriteRend.enabled = true;
            childSquare.GetComponent<SpriteRenderer>().enabled = true;
        }

        // Modulo with always positive numbers
        private int mod(float a, float b)
        {
            return (int)(a - b * Mathf.Floor(a / b));
        }

        public override void die()
        {
            Debug.Log("Teleporter");
            base.die();
        }
    }
}