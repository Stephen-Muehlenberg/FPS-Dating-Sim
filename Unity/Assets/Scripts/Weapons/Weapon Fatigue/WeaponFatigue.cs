abstract public class WeaponFatigue {
  public abstract bool canFire { get; }
  public abstract void firePrimary();
  public abstract void fireSecondary();
  public abstract float primaryCooldown { get; }
  public abstract float secondaryCooldown { get; }
  public abstract void update();
  public abstract float getAsFraction();
}
