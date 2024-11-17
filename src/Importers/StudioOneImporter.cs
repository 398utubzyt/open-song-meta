// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Opsm.IO;

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace Opsm.Importers
{
    public sealed class StudioOneImporter : IMetaImporter
    {
        public string ImporterName => "Studio One";
        public bool IsPopulated => _populated;

        private bool _populated;

#nullable disable
        private string _name;
#nullable restore
        private string? _title;
        private string? _artist;
        private string? _album;
#nullable disable
        private TimedTrack<Tempo> _tempos;
        private TimedTrack<TimeSignature> _timeSignatures;
        private TimedTrack<KeySignature> _keySignatures;
#nullable restore

        public void Dispose()
        {
        }

        private static readonly XmlReaderSettings _xmlSettings = new() { CloseInput = true };
        private static readonly XmlParserContext _xmlContext = new(null, new XmlNamespaceManager(new NameTable()), null, XmlSpace.None);

        static StudioOneImporter()
        {
            _xmlContext.NamespaceManager!.AddNamespace("x", string.Empty);
        }

        public bool ValidatePath(string path)
            => path.EndsWith(".song", System.StringComparison.Ordinal);

        public bool Populate(MetaReader reader)
        {
            ZipArchive zip = new(reader.Stream, ZipArchiveMode.Read, true);

            using (zip)
            {
                ZipArchiveEntry? e = zip.GetEntry("metainfo.xml");
                if (e == null)
                {
                    return false;
                }

                Stream xmlInfo = e.Open();
                using (xmlInfo)
                {
                    XmlReader xr;
                    using (xr = XmlReader.Create(xmlInfo, _xmlSettings, _xmlContext))
                    {
                        if (!xr.ReadToFollowing("MetaInformation"))
                            return false;

                        if (xr.ReadToDescendant("Attribute"))
                        {
                            do
                            {
                                string? id = xr.GetAttribute("id");
                                if (id == null)
                                    return false;

                                string? value = xr.GetAttribute("value");
                                if (value == null)
                                    return false;

                                switch (id)
                                {
                                    case "Media:Artist":
                                        _artist = string.IsNullOrWhiteSpace(value) ? null : value;
                                        break;
                                    case "Document:Title":
                                        _name = value;
                                        break;
                                    case "Media:Title":
                                        _title = string.IsNullOrWhiteSpace(value) ? null : value;
                                        break;
                                    case "Media:Album":
                                        _album = string.IsNullOrWhiteSpace(value) ? null : value;
                                        break;
                                }
                            } while (xr.ReadToNextSibling("Attribute"));
                        }
                    }
                }

                e = zip.GetEntry("Song/song.xml");
                if (e == null)
                {
                    return false;
                }

                using (xmlInfo = e.Open())
                {
                    XmlReader xr;
                    using (xr = XmlReader.Create(xmlInfo, _xmlSettings, _xmlContext))
                    {
                        if (!xr.ReadToFollowing("Song"))
                            return false;
                        if (!xr.ReadToDescendant("Attributes"))
                            return false;
                        if (!xr.ReadToDescendant("Attributes"))
                            return false;

                        xr.ReadStartElement();

                        while (xr.Read() && xr.NodeType == XmlNodeType.Element)
                        {
                            switch (xr.Name)
                            {
                                case "TempoMap":
                                    {
                                        List<TimedValue<Tempo>> list = [];
                                        if (xr.ReadToDescendant("TempoMapSegment"))
                                        {
                                            do
                                            {
                                                list.Add(new(double.Parse(xr.GetAttribute("start")!), new(60.0 / double.Parse(xr.GetAttribute("tempo")!))));
                                            } while (xr.ReadToNextSibling("TempoMapSegment"));
                                        }
                                        xr.ReadEndElement();
                                        _tempos = new(list);
                                    } break;

                                case "TimeSignatureMap":
                                    {
                                        List<TimedValue<TimeSignature>> list = [];
                                        if (xr.ReadToDescendant("TimeSignatureMapSegment"))
                                        {
                                            do
                                            {
                                                list.Add(new(double.Parse(xr.GetAttribute("start")!),
                                                    new(short.Parse(xr.GetAttribute("numerator")!),
                                                        short.Parse(xr.GetAttribute("denominator")!))));
                                            } while (xr.ReadToNextSibling("TempoMapSegment"));
                                        }
                                        xr.ReadEndElement();
                                        _timeSignatures = new(list);
                                    }
                                    break;

                                case "KeySignatureMap":
                                    {
                                        List<TimedValue<KeySignature>> list = [];
                                        if (xr.ReadToDescendant("Attributes"))
                                        {
                                            do
                                            {
                                                WesternMode mode = WesternUtils.ParseMode(xr.GetAttribute("scale")!);
                                                list.Add(new(double.Parse(xr.GetAttribute("start")!),
                                                    new(mode != WesternMode.Unknown ?
                                                            WesternUtils.FromCircleOfFifths(int.Parse(xr.GetAttribute("root")!)) :
                                                            WesternNote.Unknown,
                                                        mode)));
                                            } while (xr.ReadToNextSibling("Attributes"));
                                        }
                                        xr.ReadEndElement();
                                        _keySignatures = new(list);
                                    } break;

                                default:
                                    xr.Skip();
                                    break;
                            }
                        }
                    }
                }
            }

            _populated = true;

            return true;
        }

        public bool Import(out MetaFile meta)
        {
            bool result = _populated;
            meta = result ? new(MetaWriter.CurrentVersion, _name, _title, _artist, _album, _tempos ?? [], _timeSignatures ?? [], _keySignatures ?? []) : default;
            return result;
        }
    }
}
