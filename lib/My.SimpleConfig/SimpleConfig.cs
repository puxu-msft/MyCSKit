using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tommy;

namespace My
{

    public class SimpleConfig
    {
        public static SimpleConfig? Instance { get; set; }

        public TomlTable? m_config;

        public void Init(string path)
        {
            using (StreamReader reader = File.OpenText(path))
            {
                try
                {
                    m_config = TOML.Parse(reader);
                }
                catch (TomlParseException ex)
                {
                    foreach (var ex0 in ex.SyntaxErrors)
                    {
                        Console.WriteLine($"{ex0.Column}:{ex0.Line}: {ex0.Message}");
                    }
                }
            }
        }

        public TomlNode? GetNode(string path)
        {
            TomlNode current = m_config;
            var pieces = path.Split('.');
            foreach (var piece in pieces)
            {
                if (current == null)
                    return null;
                current = current[piece];
            }
            return current.HasValue ? current : null;
        }

        public string GetString(string path, string defaultValue)
        {
            var leaf = GetNode(path);
            if (leaf == null)
                return defaultValue;
            return leaf.AsString;
        }

        public long GetLong(string path, long defaultValue)
        {
            var leaf = GetNode(path);
            if (leaf == null)
                return defaultValue;
            return leaf.AsInteger;
        }

        public int GetInt(string path, int defaultValue) {
            return (int)GetLong(path, defaultValue);
        }

        public bool GetBool(string path, bool defaultValue) {
            var leaf = GetNode(path);
            if (leaf == null)
                return defaultValue;
            return leaf.AsBoolean;
        }
    }
}
