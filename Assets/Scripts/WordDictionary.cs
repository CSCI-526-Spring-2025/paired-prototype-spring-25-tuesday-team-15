using System.Collections.Generic;
using UnityEngine;

public class WordDictionary : MonoBehaviour
{
    private HashSet<string> wordSet;
    private Dictionary<char, float> letterProbabilities;
    private char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    void Start()
    {
        // Initialize word dictionary
        wordSet = new HashSet<string>()
        {
            "apple", "banana", "cherry", "date", "elder",
            "fruit", "grape", "honey", "ice", "joker",
            "kite", "lemon", "mango", "nut", "orange",
            "peach", "queen", "raspberry", "sugar", "tomato"
        };

        // Generate letter frequencies based on dictionary
        GenerateLetterFrequencies();
    }

    void GenerateLetterFrequencies()
    {
        Dictionary<char, int> letterCounts = new Dictionary<char, int>();
        int totalLetters = 0;

        foreach (string word in wordSet)
        {
            foreach (char c in word)
            {
                if (!letterCounts.ContainsKey(c))
                    letterCounts[c] = 0;
                
                letterCounts[c]++;
                totalLetters++;
            }
        }

        // Convert counts to probabilities
        letterProbabilities = new Dictionary<char, float>();
        foreach (var entry in letterCounts)
        {
            letterProbabilities[entry.Key] = (float)entry.Value / totalLetters;
        }
    }

    public char GetRandomLetter()
    {
        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        foreach (var entry in letterProbabilities)
        {
            cumulativeProbability += entry.Value;
            if (randomValue <= cumulativeProbability)
                return entry.Key;
        }

        return 'a'; // Default fallback
    }

    public bool IsValidWord(string word)
    {
        return wordSet.Contains(word.ToLower());
    }
}
