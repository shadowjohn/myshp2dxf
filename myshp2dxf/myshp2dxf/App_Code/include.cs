using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace utility
{
    public class myinclude
    {
        private Random rnd = new Random(DateTime.Now.Millisecond);
        public myinclude()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }
        public bool is_file(string filepath)
        {
            return File.Exists(filepath);
        }
        public bool in_array(string find_key, List<string> arr)
        {
            return arr.Contains(find_key);
        }
        public bool in_array(string find_key, string[] arr)
        {
            return arr.Contains(find_key);
        }
        public bool in_array(string find_key, char[] arr)
        {
            string[] o = new string[arr.Count()];
            for (int i = 0; i < arr.Count(); i++)
            {
                o[i] = arr[i].ToString();
            }
            return in_array(find_key, o);
        }
        public bool in_array(string find_key, ArrayList arr)
        {
            return arr.Contains(find_key);
        }
        public string EscapeUnicode(string input)
        {
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (char ch in input)
            {
                if (ch <= 0x7f)
                    sb.Append(ch);
                else
                    sb.AppendFormat(CultureInfo.InvariantCulture, "\\u{0:x4}", (int)ch);
            }
            return sb.ToString();
        }
        public string unEscapeUnicode(string input)
        {
            return Regex.Unescape(input);
        }
        public string dirname(string path)
        {
            return Directory.GetParent(path).FullName;
        }
        public string basename(string path)
        {
            return Path.GetFileName(path);
        }
        public string mainname(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
        public string subname(string path)
        {
            return Path.GetExtension(path).TrimStart('.');
        }
        public string json_encode(object input)
        {
            return JsonConvert.SerializeObject(input, Formatting.None);
        }
        public JArray json_decode(string input)
        {
            input = input.Trim();
            if (input.Length != 0)
            {
                if (input.Substring(1, 1) != "[")
                {
                    input = "[" + input + "]";
                    return (JArray)JsonConvert.DeserializeObject<JArray>(input);
                }
                else
                {
                    return (JArray)JsonConvert.DeserializeObject<JArray>(input);
                }
            }
            else
            {
                return null;
            }
        }
        public string deepReplace(string input, string target, string replacement)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int index = input.IndexOf(target);
            if (index == -1)
                return input;

            string before = input.Substring(0, index);
            string after = input.Substring(index + target.Length);

            return before + replacement + deepReplace(after, target, replacement);
        }
        public string json_format(string input)
        {
            JArray jdod = json_decode(input);
            return EscapeUnicode(JsonConvert.SerializeObject(jdod, Formatting.Indented));
        }
        public bool is_string_like_new(string data, string find_string)
        {
            /*
              is_string_like($data,$fine_string)

              $mystring = "Hi, this is good!";
              $searchthis = "%thi% goo%";

              $resp = string_like($mystring,$searchthis);


              if ($resp){
                 echo "milike = VERDADERO";
              } else{
                 echo "milike = FALSO";
              }

              Will print:
              milike = VERDADERO

              and so on...

              this is the function:
            */
            bool tieneini = false;
            if (find_string == "") return true;
            var vi = explode("%", find_string);
            int offset = 0;
            for (int n = 0, max_n = vi.Count(); n < max_n; n++)
            {
                if (vi[n] == "")
                {
                    if (vi[0] == "")
                    {
                        tieneini = true;
                    }
                }
                else
                {
                    //newoff =  strpos(data,vi[$n],offset);
                    int newoff = data.IndexOf(vi[n], offset);
                    if (newoff != -1)
                    {
                        if (!tieneini)
                        {
                            if (offset != newoff)
                            {
                                return false;
                            }
                        }
                        if (n == max_n - 1)
                        {
                            if (vi[n] != data.Substring(data.Length - vi[n].Length, vi[n].Length))
                            {
                                return false;
                            }

                        }
                        else
                        {
                            offset = newoff + vi[n].Length;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public string UTF8toBig5(string utf8Text)
        {
            // 嘗試以UTF-8解碼
            Encoding utf8Encoding = Encoding.UTF8;
            byte[] utf8Bytes = utf8Encoding.GetBytes(utf8Text);

            // 嘗試以Big5解碼
            Encoding big5Encoding = Encoding.GetEncoding("big5");
            byte[] big5Bytes = Encoding.Convert(utf8Encoding, big5Encoding, utf8Bytes);

            // 將字節數組轉換回Big5編碼的文本
            string big5Text = big5Encoding.GetString(big5Bytes);

            return big5Text;
        }
        public string UTF8toCP950(string utf8Text)
        {
            // 嘗試以UTF-8解碼
            Encoding utf8Encoding = Encoding.UTF8;
            byte[] utf8Bytes = utf8Encoding.GetBytes(utf8Text);

            // 嘗試以CP950解碼
            Encoding cp950Encoding = Encoding.GetEncoding(950);
            byte[] cp950Bytes = Encoding.Convert(utf8Encoding, cp950Encoding, utf8Bytes);

            // 將字節數組轉換回CP950編碼的文本
            string cp950Text = cp950Encoding.GetString(cp950Bytes);

            return cp950Text;
        }
        public string Big5ToUTF8(string big5Text)
        {
            // 嘗試以Big5解碼
            Encoding big5Encoding = Encoding.GetEncoding("big5");
            byte[] big5Bytes = big5Encoding.GetBytes(big5Text);

            // 嘗試以UTF-8解碼
            Encoding utf8Encoding = Encoding.UTF8;
            byte[] utf8Bytes = Encoding.Convert(big5Encoding, utf8Encoding, big5Bytes);

            // 將字節數組轉換回UTF-8編碼的文本
            string utf8Text = utf8Encoding.GetString(utf8Bytes);

            return utf8Text;
        }
        public string CP950ToUTF8(string cp950Text)
        {
            // 嘗試以CP950解碼
            Encoding cp950Encoding = Encoding.GetEncoding(950);
            byte[] cp950Bytes = cp950Encoding.GetBytes(cp950Text);

            // 嘗試以UTF-8解碼
            Encoding utf8Encoding = Encoding.UTF8;
            byte[] utf8Bytes = Encoding.Convert(cp950Encoding, utf8Encoding, cp950Bytes);

            // 將字節數組轉換回UTF-8編碼的文本
            string utf8Text = utf8Encoding.GetString(utf8Bytes);

            return utf8Text;
        }

        public bool is_string_like(string data, string find_string)
        {
            return (data.IndexOf(find_string) == -1) ? false : true;
        }
        public bool is_istring_like(string data, string find_string)
        {
            return (data.ToUpper().IndexOf(find_string.ToUpper()) == -1) ? false : true;
        }
        public string json_format_utf8(string input)
        {
            JArray jdod = json_decode(input);
            return JsonConvert.SerializeObject(jdod[0], Formatting.Indented);
        }
        public string[] explode(string keyword, string data)
        {
            return data.Split(new string[] { keyword }, StringSplitOptions.None);
        }
        public string[] explode(string keyword, object data)
        {
            return data.ToString().Split(new string[] { keyword }, StringSplitOptions.None);
        }
        public string[] explode(string[] keyword, string data)
        {
            return data.Split(keyword, StringSplitOptions.None);
        }
        public string b2s(byte[] input)
        {
            return System.Text.Encoding.UTF8.GetString(input);
        }
        public byte[] s2b(string input)
        {
            return System.Text.Encoding.UTF8.GetBytes(input);
        }
        public void file_put_contents(string filepath, string input)
        {
            file_put_contents(filepath, s2b(input), false);
        }
        public void file_put_contents(string filepath, byte[] input)
        {
            file_put_contents(filepath, input, false);
        }
        public void file_put_contents(string filepath, string input, bool isFileAppend)
        {
            file_put_contents(filepath, s2b(input), isFileAppend);
        }
        public void file_put_contents(string filepath, byte[] input, bool isFileAppend)
        {

            switch (isFileAppend)
            {
                case true:
                    {
                        FileMode FM = new FileMode();
                        if (!is_file(filepath))
                        {
                            FM = FileMode.Create;
                            using (FileStream myFile = File.Open(@filepath, FM, FileAccess.Write, FileShare.Read))
                            {
                                myFile.Seek(myFile.Length, SeekOrigin.Begin);
                                myFile.Write(input, 0, input.Length);
                                myFile.Dispose();
                            }
                        }
                        else
                        {
                            FM = FileMode.Append;
                            using (FileStream myFile = File.Open(@filepath, FM, FileAccess.Write, FileShare.Read))
                            {
                                myFile.Seek(myFile.Length, SeekOrigin.Begin);
                                myFile.Write(input, 0, input.Length);
                                myFile.Dispose();
                            }
                        }
                    }
                    break;
                case false:
                    {
                        using (FileStream myFile = File.Open(@filepath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                        {
                            myFile.Write(input, 0, input.Length);
                            myFile.Dispose();
                        };
                    }
                    break;
            }
        }
        public Dictionary<string, int> ParseRgbHex(string color)
        {
            string pattern = @"^#(?:[0-9a-fA-F]{3}){1,2}$"; // HEX表示的正則表達式
            bool isHex = Regex.IsMatch(color, pattern);

            if (isHex)
            {
                if (color.StartsWith("#"))
                {
                    color = color.Substring(1); // 去掉 "#" 符號
                }

                int r = Convert.ToInt32(color.Substring(0, 2), 16);
                int g = Convert.ToInt32(color.Substring(2, 2), 16);
                int b = Convert.ToInt32(color.Substring(4, 2), 16);

                return new Dictionary<string, int>
            {
                { "R", r },
                { "G", g },
                { "B", b }
            };
            }
            else
            {
                // 將 RGB(255, 165, 0) 格式轉換成數值
                MatchCollection matches = Regex.Matches(color, @"\d+");
                int r = int.Parse(matches[0].Value);
                int g = int.Parse(matches[1].Value);
                int b = int.Parse(matches[2].Value);

                return new Dictionary<string, int>
            {
                { "R", r },
                { "G", g },
                { "B", b }
            };
            }
        }
    }
}
