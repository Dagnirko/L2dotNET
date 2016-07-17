﻿using System.Collections.Generic;

namespace L2dotNET.GameService.Tables
{
    class FString
    {
        private static readonly FString Inst = new FString();

        public static FString GetInstance()
        {
            return Inst;
        }

        public SortedList<int, string> Strings = new SortedList<int, string>();

        public string Get(int p)
        {
            if (Strings.ContainsKey(p))
                return Strings[p];

            return "" + p;
        }
    }
}