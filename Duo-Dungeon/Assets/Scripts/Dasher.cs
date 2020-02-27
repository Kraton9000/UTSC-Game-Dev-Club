using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Completed
{
    public class Dasher : MovingObject
    {
        Rigidbody2D rigid;

        public string horizontalControl = "P1RightHorizontal";
        public string verticalControl = "P1RightVertical";
        public string actionControl = "leftBumper";
        public float dashMoveTime = 0.3f;
        private float lastX, lastY;
        private bool isDashing = false;     // dasher is attempting to dash

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
            joyStickHorizontal = Input.GetAxisRaw(horizontalControl);

            //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
            joyStickVertical = Input.GetAxisRaw(verticalControl);

            // If 1, then action button is held
            action = Input.GetButton(actionControl);

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

                if (action)
                {
                    Ability(horizontal, vertical);
                }
                else
                {
                    AttemptMove<Wall>(horizontal, vertical);
                }
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

        // code mostly copied from MovingObject
        protected override bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);
            LayerMask layers = blockingLayer;   // layers that dasher will be blocked by
            if (isDashing)
            {
                layers |= yDir > 0 ? (LayerMask) 0 : dashableLayerN;  // if going north
                layers |= yDir < 0 ? (LayerMask) 0 : dashableLayerS;  // if going south
                layers |= xDir < 0 ? (LayerMask) 0 : dashableLayerW;  // if going west
                layers |= xDir > 0 ? (LayerMask) 0 : dashableLayerE;  // if going east
            }
            else
            {
                layers |= dashableLayer | dashableLayerN | dashableLayerS | dashableLayerW | dashableLayerE;
            }
            hit = Physics2D.Linecast(start, end, layers);
            if (hit.transform == null && !isMoving)
            {
                // if dashing, fail the dash if there's something at the destination tile
                layers = blockingLayer | dashableLayer | dashableLayerN | dashableLayerS | dashableLayerW | dashableLayerE;
                if (isDashing && Physics2D.Linecast(end, end, layers).transform != null)  
                {
                    return false;
                }
                StartCoroutine(SmoothMovement(end));
                return true;
            }
            return false;
        }

        // code mostly copied from MovingObject
        protected override IEnumerator SmoothMovement(Vector3 end)
        {
            isMoving = true;
            float invMoveT = isDashing ? 1f / dashMoveTime : 1f / moveTime;  // use different speed for dashing
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            while (sqrRemainingDistance > float.Epsilon)
            {
                Vector3 newPostion = Vector3.MoveTowards(rigid.position, end, invMoveT * Time.deltaTime);
                rigid.MovePosition(newPostion);
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }
            rigid.MovePosition(end);
            isMoving = false;
            isDashing = false;
        }

        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
        protected override void OnCantMove<T>(T component)
        {

        }

        protected override void Ability(int horizontal, int vertical)
        {
            if (isFrozen)
            {
                //Debug.Log("dash frozen");
                return;
            }
            
            RaycastHit2D dummy;

            // don't dash if already dashing
            if (!isDashing)
            {
                isDashing = true;
                // try to dash 2 tiles forward
                // if can't dash 2 tiles forward, try dashing 1 tile forward
                // if can't, the entire dash fails
                if (!Move(horizontal*2, vertical*2, out dummy))
                {
                    if (!Move(horizontal, vertical, out dummy))
                    {
                        isDashing = false;
                    }
                }
            }
        }
    }
}