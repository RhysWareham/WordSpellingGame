using System;
using System.Collections.Generic;

[Serializable]
public class Word
{
    public string text;
}

[Serializable]
public class WordList
{
    public List<Word> words;
}

[Serializable]
public class Root
{
    public WordList list;
}