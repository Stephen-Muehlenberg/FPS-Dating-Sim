using UnityEngine;

/**
 * First Person view movement controller. Handles walking, running, jumping, mouselook, etc.
 * Each module can be enabled or disabled individually. Modules can also be set to ignore
 * input, useful for example if you want the player to stop moving but want to use the normal
 * deceleration.
 */
namespace FirstPersonModule {
  public class FirstPersonController : MonoBehaviour {
    [HideInInspector]
    private CharacterController characterController;
    [Tooltip("Transform the camera is attached to. Should be a child of the main character Transform to allow the head to be moved independently.")]
    public Transform cameraPivot;
    [Tooltip("Is the character currently off the ground?")]
    public bool inAir = false;
    [Tooltip("If true, the character won't be affected by movement, gravity or collisions. Useful for cutscenes.")]
    public bool isKinematic = false;
    public LookModule look;
    public MoveModule move;
    public JumpModule jump;
    public HeadbobModule headbob;
    public GravityModule gravity;

    public Vector3 velocity = Vector3.zero;

    [HideInInspector]
    public int collisionMask;

    public void Awake() {
      characterController = GetComponent<CharacterController>();
      collisionMask = ~LayerMask.GetMask("Weapons", "Invisible Wall", "Player"); // Ignore these layers
      look.init(this);
      move.init(this);
      jump.init(this);
      headbob.init(this);
      gravity.init(this);
      reset();
    }

    public void Update() {
      if (TimeUtils.mode == TimeUtils.TimeMode.PAUSED) return;

      // Check if there's anything beneath the player. The distance is equal to characterController.skinWidth (currently 0.035) + 0.02 for extra padding
      inAir = Physics.OverlapBox(transform.position + Vector3.down * 0.055f, new Vector3(0.25f, 0.06f, 0.25f), Quaternion.identity, collisionMask).Length == 0;
      
      look.update();
      move.update();
      jump.update();
      headbob.update();
      gravity.update();

      if (!isKinematic) characterController.Move(velocity * TimeUtils.dialogDeltaTime);
    }

    public void enableAllInput() {
      look.inputEnabled = true;
      move.inputEnabled = true;
      jump.inputEnabled = true;
    }

    public void disableAllInput() {
      look.inputEnabled = false;
      move.inputEnabled = false;
      jump.inputEnabled = false;
    }

    public void setState(bool lookEnabled = true,
                         bool moveEnabled = true,
                         bool jumpEnabled = true,
                         bool headbobEnabled = true,
                         bool gravityEnabled = true) {
      look.enabled = lookEnabled;
      move.enabled = moveEnabled;
      jump.enabled = jumpEnabled;
      headbob.enabled = headbobEnabled;
      gravity.enabled = gravityEnabled;
    }

    public void enableAll() {
      look.enabled = true;
      move.enabled = true;
      jump.enabled = true;
      headbob.enabled = true;
      gravity.enabled = true;
    }

    public void disableAll() {
      look.enabled = false;
      move.enabled = false;
      jump.enabled = false;
      headbob.enabled = false;
      gravity.enabled = false;
    }

    /**
     * Return everything to default state.
     */
    public void reset() {
      velocity = Vector3.zero;
      look.reset();
      move.reset();
      jump.reset();
      headbob.reset();
      gravity.reset();
      isKinematic = false;
    }
  }

  public abstract class ComponentModule {
    [HideInInspector]
    public FirstPersonController controller;
    public bool enabled; // If false, module won't update at all
    public bool inputEnabled; // If false, user can't take new actions, but existing actions will still be resolved, e.g. completing a jump

    public virtual void init(FirstPersonController controller) {
      this.controller = controller;
    }

    /*
     * Reset the module back to its initial state.
     */
    public virtual void reset() { }

    public abstract void update();
  }

  // -------------- MOVE --------------

