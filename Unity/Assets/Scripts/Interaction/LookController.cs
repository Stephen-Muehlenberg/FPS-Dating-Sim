using UnityEngine;

/**
 * A component that lives on the Player, raycasting forward to see if we're looking at anything
 * with a LookTarget component. If we are, that component displays some text and can be intereacted
 * with.
 */
public class LookController : MonoBehaviour {
  private const float MAX_LOOK_RANGE = 100f;
  private static LookController SINGLETON;

  private bool active = true;
  private bool rayHitSomething = false;
  private int layerMask;
  private RaycastHit hitInfo;
  private LookTarget currentTarget;
  private LookTarget previousTarget;

  void Start() {
    SINGLETON = this;
    layerMask = ~LayerMask.GetMask("Weapons", "Invisible Wall", "Player", "UI", "Ignore Raycast"); // Ignore these layers
  }

  public static void enable() {
    if (SINGLETON != null) SINGLETON.active = true; // LookController doesn't exist in this context
  }

  public static void disable() {
    if (SINGLETON == null) return; // LookController hasn't been created yet
    SINGLETON.stopLookingAtPreviousTarget();
    SINGLETON.active = false;
  }

  void Update() {
    if (!active) return;
    if (TimeUtils.gameplayPaused) return;

    // TODO ignore UI layer?
    rayHitSomething = Physics.Raycast(transform.position, transform.forward, out hitInfo, MAX_LOOK_RANGE, layerMask);

    if (!rayHitSomething) {
      stopLookingAtPreviousTarget();
      return;
    }

    currentTarget = hitInfo.collider.GetComponent<LookTarget>();
    if (currentTarget == null || !currentTarget.enabled) {
      stopLookingAtPreviousTarget();
      return;
    }

    if (hitInfo.distance > currentTarget.interactionRange) {
      stopLookingAtPreviousTarget();
      return;
    }

    if (previousTarget != null) {
      if (currentTarget == previousTarget) {
        // Can only interact with a target if you didnt start looking at it this frame
        if (Input.GetButtonUp("PrimaryFire")) currentTarget.interact();
        return;
      }
      stopLookingAtPreviousTarget();
    }

    currentTarget.startLooking();
    previousTarget = currentTarget;
  }

  private void stopLookingAtPreviousTarget() {
    if (previousTarget != null) {
      previousTarget.stopLooking();
      previousTarget = null;
    }
  }
}
