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
        public string word;
        public string token;

        public uint getLineNumber()
        {
            return lineNumber;
        }
        public string GetWord()
        {
            return word;
        }
        public RawWords(string Word, int LineNo)
        {
            this.word = Word;
            this.lineNumber = (uint)LineNo;
        }
        public RawWords(int LineNo, string Token)
        {
            this.lineNumber = (uint)LineNo;
            this.token = Token;
        }
    }
    class LexicalAnalyzer
    {
        public static string Source;

        public static void SetSource(string Input)
        {
            Source = Input;
        }
        #region regex for different scenarios
        private static string floatRegex()
        {
                    // + - or (digits)
            return @"(^[\+-]?[\d]*\.[\d]+([e]([\+-]?[\d]+))?$|^[\+-]?[\d]+\.[\d]*([e]([\+-]?[\d]+))?$)";
        }
        private static string intRegex()
        {
            return @"^-?\+?[\d]+$";
        }

        #endregion
        private static void parse(RawWords Word)
        {
            string input = Word.GetWord();
            //regular expressions...
            if(Regex.IsMatch(input, @"^[a-zA-Z_][\w_]*$"))
            {
                #region classifying keywords & identifiers
                switch (input)
                {
                    case "and":
                        Word.token = "Logical_And_Or";
                        break;
                    case "assert":
                        Word.token = "assert";
                        break;
                    case "break":
                        Word.token = "break";
                        break;
                    case "case":
                        Word.token = "case";
                        break;
                    case "class":
                        Word.token = "class";
                        break;
                    case "continue":
                        Word.token = "continue";
                        break;
                    case "def":
                        Word.token = "Def";
                        break;
                    case "default":
                        Word.token = "default";
                        break;
                    case "del":
                        Word.token = "del";
                        break;
                    case "elif":
                        Word.token = "elif";
                        break;
                    case "else":
                        Word.token = "else";
                        break;
                    case "except":
                        Word.token = "except";
                        break;
                    case "exec":
                        Word.token = "Exec";
                        break;
                    case "finally":
                        Word.token = "finally";
                        break;
                    case "for":
                        Word.token = "for";
                        break;
                    case "from":
                        Word.token = "from";
                        break;
                    case "global":
                        Word.token = "global";
                        break;
                    case "if":
                        Word.token = "if";
                        break;
                    case "import":
                        Word.token = "import";
                        break;
                    case "in":
                        Word.token = "in";
                        break;
                    case "is":
                        Word.token = "is";
                        break;
                    case "lambda":
                        Word.token = "lambda";
                        break;
                    case "not":
                        Word.token = "not";
                        break;
                    case "or":
                        Word.token = "or";
                        break;
                    case "pass":
                        Word.token = "pass";
                        break;
                    case "print":
                        Word.token = "print";
                        break;
                    case "raise":
                        Word.token = "raise";
                        break;
                    case "repeat":
                        Word.token = "repeat";
                        break;
                    case "return":
                        Word.token = "return";
                        break;
                    case "try":
                        Word.token = "try";
                        break;
                    case "until":
                        Word.token = "until";
                        break;
                    case "while":
                        Word.token = "while";
                        break;
                    case "with":
                        Word.token = "with";
                        break;
                    case "yield":
                        Word.token = "yield";
                        break;
                    case "__BODY_END__":
                        Word.token = "__BODY_END__";
                        break;
                    case "__BODY_START__":
                        Word.token = "__BODY_START__";
                        break;
                    default:
                        Word.token = "Identifier";
                        break;
                }
                #endregion
            }
            // regex for string literal
            else if (Regex.IsMatch(input, "^\"" + @"[\w\s\W]*" + "\"$") || Regex.IsMatch(input, "^\"\"\"" + @"[\w\s\W]*" + "\"\"\"$"))
            {
                Word.token = "StringLiteral";
            }
            // regex for integers
            else if(Regex.IsMatch(input,intRegex()))
            {
                if (Regex.IsMatch(input, @"^[\d]+$"))
                {
                    Word.word = "+" + Word.word;
                }
                Word.token = "Integer";
            }
            // regex for float
            else if (Regex.IsMatch(input,floatRegex(),RegexOptions.IgnoreCase) )
            {
                if (Regex.IsMatch(input, @"^[\.]"))
                {
                    Word.word = Regex.Replace(input, @"^[\.]", "0.");
                }
                else if (Regex.IsMatch(input, @"[\.]$"))
                {
                    Word.word = Regex.Replace(input, @"[\.]$", ".0");
                }
                Word.token = "Float";
            }
            else if (input == "(" || input == ")" || input == "%" || input == ",")
            {
                Word.token = input;
            }
            else
            {
                Word.token = "Invalid";
            }
        }

        
        public static string Compile(string Input)
        {
            Source = Input;
            var Words = WordBreak(Input);
            StringBuilder Token = new StringBuilder();
            foreach(var word in Words)
            {
                parse(word);
                Token.AppendFormat("({0}, {1}, {2})\n", word.token, word.GetWord(), word.getLineNumber());
            }
            return Token.ToString();

        }

        private static List<RawWords> WordBreak(string WordsToBreak)
        {
            ushort LastIndent = 0;
            List<RawWords> Words = new List<RawWords>();
            StringBuilder temp = new StringBuilder();
            int lineNumber = 1;
            bool isMultilineEnd = false;
            bool isStringEnd = false;
            char ch;
            var i = 0;
            ch = WordsToBreak[i];
            while(ch == '\t')
            {
                LastIndent++;
            }
            for (i = 0; i < WordsToBreak.Length;i++)
            {
                ch = WordsToBreak[i];
                // space handled here
                if (ch == ' ')
                {
                    if (isStringEnd || isMultilineEnd)
                    {
                        temp.Append(ch);
                    } 
                    else if (temp.ToString() != "")
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber) );
                        temp.Clear();
                    }
                }

                else if (ch== '.')
                {
                    // if temp already contains a . (dot), then push it in words and append a new . (Dot)
                    if (Regex.IsMatch(temp.ToString(), @"[\.]"))
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                    }
                    temp.Append(ch);
                }
                else if (ch == '+' || ch == '-')
                {
                    // if it already has something in temp. push it in, because a new something is coming in..
                    if (temp.ToString() != "" && !Regex.IsMatch(temp.ToString(), @"[\.]\d*[eE]$"))
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                    }

                    temp.Append(ch);
                }
                // numbers are being handled here  Have to handle point too
                else if ( ch >= '0' && ch < '9')
                {
                    temp.Append(ch);
                }
                
                // alphabets and Underscore handled here 
                else if( (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch == '_'))
                {
                    temp.Append(ch);
                }

                // ( ) % , handeled as they function similarly as for off now, i may have to seperate comma later.
                else if (ch == '(' || ch == ')' || ch == '%' || ch == ',')
                {
                    if (isStringEnd || isMultilineEnd)
                    {
                        temp.Append(ch);
                    }
                    else
                    {
                        if (temp.ToString() != "")
                        {
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                        }
                        Words.Add(new RawWords(ch.ToString(), lineNumber));
                    }
                }
                // double quotes handled here
                else if (ch == '"')
                {
                    if ((WordsToBreak.Length >= i + 3) && WordsToBreak.Substring(i, 3) == "\"\"\"")
                    {
                        isMultilineEnd = !isMultilineEnd;
                        temp.Append(ch + "\"\"");
                        i += 2;
                    }
                    else
                    {
                        temp.Append(ch);
                        
                        if (isStringEnd && temp[temp.Length-1] != '\\')
                        {
                            
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();

                        }
                        isStringEnd = !isStringEnd;

                    }
                }
                // handling remaining symbols & special character
                // please refer to an ASCII table to understand what i did here
                else if ((ch > ' ' && ch < 'A') || (ch >= '{' && ch <= '~') || (ch >= '[' && ch <= '`'))
                {
                    if (isStringEnd || isMultilineEnd)
                    {
                        temp.Append(ch);
                    }
                    else
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                        temp.Append(ch);
                    }
                }
                // tabs handled here
                else if (ch == '\t')
                {
                    var NextIndent = 0;
                    while(ch == '\t')
                    {
                        NextIndent++;
                        i++;
                        ch = WordsToBreak[i];
                    }
                    if (NextIndent < LastIndent)
                    {
                        Words.Add(new RawWords("__BODY_END__", lineNumber));
                        temp.Clear();
                    }
                    else if (NextIndent > LastIndent)
                    {
                        Words.Add(new RawWords("__BODY_START__", lineNumber));
                        temp.Clear();
                    }
                }
                // newline case handled here
                else if (ch == '\n' || ch == '\r')
                {
                    // if it is in a multiline string of three double quotes
                    if (isMultilineEnd)
                    {
                        temp.Append(ch);
                    } 
                    else
                    {
                        if(temp.ToString() != "")
                        {
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                        }
                        
                    }
                    if (ch == '\n')
                    {
                        lineNumber++;
                    }
                }
                // no case occured
                else
                {
                    Words.Add(new RawWords(temp.ToString(), lineNumber));
                    temp.Clear();
                }
            }
            if (temp.ToString() != "")
            {
                Words.Add(new RawWords(temp.ToString(), lineNumber));
                temp.Clear();
            }
            return Words;
        }
    }
}
