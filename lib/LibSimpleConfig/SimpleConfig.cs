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
        public TomlNode Root { get; protected set; }

        public SimpleConfig(TomlNode root) {
            Root = root;
        }

        public static SimpleConfig? FromFile(string path) {
            using var reader = File.OpenText(path);
            try {
                var root = TOML.Parse(reader);
                if (root == null) {
                    Console.Error.WriteLine($"Empty: {path}");
                }
                return new SimpleConfig(root);
            }
            catch (TomlParseException ex) {
                foreach (var ex0 in ex.SyntaxErrors) {
                    Console.Error.WriteLine($"{ex0.Column}:{ex0.Line}: {ex0.Message}");
                }
            }
            return default;
        }

        public SimpleConfig Go(string path) {
            var target = Root.WalkNode(path);
            if (target == null)
                throw new Exception($"NotFound: {path}");
            return new(target);
        }

        public TomlNode? GetNode(string path) {
            return Root.WalkNode(path, throwIfNotFound: false);
        }

        public TomlArray? GetArray(string path) {
            var leaf = Root.WalkNode(path, throwIfNotFound: false);
            if (leaf == null)
                return default;
            if (!leaf.IsArray)
                throw new Exception($"NotArray: {path} {leaf}");
            return leaf.AsArray;
        }

        public string GetString(string path, string? defaultValue = null) {
            var leaf = Root.WalkNode(path, throwIfNotFound: (defaultValue == null));
            if (leaf == null) {
                return defaultValue;
            }
            if (!leaf.HasValue) {
                throw new Exception($"NotValue: {path} {leaf}");
            }
            return leaf.AsString;
        }

        public long GetLong(string path, long? defaultValue = null) {
            var leaf = Root.WalkNode(path, throwIfNotFound: (defaultValue == null));
            if (leaf == null) {
                return defaultValue.Value;
            }
            if (!leaf.IsInteger) {
                throw new Exception($"NotInteger: {path} {leaf}");
            }
            return leaf.AsInteger;
        }

        public int GetInt(string path, int? defaultValue = null) {
            return (int)GetLong(path, (int)defaultValue);
        }

        public bool GetBool(string path, bool? defaultValue = null) {
            var leaf = Root.WalkNode(path, throwIfNotFound: (defaultValue == null));
            if (leaf == null) {
                return defaultValue.Value;
            }
            if (!leaf.IsBoolean) {
                throw new Exception($"NotBoolean: {path} {leaf}");
            }
            return leaf.AsBoolean;
        }

        public E GetEnum<E>(string path, E defaultValue) where E : struct {
            var s = GetString(path);
            if (s == null) {
                return defaultValue;
            }
            return (E)Enum.Parse(typeof(E), s);
        }
    }
}
