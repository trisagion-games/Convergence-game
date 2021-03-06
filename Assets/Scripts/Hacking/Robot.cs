using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Robot : HackableObject {

    // Set to true when move command is set
    private bool canMove = false;
    // speed of robot in game
    public float movementSpeed;

    // for debugging moving state
    public bool moving = false;

    // for flipping robot 
    public bool facingRight = true;

    public CanvasGroup panOverlay;

    // to change animation states
    public Animator baseAnimator;

    // to move robot across 2D space
    private Rigidbody2D rigidBody;

    // to maintain moving if A or D is in contact
    private int movingKeys;

    private int horiz, vert;

    // runs at start of game
    void Start () {
        rigidBody = GetComponent<Rigidbody2D> ();
        movingKeys = 0;
        horiz = vert = 0;
        command_library = new Dictionary<string, Command> { { "move", new Move (GameObject.FindObjectOfType<Commands> (), this) } };
    }

    // runs every frame
    void Update () {

        // if not active,
        // no functionality available
        if (!active || !canMove)
            return;

        // variables needed to determine state changes
        int newMovingKeys = movingKeys; // Movement/Idle state change
        bool newDirRight = facingRight; // Left/right facing state change

        // For possible vertical control, here is base code
        // if (Input.GetKey(KeyCode.W))
        //     vert += 1;
        // if (Input.GetKey(KeyCode.S))
        //     vert -= 1;

        // Determine if robot is in state of moving or not
        // Also determine state change
        if (Input.GetKeyDown (KeyCode.A)) {
            newMovingKeys++;
            newDirRight = false;
            horiz -= 1;
        }
        if (Input.GetKeyDown (KeyCode.D)) {
            newDirRight = true;
            horiz += 1;
            newMovingKeys++;
        }
        if (Input.GetKeyUp (KeyCode.A)) {
            newMovingKeys--;
            horiz += 1;
            if (horiz > 0)
                newDirRight = true;
        }
        if (Input.GetKeyUp (KeyCode.D)) {
            newMovingKeys--;
            horiz -= 1;
            if (horiz < 0)
                newDirRight = false;
        }
        if (canMove && Input.GetKey (KeyCode.Q)) {
            canMove = false;
            panOverlay.alpha = 0f;
            panOverlay.blocksRaycasts = false;
            terminal.PrintLine("<color=\"blue\">Robot Move Disabled.</color>");
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<Terminal>().input.gameObject);
        }

        // Flip Robot
        if (newDirRight != facingRight) {
            facingRight = newDirRight;
            foreach (Transform t in GetComponentsInChildren<Transform> ()) {
                if (t != transform && t.GetComponent<Camera> () == null) {
                    t.Rotate (new Vector2 (0, 180));
                }
            }
        }

        // Movement Animation
        if (newMovingKeys != movingKeys) {
            // only one key (A or D), not both
            // must be active
            if (newMovingKeys == 1) {
                baseAnimator.ResetTrigger ("Idle");                                                
                baseAnimator.SetTrigger ("Motion");
                baseAnimator.SetBool ("Moving", moving = true);
                // AudioHelper.PlaySound("robot", true);
            } else {
                baseAnimator.ResetTrigger ("Motion");
                baseAnimator.SetTrigger ("Idle");
                baseAnimator.SetBool ("Moving", moving = false);
                // AudioHelper.Stop();
            }
            movingKeys = newMovingKeys;
        }

        // Vector Change
        Vector2 move = new Vector2 (horiz, vert);
        move = move.normalized * movementSpeed * Time.deltaTime;
        rigidBody.MovePosition (rigidBody.position + move);
    }

    class Move : Command {

        private Robot robotRef;
        public Move (Commands com, Robot robot) : base (com) {
            name = "move";
            description = "turns move mode on. Move robot left with A, right with D. Press Q to quit.";
            usage = "move";
            robotRef = robot;
        }

        public override void Action (string[] args) {
            robotRef.canMove = true;
            robotRef.panOverlay.alpha = 1f;
            robotRef.panOverlay.blocksRaycasts = true;
            EventSystem.current.SetSelectedGameObject (null);
            comRef.PrintToTerminal ("<color=\"blue\">Robot Move Enabled. Press A and D to move</color>");
        }
    }

}