  [System.Serializable]
  public class MoveModule : ComponentModule {
    [System.Serializable]
    public enum RunMode { ALWAYS_WALK, ALWAYS_RUN, HOLD_TO_RUN, HOLD_TO_WALK, PRESS_TO_TOGGLE }

    [System.Serializable]
    public struct MovementProfile {
      [Tooltip("Maximum speed the character can reach under their own power.")]
      public float speed;
      [HideInInspector]
      public float speedSq;
      [Tooltip("Acceleration while on the ground.")]
      public float acceleration;
      [Tooltip("How fast the character's movement returns to zero while on the ground.")]
      public float deceleration;
      [Tooltip("Acceleration while in the air.")]
      public float airAcceleration;
      [Tooltip("How fast the character's movement returns to zero while in the air.")]
      public float airDeceleration;
    }

    public RunMode mode;
    [Tooltip("Prevent player from moving below minHeight.")]
    public bool lockMinHeight = true;
    public float minHeight = 0;
    public MovementProfile walkProfile;
    public MovementProfile runProfile;
    [HideInInspector]
    public bool moving = false; // Is the player currently inputting any movement commands?
    [HideInInspector]
    public bool running = false; // Is the player currently running (as opposed to walking)?
    
    private MovementProfile currentProfile;

    // Cached to avoid re-allocation
    private Vector3 movementInput;
    private float currentAcceleration, currentDeceleration; // Current Accel/Decel value to use, depending on whether we're in air or not
    private float newSpeedSq, oldSpeedSq;
    private Vector3 velocityDelta; // Speed change based on input
    private Vector3 velocityTangent, sidewaysMovement, oldHorizontalVelocity, newHorizontalVelocity;
    private Quaternion rightAnglesRotation = Quaternion.Euler(0, 90, 0);
    private Vector3 deceleration;
    private bool runButtonDown;

    public override void init(FirstPersonController controller) {
      base.init(controller);
      recalculateProfileValues();
    }

    public override void reset() {
      moving = false;
      running = false;
    }

    /**
     * Some profile values are derived from other values. If the profile is changed at runtime, this
     * method must be called to ensure derived values are updated accordingly.
     */
    public void recalculateProfileValues() {
      walkProfile.speedSq = walkProfile.speed * walkProfile.speed;
      runProfile.speedSq = runProfile.speed * runProfile.speed;
    }

    public void setMode(RunMode mode) {
      if (this.mode == mode) return;
      this.mode = mode;
    }

