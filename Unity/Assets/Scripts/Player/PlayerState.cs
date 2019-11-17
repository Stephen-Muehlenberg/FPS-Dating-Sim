using System;

[Serializable]
public class PlayerState {
  public readonly float posX, posY, posZ;
  public readonly float rotX, rotY;
  public readonly bool look, move, jump; // True if these inputs are enabled

  public PlayerState(float posX, float posY, float posZ, float rotX, float rotY, bool look, bool move, bool jump) {
    this.posX = posX;
    this.posY = posY;
    this.posZ = posZ;
    this.rotX = rotX;
    this.rotY = rotY;
    this.look = look;
    this.move = move;
    this.jump = jump;
  }
}