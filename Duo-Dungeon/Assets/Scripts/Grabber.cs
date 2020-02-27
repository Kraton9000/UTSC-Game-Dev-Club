using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Completed
{
    public class Grabber : MovingObject
    {
        Rigidbody2D rigid;

        public string horizontalControl = "P1RightHorizontal";
        public string verticalControl = "P1RightVertical";
        public string actionControl = "leftBumper";
        public int maxGrabLength = 5;
        public float dashMoveTime = 0.2f;
        private float lastX, lastY;
        public LayerMask ringLayer;         //Layer with the grabbable rings.
        public LayerMask playerLayer;         //Layer with the players rings.


        //Start overrides the Start function of MovingObject
        protected override void Start()
        {
            Debug.Log("H");

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
            joyStickHorizontal = Input.GetAxisRaw(horizontalControl);

            //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
            joyStickVertical = Input.GetAxisRaw(verticalControl);

            // If 1, then action button is pressed
            action = Input.GetButtonDown(actionControl);

            if (joyStickHorizontal < -0.15)
            {
                horizontal = -1;
                lastX = -1;
            }
            else if (joyStickHorizontal > 0.15)
            {
                horizontal = 1;
                lastX = 1;
            }

            if (joyStickVertical < -0.15)
            {
                vertical = -1;
                lastY = -1;
            }
            else if (joyStickVertical > 0.15)
            {
                vertical = 1;
                lastY = 1;
            }

            //Check if moving horizontally, if so set vertical to zero.
            if (horizontal != 0)
            {
                vertical = 0;
                lastY = 0;
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
                        lastY = 0;
                    }
                    else
                    {
                        horizontal = 0;
                        lastX = 0;
                    }
                } else if (horizontal != 0)
                {
                    vertical = 0;
                    lastY = 0;
                }
                else if (vertical != 0)
                {
                    horizontal = 0;
                    lastX = 0;
                }

                //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                //Pass in horizontal and vertical as parameters to specify the direction to move Player in.

                AttemptMove<Wall>(horizontal, vertical);
            }
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
            }
            //Set the playersTurn boolean of GameManager to false now that players turn is over.
            GameManager.instance.playersTurn = false;
            return false;
        }

        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
        protected override void OnCantMove<T>(T component)
        {

        }

        protected override void Ability(int horizontal = 0, int vertical = 0)
        {
            if (isFrozen)
            {
                return;
            }
            Vector2 start = transform.position;
            Vector2 end = start;
            // Checks the last direction faced
            if (lastX == -1) // Left
            {
                start += new Vector2(-1, 0);
                end = start + new Vector2(-maxGrabLength - 1, 0);
            } else if (lastX == 1) // right
            {
                start += new Vector2(1, 0);
                end = start + new Vector2(maxGrabLength - 1, 0);
            } else if (lastY == -1) // Down
            {
                start += new Vector2(0, -1);
                end = start + new Vector2(0, -maxGrabLength - 1);
            } else // Up (Default)
            {
                start += new Vector2(0, 1);
                end = start + new Vector2(0, maxGrabLength - 1);
            }
            LayerMask layers = ringLayer;
            layers |= playerLayer;
            RaycastHit2D hit = Physics2D.Linecast(start, end, layers);
            GameObject child = transform.GetChild(0).gameObject;
            Rope ropeScript = child.GetComponent<Rope>();
            if (hit.transform != null && !isMoving) // If a ring is hit
            {
                if (hit.transform.gameObject.layer == Mathf.RoundToInt(Mathf.Log(ringLayer.value, 2)))
                {
                    
                    Vector2 endLocation = hit.transform.gameObject.transform.position;
                    // Checks the last direction faced
                    if (lastX == -1) // Left
                    {
                        endLocation += new Vector2(1, 0);
                    }
                    else if (lastX == 1) // right
                    {
                        endLocation += new Vector2(-1, 0);
                    }
                    else if (lastY == -1) // Down
                    {
                        endLocation += new Vector2(0, 1);
                    }
                    else // Up (Default)
                    {
                        endLocation += new Vector2(0, -1);

                    }
                    
                    
                    
                    StartCoroutine(SmoothGrappleMovement(endLocation, ropeScript, hit.transform.gameObject.transform.position));
                } else if (hit.transform.gameObject.layer == Mathf.RoundToInt(Mathf.Log(playerLayer.value, 2)))
                {
                    Debug.Log("plauer");

                }
            } else if (!isMoving)
            {
                start = transform.position;
                layers = blockingLayer | dashableLayerN | dashableLayerS | dashableLayerW | dashableLayerE;
                hit = Physics2D.Linecast(start, end, layers);
                if (hit.transform)
                {
                    StartCoroutine(SmoothGrapple(ropeScript, hit.point));
                }
                else
                {
                    StartCoroutine(SmoothGrapple(ropeScript, end));
                }
            }
        }

        private IEnumerator SmoothGrapple(Rope ropeScript, Vector3 realEnd)
        {
            isMoving = true;
            float invMoveT = 1f / dashMoveTime;  // use different speed for dashing
            float sqrRemainingDistance = (transform.position - realEnd).sqrMagnitude;
            while (sqrRemainingDistance > float.Epsilon)
            {
                ropeScript.updateRope(transform.position, realEnd);
                Vector3 newPostion = Vector3.MoveTowards(realEnd, rigid.position, invMoveT * Time.deltaTime);
                realEnd = newPostion;
                sqrRemainingDistance = (transform.position - realEnd).sqrMagnitude;
                yield return null;
            }
            ropeScript.updateRope(transform.position, transform.position);
            isMoving = false;
        }

        // code mostly copied from MovingObject
        private IEnumerator SmoothGrappleMovement(Vector3 end, Rope ropeScript, Vector2 realEnd)
        {
            isMoving = true;
            Debug.Log(end);
            float invMoveT = 1f / dashMoveTime;  // use different speed for dashing
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            while (sqrRemainingDistance > float.Epsilon)
            {
                ropeScript.updateRope(transform.position, realEnd);
                Vector3 newPostion = Vector3.MoveTowards(rigid.position, end, invMoveT * Time.deltaTime);
                rigid.MovePosition(newPostion);
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }
            ropeScript.updateRope(transform.position, transform.position);

            rigid.MovePosition(end);
            isMoving = false;
        }
    }
}