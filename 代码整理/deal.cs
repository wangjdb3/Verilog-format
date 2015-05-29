using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace 代码整理
{
    class Deal
    {
        public struct Key
        {
            public int index;
            public string key;
            public string value;
        }
        public static int deal(ref string strTmp, ref Last last)
        {
            //            char[] char_tmp = new char[strTmp.Length];
            //            strTmp.CopyTo(0, char_tmp, 0, strTmp.Length);
            List<Key> list_key = new List<Key>();
            get_key_deal(ref strTmp, ref list_key);
            last.note_index = -1;
            if (list_key.Count != 0 && !last.is_module_use)
            {
                int indexup = 0;
                for (int i = 0; i < list_key.Count; i++)
                {
                    switch (list_key[i].key)
                    {
                        case "//":
                            last.note_index = list_key[i].index;
                            break;
                        case "put":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                last.is_put = true;
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                strTmp = strTmp.Insert(list_key[i].index + indexup, "\t");
                                indexup -= list_key[i].value.Length;
                                indexup += 1;
                                Regex reg = new Regex(@"(reg|wire)");
                                if (reg.IsMatch(strTmp))
                                {
                                    reg = new Regex(@"(?<=(input|output))\s*");
                                    Match match = reg.Match(strTmp);
                                    if (match.Success)
                                    {
                                        strTmp = strTmp.Remove(match.Index, match.Length);
                                        strTmp = strTmp.Insert(match.Index, " ");
                                        indexup -= match.Length;
                                        indexup += 1;
                                    }
                                }
                                else
                                {
                                    reg = new Regex(@"(?<=[]])\s*");
                                    Match match = reg.Match(strTmp, list_key[i].index + indexup);
                                    indexup -= list_key[i].value.Length;
                                    indexup += 1;
                                    if (match.Success)
                                    {
                                        strTmp = strTmp.Remove(match.Index, match.Length);
                                        strTmp = strTmp.Insert(match.Index, "\t");
                                        indexup -= match.Length;
                                        indexup += 1;
                                    }
                                    else
                                    {
                                        reg = new Regex(@"(?<=(input|output))\s*");
                                        match = reg.Match(strTmp);
                                        if (match.Success)
                                        {
                                            strTmp = strTmp.Remove(match.Index, match.Length);
                                            strTmp = strTmp.Insert(match.Index, "\t");
                                            indexup -= match.Length;
                                            indexup += 1;
                                        }
                                    }
                                }
                            }
                            break;
                        case "wire":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                if (last.is_put)
                                {
                                    strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, " ");
                                    last.is_put = false;
                                }
                                else
                                {
                                    strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, "\t");
                                }
                                Regex reg = new Regex(@"(?<=[]])\s*");
                                Match match = reg.Match(strTmp, list_key[i].index + indexup);
                                indexup -= list_key[i].value.Length;
                                indexup += 1;
                                if (match.Success)
                                {
                                    strTmp = strTmp.Remove(match.Index, match.Length);
                                    strTmp = strTmp.Insert(match.Index, "\t");
                                    indexup -= match.Length;
                                    indexup += 1;
                                }
                                else
                                {
                                    reg = new Regex(@"(?<=(reg|wire))\s*");
                                    match = reg.Match(strTmp);
                                    if (match.Success)
                                    {
                                        strTmp = strTmp.Remove(match.Index, match.Length);
                                        strTmp = strTmp.Insert(match.Index, "\t");
                                        indexup -= match.Length;
                                        indexup += 1;
                                    }
                                }
                            }
                            break;
                        case "always":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                strTmp = strTmp.Insert(list_key[i].index + indexup, "\t");
                                indexup -= list_key[i].value.Length;
                                indexup += 1;
                                last.num_t = 2;
                            }
                            break;
                        case "assign":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                strTmp = strTmp.Insert(list_key[i].index + indexup, "\t");
                                indexup -= list_key[i].value.Length;
                                indexup += 1;
                                last.num_t = 0;
                            }
                            break;
                        case "begin":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                last.num_begin += 1;
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                string sp = "";
                                for (int j = 0; j < last.num_t; j++)
                                {
                                    sp += "\t";
                                }
                                strTmp = strTmp.Insert(list_key[i].index + indexup, sp);
                                if (list_key[i].index != 0)
                                {
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, "\n");
                                    indexup += 1;
                                }
                                indexup -= list_key[i].value.Length;
                                indexup += last.num_t;
                            }
                            break;
                        case "end":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                last.num_begin -= 1;
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                string sp = "";
                                for (int j = 0; j < last.num_t; j++)
                                {
                                    sp += "\t";
                                }
                                strTmp = strTmp.Insert(list_key[i].index + indexup, sp);
                                if (list_key[i].index != 0)
                                {
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, "\n");
                                    indexup += 1;
                                }
                                indexup -= list_key[i].value.Length;
                                indexup += last.num_t;
                                last.num_t -= 1;
                            }
                            break;
                        case "if":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                string sp = "";
                                for (int j = 0; j < last.num_t; j++)
                                {
                                    sp += "\t";
                                }
                                strTmp = strTmp.Insert(list_key[i].index + indexup, sp);
                                if (list_key[i].index != 0)
                                {
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, "\n");
                                    indexup += 1;
                                }
                                indexup -= list_key[i].value.Length;
                                indexup += last.num_t;
                                last.num_t += 1;
                            }
                            break;
                        case "else_if":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                string sp = "";
                                for (int j = 0; j < last.num_t; j++)
                                {
                                    sp += "\t";
                                }
                                strTmp = strTmp.Insert(list_key[i].index + indexup, sp);
                                if (list_key[i].index != 0)
                                {
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, "\n");
                                    indexup += 1;
                                }
                                indexup -= list_key[i].value.Length;
                                indexup += last.num_t;
                                last.num_t += 1;
                            }
                            break;
                        case "case":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                string sp = "";
                                for (int j = 0; j < last.num_t; j++)
                                {
                                    sp += "\t";
                                }
                                strTmp = strTmp.Insert(list_key[i].index + indexup, sp);
                                if (list_key[i].index != 0)
                                {
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, "\n");
                                    indexup += 1;
                                }
                                indexup -= list_key[i].value.Length;
                                indexup += last.num_t;
                            }
                            break;
                        case "case_:":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                string sp = "";
                                for (int j = 0; j < last.num_t; j++)
                                {
                                    sp += "\t";
                                }
                                strTmp = strTmp.Insert(list_key[i].index + indexup, sp);
                                if (list_key[i].index != 0)
                                {
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, "\n");
                                    indexup += 1;
                                }
                                indexup -= list_key[i].value.Length;
                                indexup += last.num_t;
                                last.num_t += 1;
                            }
                            break;
                        case "endcase":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                string sp = "";
                                for (int j = 0; j < last.num_t; j++)
                                {
                                    sp += "\t";
                                }
                                strTmp = strTmp.Insert(list_key[i].index + indexup, sp);
                                if (list_key[i].index != 0)
                                {
                                    strTmp = strTmp.Insert(list_key[i].index + indexup, "\n");
                                    indexup += 1;
                                }
                                indexup -= list_key[i].value.Length;
                                indexup += last.num_t;
                                //last.num_t -= 1;
                            }
                            break;
                        case "common":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                Regex reg = new Regex(@"\b(module|assign|initial|wire|reg)\b");
                                Match match = reg.Match(list_key[i].value);
                                if (!match.Success)
                                {
                                    reg = new Regex(@"\s*(?=[`\w]+.*;)");
                                    match = reg.Match(strTmp);
                                    strTmp = strTmp.Remove(match.Index, match.Length);
                                    string sp = "";
                                    for (int j = 0; j < last.num_t; j++)
                                    {
                                        sp += "\t";
                                    }
                                    strTmp = strTmp.Insert(match.Index, sp);
                                    if (match.Index != 0)
                                    {
                                        strTmp = strTmp.Insert(match.Index, "\n");
                                        indexup += 1;
                                    }
                                    indexup -= match.Length;
                                    indexup += last.num_t;
                                }
                            }
                            break;
                        case "module_use":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                Regex reg = new Regex(@"\bmodule\b");
                                Match match = reg.Match(list_key[i].value);
                                if (match.Success)
                                {
                                    break;
                                }
                                else
                                {
                                    reg = new Regex(@"\s*(?=\S+\s+\S+[(]\n)");
                                    match = reg.Match(strTmp);
                                    if (match.Success)
                                    {
                                        last.is_module_use = true;
                                        strTmp = strTmp.Remove(match.Index + indexup, match.Length);
                                        strTmp = strTmp.Insert(match.Index + indexup, "\t");
                                        indexup -= match.Length;
                                        indexup += 1;
                                        last.num_t = 2;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                indexup = 0;
                last.note_index = -1;
            }
            else if (list_key.Count != 0 && last.is_module_use)
            {
                int indexup = 0;
                for (int i = 0; i < list_key.Count; i++)
                {
                    switch (list_key[i].key)
                    {
                        case "module_use_":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                strTmp = strTmp.Insert(list_key[i].index + indexup, "\t\t");
                                indexup -= list_key[i].value.Length;
                                indexup += 2;
                            }
                            break;
                        case "module_use_end":
                            if (list_key[i].index < last.note_index || last.note_index == -1)
                            {
                                last.is_module_use = false;
                                strTmp = strTmp.Remove(list_key[i].index + indexup, list_key[i].value.Length);
                                strTmp = strTmp.Insert(list_key[i].index + indexup, "\t");
                                indexup -= list_key[i].value.Length;
                                indexup -= 1;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return 1;
        }
        public static void get_key_deal(ref string strTmp, ref List<Key> list_key)
        {
            string pattern = @"\s*(?=(//|/\*))";
            string key = "//";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*(?=\b(input|output)\b)";
            key = "put";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*(?=\b(reg|wire)\b)";
            key = "wire";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*(?=\b(always|initial)\b)";
            key = "always";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*(?=\b(assign)\b)";
            key = "assign";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*(?=\b(begin)\b)";
            key = "begin";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*(?=\b(end)\b)";
            key = "end";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*[`\w]+.*;";
            key = "common";
            get_key(ref strTmp, ref list_key, pattern, key);
            get_if_else(ref strTmp, ref list_key);
            get_case(ref strTmp, ref list_key);
            pattern = @"\s*\S+\s+\S+[(]\n";
            key = "module_use";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*(?=\.\s*\w+\s*[(].*[)]\s*,?)";
            key = "module_use_";
            get_key(ref strTmp, ref list_key, pattern, key);
            pattern = @"\s*(?=[)];)";
            key = "module_use_end";
            get_key(ref strTmp, ref list_key, pattern, key);
            list_key.Sort(new KeyComparer());
        }
        public static void get_key(ref string strTmp, ref List<Key> list_key, string pattern, string key)
        {
            Regex reg = new Regex(pattern);
            Match match = reg.Match(strTmp);
            if (match.Success)
            {
                Key k1;
                k1.value = match.Value;
                k1.index = match.Index;
                k1.key = key;
                list_key.Add(k1);
            }
        }
        public static void get_if_else(ref string strTmp, ref List<Key> list_key)
        {

            Regex reg = new Regex(@"\s*(?=else\s*if)");
            Match match = reg.Match(strTmp);
            if (match.Success)
            {
                Key k1;
                k1.value = match.Value;
                k1.index = match.Index;
                k1.key = "else_if";
                list_key.Add(k1);
            }
            else
            {
                reg = new Regex(@"\s*(?=\b(if|else)\b)");
                match = reg.Match(strTmp);
                if (match.Success)
                {
                    Key k1;
                    k1.value = match.Value;
                    k1.index = match.Index;
                    k1.key = "if";
                    list_key.Add(k1);
                }
            }
        }
        public static void get_case(ref string strTmp, ref List<Key> list_key)
        {
            Regex reg = new Regex(@"\s*(?=\bcase\b)");
            Match match = reg.Match(strTmp);
            if (match.Success)
            {
                Key k1;
                k1.value = match.Value;
                k1.index = match.Index;
                k1.key = "case";
                list_key.Add(k1);
            }
            reg = new Regex(@"\s*(?=[`\w]+.*:)");
            match = reg.Match(strTmp);
            if (match.Success)
            {
                reg = new Regex(@"[[]");
                if (!reg.IsMatch(strTmp,match.Index))
                {
                    Key k1;
                    k1.value = match.Value;
                    k1.index = match.Index;
                    k1.key = "case_:";
                    list_key.Add(k1);
                }
            }
            reg = new Regex(@"\s*(?=\bendcase\b)");
            match = reg.Match(strTmp);
            if (match.Success)
            {
                Key k1;
                k1.value = match.Value;
                k1.index = match.Index;
                k1.key = "endcase";
                list_key.Add(k1);
            }
        }
        public struct Last
        {
//            public bool is_n;
//            public bool is_note;
//            public bool is_include_not1;
//            public bool is_have_note;
            public bool is_put;
            public bool is_module_use;
            public int note_index;
            public int num_t;
//            public int num_if;
//            public int num_else;
//            public int num_case;
            public int num_begin;
        }
        public static int delete_space(ref string strBody, ref string strBody_done, ref int index)
        {
            Regex reg = new Regex(@"[^\n]*\n");
            Match match = reg.Match(strBody, index);
            if (match.Success)
            {
                string str_Read = match.Value;
                index = match.Index + match.Length;
                reg = new Regex(@"^[ \t]*(?=[^ \t])");
                match = reg.Match(str_Read);
                if (match.Success)
                {
                    strBody_done += str_Read.Remove(match.Index, match.Length);
                }
                return 1;
            }
            else
            {
                reg = new Regex(@"\s*\w+.*");
                match = reg.Match(strBody, index);
                if (match.Success)
                {
                    string str_Read = match.Value;
                    index = match.Index + match.Length;
                    reg = new Regex(@"^[ \t]*(?=[^ \t])");
                    match = reg.Match(str_Read);
                    if (match.Success)
                    {
                        strBody_done = strBody_done + str_Read.Remove(match.Index, match.Length) + "\n";
                    }
                    return 1;
                }
                else
                    return 0;
            }
        }
        public static int deal1(ref string strBody, ref string strBody_done, ref Last last, ref int index)
        {
            Regex reg = new Regex("[^\\n]{0,}\\n");
            Match match = reg.Match(strBody, index);
            string strTmp = "";
            if (match.Success)
            {
                strTmp = match.Value;
                index = match.Index + match.Length;
                deal(ref strTmp, ref last);
                strBody_done += strTmp;
            }
            else
            {
                return 0;
            }
            return 1;
        }
        public class KeyComparer : IComparer<Key>
        {
            //实现num升序
            public int Compare(Key x, Key y)
            {
                return (x.index - y.index);
            }
        }
    }
}
