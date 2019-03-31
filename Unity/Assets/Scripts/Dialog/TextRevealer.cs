using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Writes text to a UI Text element one character at a time, based on the
 * specified speed. Automatically handles markup.
 */
public class TextRevealer {
  private Text uiText;
  private string message;
  private float speedMultiplier;
  private Callback revealFinishedCallback;

  private float timeTillNextCharacter;
  private int nextCharacterIndex;

  private List<string> currentMarkup = new List<string>();
  private int currentMarkupLength;
  private string currentMarkupString;

  private char nextChar; // Cached to avoid re-allocation.
  private string markup; // Cached to avoid re-allocation.

  public void set(Text uiText, string message, float speedMultiplier, Callback revealFinishedCallback) {
    this.uiText = uiText;
    uiText.text = "";
    this.message = message;
    this.speedMultiplier = speedMultiplier;
    this.revealFinishedCallback = revealFinishedCallback;
    timeTillNextCharacter = 0f;
    nextCharacterIndex = 0;
    currentMarkup.Clear();
    currentMarkupLength = 0;
    currentMarkupString = "";
  }

  public void update() {
    timeTillNextCharacter -= Time.unscaledDeltaTime;

    if (timeTillNextCharacter <= 0) {
      do
      {
        nextChar = message[nextCharacterIndex];

        // Remove temporary markup.
        if (currentMarkup.Count > 0) {
          uiText.text = uiText.text.Substring(0, uiText.text.Length - currentMarkupLength);
        }

        // Handle markup.
        while (nextChar == '<') {
          markup = message.Substring(nextCharacterIndex).Split('>')[0] + ">"; // Gets from here till '>'.

          if (markup[1] == '/') currentMarkup.Remove(markup); // Stop adding temporary closures.
          else currentMarkup.Insert(0, "</" + markup.Substring(1)); // Start adding temporary closures.
          updateMarkupValues();

          uiText.text += markup;
          nextCharacterIndex += markup.Length;
          nextChar = message[nextCharacterIndex];
        }

        // TODO prevent a word from half appearing on one line, then it gets too long for that line and
        // it then jumps down to the next line.
        // Simplest hack might be to display the entire word at once, but set the remaining letters to
        // transparent. Complicated but easier than messing around with calculating widths and such.
        // See https://docs.unity3d.com/Manual/StyledText.html
        // Note: upon further thinking, the entire message could be displayed at once and forevery, and
        // only the position of the transparent characters needs to change.

        // Append the next character.
        uiText.text += nextChar;
        nextCharacterIndex++;

        // Add temporary markup (so the partially constructed message still marks up correctly).
        if (currentMarkup.Count > 0) uiText.text += currentMarkupString;

        // Wait a small amount of time before revealing the next character (ignoring whitespace).
        // TODO this should not be accessing settings all the time (probably) - should be passed a speed value in (maybe?)
        if (!nextChar.isWhitespace()) timeTillNextCharacter += Settings.textSpeed * speedMultiplier;

        // When no characters left to reveal, stop reveal process.
        if (uiText.text.Length == message.Length) {
          // Need to ensure the actual member variable is set to null before the callback is invoked.
          var callback = revealFinishedCallback;
          revealFinishedCallback = null;
          callback?.Invoke();
        }

        // Possible for multiple letters to get revealed in a single update(); keep looping until all done.
      } while (timeTillNextCharacter <= 0);
    }
  }

  private void updateMarkupValues() {
    currentMarkupLength = 0;
    currentMarkupString = "";
    foreach (string markupElement in currentMarkup) {
      currentMarkupLength += markupElement.Length;
      currentMarkupString += markupElement;
    }
  }
}
