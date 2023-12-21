using System;
using System.Collections.Generic;
using System.Text;

namespace CaesarCipherWinFormsApplication;

public class CaesarCipherLogic
{
    private const string RuAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
    private const string EnAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static readonly HashSet<char> RuAlphabetChars = new(RuAlphabet);
    private static readonly HashSet<char> EnAlphabetChars = new(EnAlphabet);

    private readonly Dictionary<char, double> ruAlphabetByFrequency = new()
    {
        { 'А', 0.062 },
        { 'Б', 0.014 },
        { 'В', 0.038 },
        { 'Г', 0.013 },
        { 'Д', 0.025 },
        { 'Е', 0.072 },
        { 'Ж', 0.007 },
        { 'З', 0.016 },
        { 'И', 0.062 },
        { 'Й', 0.010 },
        { 'К', 0.028 },
        { 'Л', 0.035 },
        { 'М', 0.026 },
        { 'Н', 0.053 },
        { 'О', 0.090 },
        { 'П', 0.023 },
        { 'Р', 0.040 },
        { 'С', 0.045 },
        { 'Т', 0.053 },
        { 'У', 0.021 },
        { 'Ф', 0.002 },
        { 'Х', 0.009 },
        { 'Ц', 0.004 },
        { 'Ч', 0.012 },
        { 'Ш', 0.006 },
        { 'Щ', 0.003 },
        { 'Ъ', 0.007 },
        { 'Ы', 0.014 },
        { 'Ь', 0.007 },
        { 'Э', 0.003 },
        { 'Ю', 0.006 },
        { 'Я', 0.018 }
    };

    public string SplitStr(string str, int maxSymbols)
    {
        var sb = new StringBuilder();
        var counter = 0;
        foreach (var element in str)
        {
            if (counter == maxSymbols)
            {
                sb.Append(" ");
                counter = 0;
            }
            sb.Append(element);
            counter++;
        }
        return sb.ToString();
    } 

    public string EncryptMessage(int shift, string message)
    {
        var sb = new StringBuilder();
        foreach (var ch in message)
        {
            var alphabet = "";
            if (RuAlphabetChars.Contains(ch)) 
                alphabet = RuAlphabet;
            if (EnAlphabetChars.Contains(ch))
                alphabet = EnAlphabet;
            var index = ((alphabet.IndexOf(ch) + shift) % alphabet.Length + alphabet.Length) % alphabet.Length;
            sb.Append(alphabet[index]);
        }
        return sb.ToString();
    }

    public string FilterSymbols(string text)
    {
        text = text.ToUpper();
        var sb = new StringBuilder();
        foreach (var textChar in text)
        {
            if (textChar == 'Ё')
            {
                sb.Append('Е');
                continue;
            }
            if (EnAlphabetChars.Contains(textChar) || RuAlphabetChars.Contains(textChar))
                sb.Append(textChar);
        }
        return sb.ToString();   
    }

    public (string Message, int Key) DecryptedMessageWithoutKey(string message)
    {
        var resultText = "";
        var resultKey = 0;
        var minValue = double.MaxValue;

        for (var key = 0; key < RuAlphabet.Length; key++)   //в требовниях
        {
            var text = EncryptMessage(-key, message);
            var textCharsFrequency = FindCharsFrequency(text);

            var sum = 0.0;
            foreach (var (alphabetChar, idealFrequency) in ruAlphabetByFrequency)
            {
                var textCharFrequency = textCharsFrequency.ContainsKey(alphabetChar) ? textCharsFrequency[alphabetChar] : 0;
                sum += Math.Pow(idealFrequency - textCharFrequency, 2);
            }
            
            if (sum < minValue)
            {
                minValue = sum;
                resultText = text;
                resultKey = key;
            }
        }

        return (resultText, resultKey);
    }

    public Dictionary<char, double> FindCharsFrequency(string message)
    {
        var frequency = new Dictionary<char, double>(); 
        foreach (var messageChar in message)
        {
            if (frequency.ContainsKey(messageChar))
            {
                frequency[messageChar] += 1;
            }
            else
            {
                frequency.Add(messageChar, 1);
            }
        }

        foreach (var (@char, count) in frequency)
        {
            frequency[@char] = count / message.Length;
        }

        return frequency;
    }
}