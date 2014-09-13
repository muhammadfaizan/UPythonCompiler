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
            this.word = "";
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
            try { 
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
                #region literal integers, floats, strings
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
                #endregion
            
                #region brackets handling
                // for brackets and modulus
                else if (input == "("){
                    Word.token = "RBracketOpen";
                }
                else if (input == ")")
                {
                    Word.token = "RBracketClose";
                }
                else if (input == "{")
                {
                    Word.token = "CBracketOpen";
                }
                else if (input == "}")
                {
                    Word.token = "CBracketClose";
                }
                else if (input == "[")
                {
                    Word.token = "SBracketOpen";
                }
                else if (input == "]")
                {
                    Word.token = "SBracketClose";
                }
                #endregion
                #region operators handling
                // dealing exponentials (**)
                else if (input == "**")
                {
                    Word.token = "Exponential";
                }
                // for floor division (//)
                else if (input == "//")
                {
                    Word.token = "FloorDivision";
                }
                // handling plus (+)
                else if (input == "+")
                {
                    Word.token = "Plus";
                }
                // handling plus (-)
                else if (input == "-")
                {
                    Word.token = "Minus";
                }
                else if( input == "%" )
                {
                    Word.token = "Mod";
                }
                #endregion
                #region bitwise operations
                else if (input == "<<" || input == ">>")
                {
                    Word.token = "BitwiseShift";
                }
                #endregion
                #region comparision operators
                else if(input == "<=" || input == ">=" 
                    || input == "<>" || input == "!=" || input == "=="
                    || input == "<" || input == ">")
                {
                    Word.token = "ComparisionOperator";
                }
                #endregion
                #region assignment operators
                else if (input == "=" || input == "+=" || input == "-=" 
                    || input == "/=" || input == "//=" || input == "%=" 
                    || input == "*=" || input == "**=")
                {
                    Word.token = "AssignmentOperator";
                }
                #endregion
                else if (input == ":")
                {
                    Word.token = "Colon";
                }
                else if (input == ",")
                {
                    Word.token = "Comma";
                }
                else
                {
                    if (Word.token == null)
                    {
                        Word.token = "Invalid";
                    }
                }
            }
            catch (Exception e)
            {

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
                #region space case here
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
                #endregion
                #region Colon case here
                else if (ch == ':')
                {
                    if (temp.ToString() != String.Empty && !isMultilineEnd && !isStringEnd)
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                        Words.Add(new RawWords(ch.ToString(), lineNumber));
                    }
                    else
                    {
                        temp.Append(ch);
                    }
                }
                #endregion
                #region Dot case here
                else if (ch== '.')
                {
                    // if temp already contains a . (dot), then push it in words and append a new . (Dot)
                    if (Regex.IsMatch(temp.ToString(), @"[\.]") && !isStringEnd && !isMultilineEnd)
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                    }
                    temp.Append(ch);
                }
                #endregion
                #region Plus Minus case here
                else if (ch == '+' || ch == '-')
                {
                    // if it is a part of any string or a building float
                    if (isStringEnd || isMultilineEnd || Regex.IsMatch(temp.ToString(), @"[\.]\d*[eE]$"))
                    {
                        // then simply append temp
                        temp.Append(ch);
                    }
                    // if not yet the temp has something,
                    else if (temp.ToString() != "")
                    {
                        //then push it
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();                        
                    }
                    if (WordsToBreak.Length > i + 1 && WordsToBreak[i + 1] == '=')
                    {
                        temp.Append(ch.ToString() + "=");
                        i++;
                    }
                    else
                    {
                        temp.Append(ch);
                    }
                    Words.Add(new RawWords(temp.ToString(), lineNumber));
                    temp.Clear();
                    
                }
                #endregion
                #region Multiply Divide
                else if (ch == '*' || ch == '/')
                {
                    if (isMultilineEnd || isStringEnd)
                    {
                        temp.Append(ch);
                    } else if (temp.ToString() == "*" || temp.ToString() == "/")
                    {
                        temp.Append(ch);
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();

                    } 
                    else if (temp.ToString() != "")
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                        temp.Append(ch);
                    } 
                    else {
                        temp.Append(ch);
                    }

                    if ( (WordsToBreak.Length > (i+1) && (WordsToBreak[i+1] == '=' || WordsToBreak[i+1] == ch)) )
                    {
                        i++;
                        temp.Append(WordsToBreak[i]);
                        if (WordsToBreak.Length > (i + 1) && WordsToBreak[i + 1] == '=' && Regex.IsMatch(temp.ToString(),@"[^=]$"))
                        {
                            i++;
                            temp.Append(WordsToBreak[i]);
                        }
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                        
                    }
                }
                #endregion
                #region numbers are being handled here
                // numbers are being handled here  Have to handle point too
                else if ( ch >= '0' && ch <= '9')
                {
                    temp.Append(ch);
                }
                #endregion
                #region Alphabets and Underscore
                // alphabets and Underscore handled here 
                else if( (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch == '_'))
                {
                    if (Regex.IsMatch(temp.ToString(), @"[^_\w]$") && !isMultilineEnd && !isStringEnd)
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                    }
                    temp.Append(ch);
                }
                #endregion
                #region Less Than < Greater Than >
                else if (ch == '<' || ch == '>')
                {
                    if (isStringEnd || isMultilineEnd)
                    {
                        temp.Append(ch);
                    }
                    else if ( (WordsToBreak.Length > i+1) && WordsToBreak[i + 1] == '=')
                    {
                        if (temp.ToString() != String.Empty)
                        {
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                        }
                        temp.Append(ch.ToString() + WordsToBreak[++i].ToString());
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
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
                #endregion
                #region Exclamation Mark !
                else if (ch == '!')
                {
                    if (isMultilineEnd || isStringEnd)
                    {
                        temp.Append(ch);
                    } else if ( (WordsToBreak.Length > i+1) && WordsToBreak[i + 1] == '=')
                    {
                        i++;
                        temp.Append("!=");
                    }
                    else
                    {
                        if (temp.ToString() != "")
                        {
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                        }
                        else
                        {
                            Words.Add(new RawWords("!", lineNumber));
                        }
                    }

                }
                #endregion
                #region equal to
                else if (ch == '=')
                {
                    if (isStringEnd || isMultilineEnd)
                    {
                        temp.Append(ch);
                    }
                    else if (WordsToBreak.Length > i + 1 && WordsToBreak[i + 1] == '=')
                    {
                        if (temp.ToString() != String.Empty)
                        {
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();

                        }
                        temp.Append("==");
                        i++;
                    }
                    else
                    {
                        temp.Append(ch);
                    }
                    Words.Add(new RawWords(temp.ToString(), lineNumber));
                    temp.Clear();
                }
                #endregion
                #region Brackets Mod and Comma
                // ( ) % , handeled as they function similarly as for off now, i may have to seperate comma later.
                else if (ch == '(' || ch == ')' || ch == '%' || ch == ',' || ch == '{' || ch == '}' || ch == '[' || ch == ']')
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
                #endregion
                #region Double Quotes "
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
                #endregion
                #region remaining symbols
                // handling remaining symbols & special character
                // please refer to an ASCII table to understand what i did here
                else if ((ch > ' ' && ch < 'A') || (ch >= '{' && ch <= '~') || (ch >= '[' && ch <= '`'))
                {
                    temp.Append(ch);
                    /*
                    if (isStringEnd || isMultilineEnd)
                    {
                        temp.Append(ch);
                    }
                    else
                    {
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                        temp.Append(ch);
                    }*/
                }
                #endregion
                #region tab
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
                    i--;
                    if (NextIndent < LastIndent)
                    {
                        Words.Add(new RawWords(lineNumber-1,"BodyEnd"));
                        temp.Clear();
                    }
                    else if (NextIndent > LastIndent)
                    {
                        Words.Add(new RawWords(lineNumber, "BodyStart"));
                        temp.Clear();
                    }

                    LastIndent = (ushort)NextIndent;
                }
                #endregion
                #region newLine and Carriage
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
                        var j = i;
                        var NextIndent = 0;
                        while (WordsToBreak[j] == '\t')
                        {
                            NextIndent++;
                        }
                        i = j;
                        if (NextIndent < LastIndent)
                        {
                            Words.Add(new RawWords(lineNumber - 1, "BodyEnd"));
                            temp.Clear();
                        }
                        else if (NextIndent > LastIndent)
                        {
                            Words.Add(new RawWords(lineNumber, "BodyStart"));
                            temp.Clear();
                        }
                        lineNumber++;
                    }
                }
                #endregion
                #region no case
                // no case occured
                else
                {
                    Words.Add(new RawWords(temp.ToString(), lineNumber));
                    temp.Clear();
                }
                #endregion
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
