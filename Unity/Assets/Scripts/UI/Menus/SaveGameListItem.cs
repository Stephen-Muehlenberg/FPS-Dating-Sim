using UnityEngine;
using UnityEngine.UI;

public class SaveGameListItem : MonoBehaviour {
  public Text title;
  public Text playtime;
  public Text lastPlayed;
  public SaveFileInfo info;

  private SaveLoadMenu menu;

  public void setInfo(SaveLoadMenu menu, SaveFileInfo info) {
    this.menu = menu;
    this.info = info;
    title.text = info.name;
    playtime.text = info.playtime;
    lastPlayed.text = info.lastPlayed;
  }

  public void onClick() {
    menu.selectItem(this);
  }
}