    public override void update() {
      if (!enabled) return;

      // MINIMUM Y POSITION
      if (lockMinHeight && controller.transform.position.y < minHeight) {
        controller.transform.position = new Vector3(controller.transform.position.x, minHeight, controller.transform.position.z);
        controller.velocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
      }

      // TODO see if we can optimize this further by just listening to button up / down events
      runButtonDown = Input.GetButton("Run");
      running = ((mode == RunMode.ALWAYS_RUN) || (runButtonDown && mode == RunMode.HOLD_TO_RUN) || (!runButtonDown && mode == RunMode.HOLD_TO_WALK));
      currentProfile = running ? runProfile : walkProfile;

      // INPUT
      if (inputEnabled) movementInput =
        (controller.transform.forward * Input.GetAxis("Vertical")) +
        (controller.transform.right * Input.GetAxis("Horizontal"));
      else movementInput = Vector3.zero;

      moving = movementInput != Vector3.zero;

      if (moving) {
        currentAcceleration = controller.inAir ? currentProfile.airAcceleration : currentProfile.acceleration;
        velocityDelta = movementInput.normalized * currentAcceleration * TimeUtils.gameplayDeltaTime;

        // Ignore vertical component
        newSpeedSq = (controller.velocity + velocityDelta).horizontalSqMagnitude();
        oldSpeedSq = controller.velocity.horizontalSqMagnitude();

        // If velocity plus velocity delta has a magnitude greater than max speed, just apply the side-to-side movement, ignoring forward movement.
        if (newSpeedSq > currentProfile.speedSq) {
          // Get the normalized direction of the tangent of the current velocity.
          velocityTangent = (rightAnglesRotation * controller.velocity).horizontalNormalized();
          // Project the input onto the tangent, so we get ONLY the side-to-side movement.
          sidewaysMovement = Vector3.Project(velocityDelta, velocityTangent);
          // Subtract vertical motion because we only want to modify horizontal movement.
          oldHorizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
          // Add the side-to-side movement to the current movement, then scale the result to move at max speed.
          newHorizontalVelocity = (oldHorizontalVelocity + sidewaysMovement).normalized * currentProfile.speed;
          // Add the vertical motion back in after modifying horizontal movement.
          controller.velocity = newHorizontalVelocity + (Vector3.up * controller.velocity.y);
        }

        // Otherwise just add delta to velocity
        else {
          controller.velocity += velocityDelta;
        }

        // Only apply velocity change if it's either slowing you down OR it's speeding you up but not beyond your max speed.
        // This prevents you accelerating, e.g. if something is pushing you forward rapidly - moving back or to the side will work,
        // but you can't get any more forward velocity.
        //    if (newSpeedSq < oldSpeedSq || newSpeedSq < currentProfile.speedSq) { // TODO cache currentProfile.speed ^2
        //    velocity += velocityDelta;
        //}
        // TODO if velocityDelta would push it over the speed limit but we're not CURRENTLY at the limit, increase to (but not over) the limit
      }

      if (controller.velocity == Vector3.zero) return; // No need to decelerate if we're not moving

      // DECELERATION
      currentDeceleration = controller.inAir ? currentProfile.airDeceleration : currentProfile.deceleration;

      deceleration = controller.velocity.horizontalNormalized() * -currentDeceleration;

      // If the player is accelerating, check if any part of the deceleration goes against this acceleration.
      // If so, remove that part of the decel by projecting it onto a tangent of the accel direction. This
      // leaves us with just the side-to-side deceleration, and none of the backwards deceleration.
      if (moving && Vector3.Angle(movementInput, deceleration) > 90) {
        var accelerationTangent = (Quaternion.Euler(0, 90, 0) * movementInput).normalized * currentDeceleration; // Right-angles to the current direction of acceleration
        deceleration = Vector3.Project(deceleration, accelerationTangent);
      }

      deceleration *= TimeUtils.gameplayDeltaTime;

      // If deceleration reduces horizontal speed to < 0 (moving backwards), set horizontal speed to exactly 0.
      var sqMagnitudeAfterDeceleration = controller.velocity.horizontalSqMagnitude() - deceleration.sqrMagnitude;
      if (sqMagnitudeAfterDeceleration > 0) controller.velocity += deceleration;
      else controller.velocity = new Vector3(0, controller.velocity.y, 0);
    }
  }

  // -------------- LOOK --------------

  [System.Serializable]
  public class LookModule : ComponentModule {
    public void restrictCameraToDirection(Vector3 direction, float maxRotation) {
      // TODO
    }

    // Cached to avoid re-allocation
    private float input;
    private float verticalRotation;

    public override void update() {
      if (!enabled || !inputEnabled) return;
      
      input = Input.GetAxis("Mouse X");
      if (input != 0) controller.gameObject.transform.Rotate(0, input * Settings.mouseSensitivity * TimeUtils.dialogDeltaTime, 0);

      input = Input.GetAxis("Mouse Y");
      if (input != 0) {
        // 0 = straight ahead, 90 = down, 270 = up
        verticalRotation = controller.cameraPivot.localRotation.eulerAngles.x;

        // Make sure camera doesn't rotate beyond straight up or down
        if (verticalRotation > 180) {
          if (verticalRotation + input < 270) {
            controller.cameraPivot.localRotation = Quaternion.Euler(270, 0, 0);
            return;
          }
        }
        else {
          if (verticalRotation + input > 90) {
            controller.cameraPivot.localRotation = Quaternion.Euler(90, 0, 0);
            return;
          }
        }

        controller.cameraPivot.Rotate(input * Settings.mouseSensitivity * TimeUtils.dialogDeltaTime, 0, 0);
      }
    }
  }

