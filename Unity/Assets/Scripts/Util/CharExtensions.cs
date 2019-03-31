public static class CharExtensions {
  public static bool isWhitespace(this char character) {
    return character == ' ' || character == '\n';
  }
}
