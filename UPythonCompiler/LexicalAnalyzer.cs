using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace UPythonCompiler
{
    class RawWords
    {
        uint lineNumber;
        string word;
        public string token;

        public string GetWord()
        {
            return word;
        }
        public RawWords(string Word, int LineNo)
        {
            this.word = Word;
            this.lineNumber = (uint)LineNo;
        }
    }
    class LexicalAnalyzer
    {
        public static string Source;

        public static void SetSource(string Input)
        {
            Source = Input;
        }

        private void parse(RawWords Word)
        {
            string input = Word.GetWord();
            //regular expressions...
            if(Regex.IsMatch(input, @"[a-zA-Z_][\w_]*"))
            {
                switch (input)
                {
                    case "and":
                        Word.token = "";
                        break;
                    case "assert":
                        Word.token = "";
                        break;
                    case "break":
                        Word.token = "";
                        break;
                    case "case":
                        Word.token = "";
                        break;
                    case "class":
                        Word.token = "";
                        break;
                    case "continue":
                        Word.token = "";
                        break;
                    case "def":
                        Word.token = "";
                        break;
                    case "del":
                        Word.token = "";
                        break;
                    case "elif":
                        Word.token = "";
                        break;
                    case "else":
                        Word.token = "";
                        break;
                    case "except":
                        Word.token = "";
                        break;
                    case "exec":
                        Word.token = "";
                        break;
                    case "finally":
                        Word.token = "";
                        break;
                    case "for":
                        Word.token = "";
                        break;
                    case "from":
                        Word.token = "";
                        break;
                    case "global":
                        Word.token = "";
                        break;
                    case "if":
                        Word.token = "";
                        break;
                    case "import":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;
                    case "and":
                        Word.token = "";
                        break;

                }
            }

        }

        public static void Compile(string Input)
        {
            Source = Input;


        }

        private Object WordBreak(string WordsToBreak)
        {
            
            List<RawWords> Words = new List<RawWords>();
            StringBuilder temp = new StringBuilder();
            int lineNumber = 1;
            bool isMultilineEnd = false;
            bool isStringEnd = false;
            char ch; 
            for (var i = 0; i < WordsToBreak.Length;i++)
            {

                ch = WordsToBreak[i];
                if (ch == ' ')
                {
                    if (temp.ToString() != "")
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber) );
                    }
                } else if( (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch == '_'))
                {
                    temp.Append(ch);
                }
                if (ch == '"')
                {
                    if (WordsToBreak.Length - 1 > i + 3) { 
                        if (WordsToBreak.Substring(i, 3) == "\"\"\"")
                        {
                            isMultilineEnd = !isMultilineEnd;
                            temp.Append(ch + "\"\"");
                            i += 3;
                        }
                        
                    }
                    else
                    {
                        if (isStringEnd)
                        {
                            temp.Append(ch);
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            
                        }
                        isStringEnd = !isStringEnd;

                    }
                }
            }

            return Words;
        }
    }
}