  // -------------- JUMP --------------

  [System.Serializable]
  public class JumpModule : ComponentModule {
    [Tooltip ("Initial upwards velocity of jump.")]
    public float jumpVelocity = 7;
    [Tooltip ("While holding jump, apply this much upward acceleration per second to counter the effects of gravity (-9.8m/s-2).")]
    public float jumpHoldGravityReduction = 4.9f;
    [Tooltip ("How long holding down jump applies gravity reduction.")]
    public float jumpHoldDuration = 0.5f;
    public int maxDoubleJumps = 1;

    public bool jumping = false;
    private float jumpHoldRemaining;
    // TODO this wont work if like we jump and hit a roof near immediately - it will never register as having left the ground, so it will stay "jumping" indefinitely
    private bool leftTheGround; // We might not "leave" the ground on the first few frames after jumping, but we don't want to think we've landed, so 
    private int currentDoubleJumpCount = 0;

    public override void reset() {
      jumping = false;
      leftTheGround = false;
      currentDoubleJumpCount = 0;
    }

    public override void update() {
      if (!enabled) return;

      if (controller.inAir && !leftTheGround) {
        leftTheGround = true;
      }

      if (!controller.inAir && leftTheGround) {
        jumping = false;
        leftTheGround = false;
      }

      if (!inputEnabled) return;

      if (Input.GetButtonDown("Jump")) {
        // Double jump
        if (controller.inAir) {
          if (jumping && currentDoubleJumpCount < maxDoubleJumps) { // Can't double jump unless you initiated the current jump
            controller.velocity = new Vector3(controller.velocity.x, jumpVelocity, controller.velocity.z);
            jumpHoldRemaining = jumpHoldDuration;
            currentDoubleJumpCount++;
          }
        }
        // Start Jump
        else {
          jumping = true;
          leftTheGround = false;
          jumpHoldRemaining = jumpHoldDuration;
          currentDoubleJumpCount = 0;
          controller.velocity = new Vector3(controller.velocity.x, jumpVelocity, controller.velocity.z);
        }
      }
      else {
        if (Input.GetButton("Jump") && controller.velocity.y > 0 && jumpHoldRemaining > 0) {
          controller.velocity += new Vector3(0, TimeUtils.dialogDeltaTime * 6.9f, 0);
          jumpHoldRemaining -= TimeUtils.dialogDeltaTime;
        }
        else jumpHoldRemaining = 0;
      }

      // TODO If jump in progress and going up and we hit an object, stop the jump - no more upwards acceleration
    }
  }

  // -------------- HEADBOB --------------
  
  // TODO handle footsteps audio, landing crunches. Maybe in its own module that HeadbobModule sends off messages to?
  [System.Serializable]
  public class HeadbobModule : ComponentModule {
    [System.Serializable]
    public struct HeadbobProfile {
      [Tooltip("How far up and down the head moves.")]
      public float height;
      [Tooltip("How long a complete up-down cycle takes.")]
      public float duration;
    }

    public HeadbobProfile stationary;
    public HeadbobProfile walking;
    public HeadbobProfile running;

    [Tooltip("Camera Y position that headbob ossilates around. Initializes to the Controller's cameraPivot by default.")]
    public float baseHeadHeight;
    [Tooltip("How much should the head bob after landing (multiplied by fall speed).")]
    public float landingBobStrength = 0.11f;
    [Tooltip("Landing bob animation speed.")]
    public float landingBobSpeed = 10;
    [Tooltip("Cap initial landing bob downwards velocity to this.")]
    [Range(-3, 0)]
    public float landingBobMaxStrength = -0.75f;
    
