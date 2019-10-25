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

    int playtimeSeconds = int.Parse(info.playtime);
    int hours = Mathf.FloorToInt(playtimeSeconds / 3600);
    int minutes = Mathf.FloorToInt((playtimeSeconds % 3600) / 60);
    int seconds = Mathf.FloorToInt(playtimeSeconds % 60);
    string playtimeText = seconds + "s";
    if (minutes > 0 || hours > 0) playtimeText = minutes + "m " + playtimeText;
    if (hours > 0) playtimeText = hours + "h " + playtimeText;

    playtime.text = playtimeText;
    lastPlayed.text = info.lastPlayed.ToShortDateString() + "  " + info.lastPlayed.ToShortTimeString();
  }

  public void onClick() {
    menu.selectItem(this);
  }
}
