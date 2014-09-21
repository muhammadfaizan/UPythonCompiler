using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace UPythonCompiler
{
    class BodyStack
    {
        public int[] Stack;
        private byte top;
        public BodyStack()
        {
            Stack = new int[100];
            top = 0;
        }
        public void Push (int Element)
        {
            try
            {
                Stack[top++] = Element;
            }
            catch (Exception e)
            {
                top = 99;
                throw e;
            }
        }
        public bool IsEmpty()
        {
            return (top == 0) ? true : false;
        }
        public int Pop()
        {
            try
            {
                return Stack[--top];
            }
            catch
            {
                top = 0;
                return -1;
            }
        }

        public int Peek()
        {
            try
            {
                return Stack[top - 1];
            }
            catch
            {
                return -1;
            }
        }
        public int PeekBeneath()
        {
            try
            {
                return Stack[top - 2];
            }
            catch
            {
                return -1;
            }
        }
        public int[] Copy()
        {
            var c = new int[100];
            int i = 0;
            foreach (var number in Stack)
            {
                c[i++] = number;
            }
            return c;
        }
    }
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
            return @"^[\+\-]?[\d]+$";
        }

        private static string singleQuoteString()
        {
            string pattern = "^\"";
            pattern += @".*";
            pattern += "[^";
            pattern += Regex.Escape('\\'.ToString()); 
            pattern += "]?";
            pattern += "\"$";

            return pattern;
        }
        private static string tripleQuoteString()
        {
            string pattern = "";
            pattern += "^\"\"\"";
            pattern += @".*";
            pattern += "[^";
            pattern += Regex.Escape('\\'.ToString());
            pattern += "]?";
            pattern +="\"\"\"$";            
            return pattern;
        }
        #endregion
        private static void parse(RawWords Word)
        {
            //var strPattern = strRegex();
            string input = Word.GetWord();
            var isSingleQuote = Regex.IsMatch(input, singleQuoteString());
            var isTripleQuote = Regex.IsMatch(input, tripleQuoteString());

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
                else if (isSingleQuote || isTripleQuote || input == "\"\"") 
                    //if (Regex.IsMatch(input, "^\"" + @"[\w\s\W]*" + "[^\\]"+"\\" +"$") || Regex.IsMatch(input, "^\"\"\"" + @"[\w\s\W]*" + "\"\"\"$"))
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
                // handling plus Minus(+ -)
                else if (input == "+" || input == "-")
                {
                    Word.token = "P_M";
                }
                else if (input == "%")
                {
                    Word.token = "Mod";
                }
                else if (input == "/" || input == "*")
                {
                    Word.token = "Mul_Div";
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
                #region seperators
                else if (input == ":")
                {
                    Word.token = "Colon";
                }
                else if (input == ".")
                {
                    Word.token = "Dot";
                }
                else if (input == ",")
                {
                    Word.token = "Comma";
                }
                #endregion
                #region invalid
                else
                {
                    if (Word.token == null)
                    {
                        Word.token = "Invalid";
                    }
                }
                #endregion
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
            /*
            Random randomGenerator = new Random();
            NewFileGenerated = "output" + randomGenerator.Next(0,100).ToString()+ ".txt";
            System.IO.StreamWriter FileToWrite = new System.IO.StreamWriter(NewFileGenerated);
            FileToWrite.Write(Token);
            */
            return Token.ToString();

        }

        private static List<RawWords> WordBreak(string WordsToBreak)
        {
            int LastIndent = 0;
            List<RawWords> Words = new List<RawWords>();
            StringBuilder temp = new StringBuilder();
            BodyStack BS = new BodyStack();

            int lineNumber = 1;
            bool isMultilineEnd = false;
            bool isStringEnd = false;
            char ch;
            var i = 0;
            ch = WordsToBreak[i];
            while(ch == '\t' || ch == ' ')
            {
                LastIndent += (ch=='\t')? 4 : 1;
                ch = WordsToBreak[++i];            
            }
            Words.Add(new RawWords(lineNumber, "BodyStart"));
            BS.Push(LastIndent);
            for (; i < WordsToBreak.Length;i++)
            {
                ch = WordsToBreak[i];
                // space handled here
                #region space, tab and comment case here
                if (ch == ' ' || ch=='\t' || ch == '#')
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
                    if (ch == '#')
                    {
                        if ( WordsToBreak.IndexOf('\r', i) != -1)
                        {
                            i = WordsToBreak.IndexOf('\r', i) - 1;
                        }
                        else
                        {
                            i = WordsToBreak.Length - 1;
                        }
                        
                        
                        continue;
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
                        if ((Regex.IsMatch(temp.ToString(), intRegex()) || Regex.IsMatch(temp.ToString(), floatRegex())) && WordsToBreak[i+1] != '=')
                        {
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                            temp.Append(ch);
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                        }
                        else
                        {
                            //then push it
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                            temp.Append(ch);
                        }
                    } 
                    if ((WordsToBreak.Length > i + 1 && WordsToBreak[i + 1] == '=') 
                        && !isMultilineEnd && !isStringEnd 
                        && temp.ToString() == String.Empty)
                    {
                        temp.Append(ch.ToString() + "=");
                        i++;
                        Words.Add(new RawWords(temp.ToString(), lineNumber));
                        temp.Clear();
                    }
                    
                    // 
                    //temp.Clear();
                    
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
                #region backslash \ case handled
                else if (ch == '\\')
                {
                    if (isMultilineEnd || isStringEnd)
                    {
                        if (WordsToBreak.Length > i + 1)
                        {
                            temp.Append(ch.ToString() + WordsToBreak[i + 1].ToString());
                            i++;
                        }
                        else
                        {
                            temp.Append(ch);
                        }
                    }
                    else
                    {
                        temp.Append(ch);
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
                        if (temp.ToString() != String.Empty)
                        {
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                        }
                        temp.Append(ch);
                    }
                    if (temp.ToString() != String.Empty)
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
                    if ((WordsToBreak.Length >= i + 2) && WordsToBreak.Substring(i, 3) == "\"\"\"" && isStringEnd==false)
                    {
                        if (isMultilineEnd == false)
                        {
                            if (temp.ToString() != String.Empty)
                            {
                                Words.Add(new RawWords(temp.ToString(), lineNumber));
                                temp.Clear();
                            }
                            isMultilineEnd = true;
                        }
                        temp.Append(ch + "\"\"");
                        i += 2;
                        if (isMultilineEnd == true)
                        {
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                        }
                    }
                    else
                    {
                        if (isMultilineEnd == true)
                        {
                            temp.Append(ch);
                        } 
                        else if (isStringEnd == false)
                        {
                            isStringEnd = true;
                            if (temp.ToString() != "")
                            {
                                Words.Add(new RawWords(temp.ToString(), lineNumber));
                                temp.Clear();
                            }
                            temp.Append(ch);
                        }
                        else
                        {
                            temp.Append(ch);
                            Words.Add(new RawWords(temp.ToString(), lineNumber));
                            temp.Clear();
                            isStringEnd = false;
                        }

                    }
                }
                #endregion
                #region remaining symbols
                // handling remaining symbols & special character
                // please refer to an ASCII table to understand what i did here
                else if ((ch > ' ' && ch < 'A') || (ch >= '{' && ch <= '~') || (ch >= '[' && ch <= '`'))
                {
                    temp.Append(ch);
                }
                #endregion
                #region tab
                // tabs handled here
                else if (ch == '\t')
                {
                    if (isMultilineEnd || isStringEnd)
                    {
                        temp.Append(ch);
                        continue;
                    }
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

                    LastIndent = NextIndent;
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
                        lineNumber++;
                        if( isMultilineEnd)
                        {
                            continue;
                        }
                        var j = i+1;
                        var NextIndent = 0;
                        if ( j < WordsToBreak.Length)
                        { 
                            ch = WordsToBreak[j];
                            while (ch == '\t' || ch == ' ')
                            {
                                NextIndent += (ch=='\t')? 4:1;
                                ch = WordsToBreak[++j];
                            }
                            i = (j - 1);
                            LastIndent = BS.Peek();
                            if (NextIndent != LastIndent)
                            {
                                if (NextIndent > LastIndent)
                                {
                                    BS.Push(NextIndent);
                                    Words.Add(new RawWords(lineNumber, "BodyStart"));
                                }
                                else if (NextIndent < LastIndent)
                                {
                                    var bodyescape = 0;
                                    while (!BS.IsEmpty() && NextIndent < BS.Peek())
                                    {
                                        bodyescape++;
                                        LastIndent = BS.Pop();
                                    }
                                    LastIndent = BS.Peek();
                                    if (LastIndent != NextIndent || LastIndent == -1) // right conditions first case passing
                                    {
                                        Words.Add(new RawWords(lineNumber, "InvalidIndentation")); // first case passing
                                    }
                                    else if (LastIndent == NextIndent)
                                    {
                                        for (var k = 0; k < bodyescape; k++)
                                        {
                                            Words.Add(new RawWords(lineNumber, "BodyEnd"));
                                        }
                                    }
                                }
                                    
                                
                           }

                        }
                        
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
            Words.Add(new RawWords(lineNumber, "BodyEnd"));

            return Words;
        }
    }
}
