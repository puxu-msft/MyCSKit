using System;
using Tommy;

namespace My
{
    public static partial class Extension
    {

        public static TomlNode? WalkNode(this TomlNode table, string path, bool throwIfNotFound = true)
        {
            TomlNode current = table;
            var pieces = path.Split('.');
            foreach (var piece in pieces) {
                if (current.IsTable) {
                    current = current[piece];
                }
                else if (current.IsArray) {
                    if (int.TryParse(piece, out var index)) {
                        current = current[index];
                    }
                    else {
                        if (throwIfNotFound) {
                            throw new NotFoundException($"InvalidIndex: {piece} in {path}");
                        }
                    }
                }
                else {
                    if (throwIfNotFound) {
                        throw new NotFoundException($"ReachLeaf: {piece} in {path}");
                    }
                    return default;
                }

                if (current == null) {
                    if (throwIfNotFound) {
                        throw new NotFoundException($"NotFound: {piece} in {path}");
                    }
                    return default;
                }
            }

            if (false
                || current.HasValue
                || current.IsArray
                || current.IsTable) {
                return current;
            }

            if (throwIfNotFound) {
                throw new NotFoundException($"NoValue: {path}");
            }
            return default;
        }

    }
}
