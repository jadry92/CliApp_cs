﻿
using System.Text.Json;
using MyDataStructures;

namespace CliApp
{
    class Program
    {

        static int Main(string[] args)
        {
            CLI app = new CLI();
            app.Run();

            return 0;
        }
    }

    class CLI
    {
        private const uint MAX_PRESDICTION_SHOW = 20;
        private const string FILE_NAME = "data/dictionary_compact.json";
        public EnglishDictionary? myDic;

        public CLI()
        {
            this.myDic = new EnglishDictionary(FILE_NAME);
        }

        public void Run()
        {
            if (myDic == null)
            {
                throw new NullReferenceException("Dictionary can not be null");
            }

            bool flag_continue = true;

            string word = "";

            Console.Clear();
            Console.WriteLine("Please provide a word:");

            string[] prediction = [];
            do
            {
                if (!Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.Spacebar || keyInfo.Key == ConsoleKey.Escape)
                    {
                        flag_continue = false;
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (word.Length > 0)
                        {
                            word = word.Substring(0, word.Length - 1);
                        }
                    }
                    else
                    {
                        word += keyInfo.Key.ToString().ToLower();
                    }
                    // clear the console
                    this.clearPredictions(prediction);
                }
                Console.WriteLine(word);

                prediction = myDic.getPrediction(word);
                this.printPredictions(prediction);

            } while (flag_continue);

            this.clearPredictions(prediction);
            string definition = myDic.getDefinition(word);
            Console.WriteLine($"Definition of {word}:");
            Console.WriteLine(definition);
        }

        private void printPredictions(string[] prediction)
        {
            if (prediction.Length > 0)
            {
                if (prediction.Length > MAX_PRESDICTION_SHOW)
                {
                    for (int i = 0; i < MAX_PRESDICTION_SHOW; i++)
                    {
                        Console.WriteLine("-> " + prediction[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < prediction.Length; i++)
                    {
                        Console.WriteLine("-> " + prediction[i]);
                    }
                }
            }
        }

        private void clearPredictions(string[] predictions)
        {
            if (predictions.Length > 0)
            {
                if (predictions.Length > MAX_PRESDICTION_SHOW)
                {
                    for (int i = 0; i < MAX_PRESDICTION_SHOW + 1; i++)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                }
                else
                {
                    for (int i = 0; i < predictions.Length + 1; i++)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                }
            }
        }
    }


    class EnglishDictionary
    {
        public Dictionary<string, string>? dic;
        private Trie trie;

        public EnglishDictionary(string fileName)
        {
            string filePath = Path.GetFullPath(fileName);
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                this.dic = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
            }
            else
            {
                Console.WriteLine($"File {filePath} does not exist");
            }
            this.trie = new Trie();
            this.addWordsToTrie();
        }

        public string[] getPrediction(string word)
        {
            string prediction;
            TrieNode node;
            if (word != null)
            {
                (prediction, node) = this.trie.predict(word);
                if (prediction != string.Empty)
                {
                    List<string> words = new List<string>();
                    this.trie.reconstructWords(node, prediction, words);
                    return words.ToArray();
                }
            }
            return [""];
        }

        public string getDefinition(string word)
        {
            if (this.dic == null)
            {
                throw new NullReferenceException(" Dictionary can not be null");
            }
            if (this.dic.ContainsKey(word))
            {
                return this.dic[word];
            }
            else
            {
                return $"|--- {word} not in the Dictionary ---|";

            }
        }

        private void addWordsToTrie()
        {
            if (this.dic == null)
            {
                throw new NullReferenceException(" Dictionary can not be null");
            }
            List<string> words = new List<string>(this.dic.Keys);
            if (words.Count > 0)
            {
                foreach (string word in words)
                {
                    this.trie.add(word);
                }
            }
        }

    }
}
