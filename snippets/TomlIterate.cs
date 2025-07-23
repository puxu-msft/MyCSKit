
        int ParseAndRun(TomlNode opaque)
        {
            if (opaque.IsTable) {
                return RunTask(opaque.AsTable);
            }

            if (opaque.IsArray) {
                var array = opaque.AsArray;
                for (int i = 0; i < array.ChildrenCount; i++) {
                    TomlNode def = array[i];
                    int exitcode = ParseAndRun(def);
                    if (exitcode != 0) {
                        return exitcode;
                    }
                }
            }

            Console.WriteLine($"Invalid injector task: {opaque}");
            return -1;
        }
