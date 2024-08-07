﻿namespace Optovka.Model
{
    public static class StringExtensions
    {
        public static bool ContainsAny(this string value, string symbols) 
        {
            foreach (var symbol in value)
            {
                if (symbols.Contains(symbol))
                    return true;
            }
            return false; 
        }
    }
}