    private float recoveryYPosition;
    private HeadbobProfile currentProfile;
    private float currentFallSpeed = 0;
    private float landingBobVelocity = 0; // How quickly is the camera travelling down/up current due to landing impact?
    private float cycleTime = 0; // How many seconds into the current cycle are we?
    private float currentVelocity; // Used to smoothly move camera between bob positions

    private const float twoPi = Mathf.PI * 2; // Cached to avoid re-calculating every frame

    public override void init(FirstPersonController controller) {
      base.init(controller);
      baseHeadHeight = controller.cameraPivot.position.y;
    }

    public override void reset() {
      controller.cameraPivot.position = new Vector3(controller.cameraPivot.position.x, baseHeadHeight, controller.cameraPivot.position.z);
      cycleTime = 0;
      recoveryYPosition = baseHeadHeight;
      currentFallSpeed = 0;
    }

    public void setBaseHeadHeight(float height, bool snapToPosition = false) {
      recoveryYPosition += height - baseHeadHeight;
      baseHeadHeight = height;

      if (snapToPosition) {
        currentProfile = controller.move.moving ? (controller.move.running ? running : walking) : stationary;

        controller.cameraPivot.localPosition = new Vector3(
          controller.cameraPivot.localPosition.x,
          height + currentProfile.height * Mathf.Sin(cycleTime / currentProfile.duration * twoPi),
          controller.cameraPivot.localPosition.z
        );
      }
    }
    
    public override void update() {
      if (!enabled) return;

      if (controller.inAir) {
        if (controller.cameraPivot.localPosition.y != baseHeadHeight) {
          setCameraLocalYPosition(baseHeadHeight);
        }

        currentFallSpeed = controller.velocity.y;
      }
      else {
        // If just landed, trigger an extra large headbob based on the landing speed
        if (currentFallSpeed < 0) {
          landingBobVelocity = Mathf.Max(currentFallSpeed * landingBobStrength, landingBobMaxStrength);
          cycleTime = 0; // Restart the headbob cycle
          currentFallSpeed = 0;
          recoveryYPosition -= 0.001f; // Offset a tiny amount to indicate that a landing recovery is in progress
        } 

        // While still recovering from a landing, continue with the extra large headbob
        if (recoveryYPosition != baseHeadHeight) {
          landingBobVelocity += TimeUtils.dialogDeltaTime * landingBobSpeed; // Headbob downwards velocity gradually slows and eventually reverses, returning to normal position
          recoveryYPosition += landingBobVelocity;
          setCameraLocalYPosition(recoveryYPosition);

          // Recovery ended
          if (recoveryYPosition >= baseHeadHeight) {
            recoveryYPosition = baseHeadHeight;
          }
        }
        
        // Otherwise, do regular headbob
        else {
          currentProfile = controller.move.moving ? (controller.move.running ? running : walking) : stationary;

          cycleTime += TimeUtils.dialogDeltaTime;
          if (cycleTime > currentProfile.duration) cycleTime -= currentProfile.duration;

          setCameraLocalYPosition(baseHeadHeight + currentProfile.height * Mathf.Sin(cycleTime / currentProfile.duration * twoPi));
        }
      }
    }

    private void setCameraLocalYPosition(float y) {
      if (TimeUtils.dialogDeltaTime == 0) return; // SmoothDamp returns NaN if used with a delta time of 0
      controller.cameraPivot.localPosition = new Vector3(
        controller.cameraPivot.localPosition.x,
        Mathf.SmoothDamp(controller.cameraPivot.localPosition.y, y, ref currentVelocity, 0.1f, float.MaxValue, TimeUtils.dialogDeltaTime),
        controller.cameraPivot.localPosition.z);
    }
  }

  [System.Serializable]
  public class GravityModule : ComponentModule {
    public Vector3 gravity = new Vector3(0, -9.8f, 0);
    public override void update() {
      if (controller.inAir) controller.velocity += gravity * TimeUtils.dialogDeltaTime;
      else controller.velocity = new Vector3(controller.velocity.x, Mathf.Max(0, controller.velocity.y), controller.velocity.z);
    }
  }
}
