using System.Collections;
using UnityEngine;

public class Quest_Competition : Quest {
  public static readonly string NAME = "Competition";
  public override string name => NAME;
  private static readonly string SCENE = "mission_competition";

  private const string OBJECTIVE_CLEAR_PLAZA = "Kill all the monsters in the plaza";
  private const string OBJECTIVE_TIME = " seconds left";

  private float kills = 0;
  private int secondsRemaining = 60;
  private string currentWeaponText;
  private bool monsterSpawnStarted = false;

  protected override void initialise(int state, Hashtable args) {
    setUpScene(
      state: state,
      scene: state == 0 ? "cafe" : SCENE,
      mcPosition: state == 0 ? new Vector3(0, 0, -3) // Cafe
                : state < 300 ? new Vector3(34, 0, 34) // Mission start
                             : new Vector3(0, 0.5f, 0), // Mid-mission
      mcRotation: state == 0 ? Quaternion.identity
                             : Quaternion.Euler(0, -135, 0),
      rosePosition: state == 0 ? new Vector3(-1, 0, -2)
                               : null as Vector3?,
      mayPosition: state == 0 ? new Vector3(-0.35f, 0, -1.7f)
                              : null as Vector3?,
      vanessaPosition: state == 0 ? new Vector3(0.35f, 0, -1.7f)
                                  : null as Vector3?,
      fizzyPosition: state == 0 ? new Vector3(1, 0, -2)
                                : null as Vector3?,
      lookEnabled: true,
      moveEnabled: state > 0,
      jumpEnabled: state > 0,
      weaponEquipped: state == 0 ? null : Weapons.SHOTGUN,
      animateQuestMessageIn: true
    );
  }

  protected override void handleState(int state) {
    if (state == 0) s000_openingConversation();
    else if (state == 100) s100_initialEnemies();
    else if (state == 200) s200_waveShotgun();
    else if (state == 300) s300_waveGrenade();
    else if (state == 400) s400_waveSniper();
    else if (state == 500) s500_waveMgun();
    else if (state == 600) s600_end();
    else throw new UnityException("Unknown state " + state);
  }

  public override void stop() {
    if (state >= 100 && state < 600) Monsters.OnMonstersChanged += onMonstersChangedCallback;
  }

  private void s000_openingConversation() {
    new Conversation()
      .text(Character.MAY, "Ok, let's get back to saving the world!")
      .text(Character.VANESSA, "Thank you for your help, MC. Maybe we'll visit again later? In the meantime, stay safe.")
      .text(Character.FIZZY, "Wait, he should totes come with us. Much safer with magic-gun-ladies than alone.")
      .text(Character.ROSE, "Yeah I was gonna say. He's a surprisingly good shot. Like, better than the rest of you. We should keep him.")
      .text(Character.VANESSA, "Fair points, though it's up to MC.")
      .text(Character.MC, "Ok, let's get back to saving the world!")
      .text(Character.MC_NARRATION, "The girls gave a cheer, and we all left the cafe.")
      .text("It soon became clear we'd killed every monster in the vicinity. We ended up wondering over to the nearby shopping center.")
      .show(() => { SceneTransition.changeTo(scene: "mission_competition", onSceneLoaded: () => {
        Weapons.equip(Weapons.SHOTGUN);
        setState(100);
      }); });
  }

  private void s100_initialEnemies() {
    CurrentQuestMessage.set(OBJECTIVE_CLEAR_PLAZA);
    Monsters.OnMonstersChanged += onMonstersChangedCallback;
  }

  private void s200_waveShotgun() {
    setupNextWave(
      weapon: Weapons.SHOTGUN,
      nextState: 300,
      spawnMonstersImmediately: false,
      dialog: new CombatDialog()
        .message(Character.FIZZY, "Competition?")
        .message(Character.ROSE, "Competition!")
      );
  }

  private void s300_waveGrenade() {
    setupNextWave(
      weapon: Weapons.GRENADE_LAUNCHER,
      nextState: 400,
      spawnMonstersImmediately: true,
      dialog: new CombatDialog()
        .message(Character.FIZZY, "Times up, Rose!")
        .message(Character.FIZZY, "My turn.")
      );
  }

