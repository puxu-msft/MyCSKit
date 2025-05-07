using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                throw new NotFoundException($"NotFound: {path}");
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
                throw new TypeMismatchException($"NotArray: {path} {leaf}");
            return leaf.AsArray;
        }

        public IEnumerable<TR> Iterate<TR>(string path, Func<TomlNode, TR> fn)
            where TR : TomlNode
        {
            var opaque = Root.WalkNode(path, throwIfNotFound: false);
            if (opaque == null) {
                yield break;
            }
            if (opaque.IsArray) {
                foreach (var node in opaque.AsArray.RawArray) {
                    var ret = fn(node);
                    yield return ret;
                }
            }
            else {
                // if (opaque.IsTable) {
                    var ret = fn(opaque);
                    yield return ret;
                // }
            }
        }

#if NET8_0_OR_GREATER
        public async IAsyncEnumerable<TR> IterateAsync<TR>(string path, Func<TomlNode, Task<TR>> fn)
            where TR : TomlNode
        {
            var opaque = Root.WalkNode(path, throwIfNotFound: false);
            if (opaque == null) {
                yield break;
            }
            if (opaque.IsArray) {
                foreach (var node in opaque.AsArray.RawArray) {
                    var ret = await fn(node);
                    yield return ret;
                }
            }
            else {
                // if (opaque.IsTable) {
                    var ret = await fn(opaque);
                    yield return ret;
                // }
            }
        }
#endif

        public void Iterate(string path, Action<TomlNode> fn) {
            foreach (var node in Iterate(path, (e) => e)) {
                fn(node);
            }
        }

        // 兼容写法：同步调用异步函数
        // public void Iterate(string path, Func<TomlNode, Task> fn) {
        //     foreach (var node in Iterate(path, (e) => e)) {
        //         var task = fn(node);
        //         if (fn.Method.IsDefined(typeof(AsyncStateMachineAttribute), inherit: false)) {
        //             task.GetAwaiter().GetResult();
        //         }
        //         else {
        //             task.Wait();
        //         }
        //     }
        // }

        public async Task IterateAsync(string path, Func<TomlNode, Task> fn) {
            foreach (var node in Iterate(path, (e) => e)) {
                await fn(node);
            }
        }

        public string GetString(string path, string? defaultValue = null) {
            var leaf = Root.WalkNode(path, throwIfNotFound: (defaultValue == null));
            if (leaf == null) {
                return defaultValue;
            }
            if (!leaf.HasValue) {
                throw new TypeMismatchException($"NotValue: {path} {leaf}");
            }
            return leaf.AsString;
        }

        public long GetLong(string path, long? defaultValue = null) {
            var leaf = Root.WalkNode(path, throwIfNotFound: (defaultValue == null));
            if (leaf == null) {
                return defaultValue.Value;
            }
            if (!leaf.IsInteger) {
                throw new TypeMismatchException($"NotInteger: {path} {leaf}");
            }
            return leaf.AsInteger;
        }

        public int GetInt(string path, int? defaultValue = null) {
            return (int)GetLong(path, defaultValue);
        }

        public bool GetBool(string path, bool? defaultValue = null) {
            var leaf = Root.WalkNode(path, throwIfNotFound: (defaultValue == null));
            if (leaf == null) {
                return defaultValue.Value;
            }
            if (!leaf.IsBoolean) {
                throw new TypeMismatchException($"NotBoolean: {path} {leaf}");
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
