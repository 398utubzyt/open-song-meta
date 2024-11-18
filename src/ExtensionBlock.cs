// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Opsm
{
    public readonly struct ExtensionBlock(string name, byte[] data)
    {
        public readonly string Name = name;
        public readonly byte[] Data = data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasName(string name) => string.Equals(Name, name, StringComparison.Ordinal);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(Span<byte> destination)
        {
            Data.CopyTo(destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ref T GetDataAs<T>()
        {
            Debug.Assert(Data != null);
            Debug.Assert(Unsafe.SizeOf<T>() == Data.Length);

            return ref Unsafe.As<byte, T>(ref MemoryMarshal.GetArrayDataReference(Data));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Stream Open()
        {
            Debug.Assert(Data != null);
            return new MemoryStream(Data);
        }
    }
}