  private void s400_waveSniper() {
    setupNextWave(
      weapon: Weapons.SNIPER_RIFLE,
      nextState: 500,
      spawnMonstersImmediately: true,
      dialog: new CombatDialog()
        .message(Character.FIZZY, "Your turn, Nessie!")
        .message(Character.VANESSA, "Shall we, MC?")
      );
  }

  private void s500_waveMgun() {
    setupNextWave(
      weapon: Weapons.MACHINE_GUN,
      nextState: 600,
      spawnMonstersImmediately: true,
      dialog: new CombatDialog()
        .message(Character.VANESSA, "May? Your turn.")
        .message(Character.MAY, "Alright, here I come.")
      );
  }

  private void s600_end() {
    new CombatDialog()
      .message(Character.MC_NARRATION, "Some sort of conclusion will go here.")
      .message(Character.MC_NARRATION, "But for now, level's over.")
      .message(Character.MC_NARRATION, "Returning to cafe...")
      .show(CombatDialog.Priority.MAX, () => {
        QuestManager.start(Quest_BedStore.NAME);
      });
  }

  private void setupNextWave(Weapon weapon,
                             int nextState,
                             bool spawnMonstersImmediately,
                             CombatDialog dialog) {
    if (spawnMonstersImmediately && !monsterSpawnStarted) {
      Monsters.OnMonstersChanged += onMonstersChangedCallback;
      Player.SINGLETON.StartCoroutine(spawnMonstersContinuously());
    }

    dialog.show(CombatDialog.Priority.MAX, () => {
      kills = 0;
      secondsRemaining = 30;
      currentWeaponText = weapon.name + " kills: ";
      updateObjectiveText();
      Weapons.equip(weapon);
      Player.SINGLETON.StartCoroutine(countdown(stateOnComplete: nextState));

      if (!monsterSpawnStarted && !spawnMonstersImmediately) {
        Monsters.OnMonstersChanged += onMonstersChangedCallback;
        Player.SINGLETON.StartCoroutine(spawnMonstersContinuously());
      }
    });
  }

  private void updateObjectiveText() {
    CurrentQuestMessage.update(currentWeaponText + kills + "\n" + secondsRemaining + OBJECTIVE_TIME);
  }

  private void onMonstersChangedCallback(Monster monster, bool added, int monstersRemaining) {
    if (state == 100) {
      if (monstersRemaining == 0) {
        Monsters.OnMonstersChanged -= onMonstersChangedCallback;
        setState(200);
      }
    }
    else if (state >= 200) {
      if (!added) {
        kills++;
        updateObjectiveText();
      }
    }
  }

  private IEnumerator countdown(int stateOnComplete) {
    while (secondsRemaining > 0) {
      yield return new WaitForSeconds(1);
      secondsRemaining--;
      updateObjectiveText();
    }
    Monsters.OnMonstersChanged -= onMonstersChangedCallback;
    setState(stateOnComplete);
  }

  private IEnumerator spawnMonstersContinuously() {
    monsterSpawnStarted = true;
    Vector3 spawnLocation;
    while (true) {
      spawnLocation = new Vector3(Random.Range(-40, 40), 0, Random.Range(-40, 40));

      // 1 Hellfirer = 2 squigs = 5 infernals = 14 torches
      int nextMonster = Random.Range(0, 21);

      if (nextMonster == 0 && Monsters.hellfirerCount() == 0) {
        Monsters.spawnHellfirer(spawnLocation);
      }
      else if (nextMonster >= 1 && nextMonster <= 2 && Monsters.squigCount() < 3) {
        Monsters.spawnSquig(spawnLocation);
      }
      else if (nextMonster >= 3 && nextMonster <= 7) Monsters.spawnInfernal(spawnLocation);
      else Monsters.spawnTorch(spawnLocation);

      float timeUntilNextSpawn = 0.35f + (Monsters.monsters.Count * 0.20f);
      yield return new WaitForSeconds(timeUntilNextSpawn);
    }
  }
}
