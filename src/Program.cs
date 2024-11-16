// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#if !NO_MAIN

using Osm.Importers;
using Osm.IO;

using System;
using System.Collections.Generic;
using System.IO;

namespace Osm
{
    internal static class Program
    {
        private readonly struct ImporterData(char c, string n, Type t)
        {
            public readonly char Char = c;
            public readonly string Name = n;
            public readonly Type Importer = t;
        }

        private static readonly Dictionary<ConsoleKey, ImporterData> _importerData = new() {
            { ConsoleKey.S, new('S', "tudio One", typeof(StudioOneImporter)) },
        };

        public static void Main(string[] argsArray)
        {
            Dictionary<string, List<string>> args = [];
            string? currentArg = null;
            foreach (string arg in argsArray)
            {
                if (arg.StartsWith('-'))
                {
                    currentArg = arg[1..];
                    args.Add(currentArg.ToLower(System.Globalization.CultureInfo.InvariantCulture), []);
                } else if (currentArg != null)
                {
                    args[currentArg].Add(arg);
                }
            }

            string? path;
            if (args.TryGetValue("i", out List<string>? list))
            {
                if (list.Count > 1)
                {
                    Console.WriteLine("Too many input paths");
                    return;
                }

                path = list[0];
            } else
                path = null;

            string? rpath;
            if (args.TryGetValue("o", out list))
            {
                if (list.Count > 1)
                {
                    Console.WriteLine("Too many output paths");
                    return;
                }

                rpath = list[0];
            } else
                rpath = null;

            Console.WriteLine("Open Song Meta Conversion Tool");
            Console.WriteLine();

            Type? importerType = null;
            bool keepLooping = true;
            while (true)
            {
                Console.WriteLine("Select import mode:");
                Console.WriteLine("[N]one");
                foreach (ImporterData data in _importerData.Values)
                {
                    Console.Write('[');
                    Console.Write(data.Char);
                    Console.Write(']');
                    Console.WriteLine(data.Name);
                }

                Console.Write("> ");
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);
                } while (char.IsWhiteSpace(key.KeyChar));
                Console.WriteLine(key.KeyChar);

                if (_importerData.TryGetValue(key.Key, out var value))
                {
                    importerType = value.Importer;
                    keepLooping = false;
                } else if (key.Key == ConsoleKey.N)
                {
                    keepLooping = false;
                }

                if (keepLooping)
                {
                    Console.WriteLine("That is not a valid mode.");
                    Console.WriteLine();
                } else
                    break;
            }

            using IMetaImporter? importer = importerType != null ? (IMetaImporter?)Activator.CreateInstance(importerType) : null;

            if (path != null)
            {
                if (!File.Exists(path) || (importer != null && !importer.ValidatePath(path)))
                    path = null;
            }

            if (path == null)
            {
                while (true)
                {
                    Console.WriteLine("Choose the input file.");
                    Console.Write("> ");
                    path = Console.ReadLine();

                    keepLooping = !File.Exists(path) || (importer != null && !importer.ValidatePath(path));

                    if (keepLooping)
                    {
                        Console.WriteLine("That is not a valid file.");
                        Console.WriteLine();
                    } else
                        break;
                }

                if (path == null)
                    return;
            }

            // Static utilty function ftw!!!
            bool result = MetaReader.Import(path, out MetaFile file, importer);

            if (result)
            {
                Console.WriteLine("Success!");
                Console.WriteLine();

                Console.Write("Name: ");
                Console.WriteLine(file.Name);

                if (file.Title != null)
                {
                    Console.Write("Title: ");
                    Console.WriteLine(file.Title);
                }

                if (file.Artist != null)
                {
                    Console.Write("Artist(s): ");
                    Console.WriteLine(file.Artist);
                }

                if (file.Album != null)
                {
                    Console.Write("Album: ");
                    Console.WriteLine(file.Album);
                }
                
                Console.Write("Tempos: [");
                Console.Write(string.Join<TimedValue<Tempo>>(", ", file.Tempos));
                Console.WriteLine(']');

                Console.Write("Time Signatures: [");
                Console.Write(string.Join<TimedValue<TimeSignature>>(", ", file.TimeSignatures));
                Console.WriteLine(']');

                Console.Write("Key Signatures: [");
                Console.Write(string.Join<TimedValue<KeySignature>>(", ", file.KeySignatures));
                Console.WriteLine(']');
            } else
            {
                Console.WriteLine("Failed.");
                return;
            }

            if (rpath != null)
            {
                MetaWriter.Export(rpath, file);
            }
        }
    }
}

#endif