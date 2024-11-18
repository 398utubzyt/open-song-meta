// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Opsm
{
    public sealed class TimedTrack<T> : IEnumerable<TimedValue<T>>, IEnumerable<T>
    {
        private readonly TimedValue<T>[] _track;
        private int _count;

        public TimedValue<T> First => _track.Length > 0 ? _track[0] : throw new InvalidOperationException();
        public T FirstValue => _track.Length > 0 ? _track[0].Value : throw new InvalidOperationException();

        public int Length => _track.Length;

        public ref TimedValue<T> this[int index] => ref _track[index];

        private static bool TryGetIndex(TimedValue<T>[] track, double time, out int index)
        {
            bool result = false;
            int i = 0;

            while (i < track.Length)
            {
                if (time > track[i].Time)
                {
                    result = i > 0;
                    break;
                }

                ++i;
            }

            index = --i;
            return result;
        }

        private static bool TryGetValue(TimedValue<T>[] track, double time, [MaybeNullWhen(false)] out T value)
        {
            bool result = TryGetIndex(track, time, out int index);
            value = result ? track[index].Value : default;
            return result;
        }

        public readonly struct ReadOnly(TimedTrack<T> track) : IEnumerable<TimedValue<T>>, IEnumerable<T>
        {
            private readonly TimedValue<T>[] _track = track._track;
            public readonly int Length => _track.Length;

            public readonly TimedValue<T> First => _track.Length > 0 ? _track[0] : throw new InvalidOperationException();
            public readonly T FirstValue => _track.Length > 0 ? _track[0].Value : throw new InvalidOperationException();

            public readonly ref readonly TimedValue<T> this[int index] => ref _track[index];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool TryGetIndex(double time, out int index) => TimedTrack<T>.TryGetIndex(_track, time, out index);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool TryGetValue(double time, [MaybeNullWhen(false)] out T value) => TimedTrack<T>.TryGetValue(_track, time, out value);

            public IEnumerator<TimedValue<T>> GetEnumerator() => new Enumerator(_track);
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ValueEnumerator(_track);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetIndex(double time, out int index) => TimedTrack<T>.TryGetIndex(_track, time, out index);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(double time, [MaybeNullWhen(false)] out T value) => TimedTrack<T>.TryGetValue(_track, time, out value);

        public Span<TimedValue<T>> AsSpan() => _track.AsSpan();
        public ReadOnly AsReadOnly() => new(this);

        private static void ThrowIfOverflow(int count, int amount, int length)
        {
            amount += count;
            if (length < amount)
                throw new InvalidOperationException();
        }

        public void Append(double time, T value)
        {
            ThrowIfOverflow(_count, 1, _track.Length);
#if DEBUG
            if (_count > 0)
            {
                System.Diagnostics.Debug.Assert(_track[_count - 1].Time <= time);
            }
#endif
            Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_track), _count++) = new TimedValue<T>(time, value);
        }

        public void Append(TimedValue<T> value)
        {
            ThrowIfOverflow(_count, 1, _track.Length);
#if DEBUG
            if (_count > 0)
            {
                System.Diagnostics.Debug.Assert(_track[_count - 1].Time <= value.Time);
            }
#endif
            Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_track), _count++) = value;
        }

        public void Append(IEnumerable<TimedValue<T>> values)
        {
            ThrowIfOverflow(_count, values.Count(), _track.Length);
            ref TimedValue<T> r = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_track), _count);
            foreach (TimedValue<T> value in values)
            {
                r = value;
                Unsafe.Add(ref r, 1);
#if DEBUG
                if (_count > 0)
                {
                    System.Diagnostics.Debug.Assert(_track[_count - 1].Time <= value.Time);
                }
#endif
                _count++;
            }
        }

        public void Append(ICollection<TimedValue<T>> values)
        {
            ThrowIfOverflow(_count, values.Count, _track.Length);
            values.CopyTo(_track, _count);
            _count += values.Count;
#if DEBUG
            int i = _count - values.Count;
            if (i > 0)
                System.Diagnostics.Debug.Assert(_track[i - 1].Time <= _track[i].Time);
            for (++i; i < _count; ++i)
            {
                System.Diagnostics.Debug.Assert(_track[i - 1].Time <= _track[i].Time);
            }
#endif
        }



        private struct Enumerator(TimedValue<T>[] track) : IEnumerator<TimedValue<T>>
        {
            private readonly TimedValue<T>[] _track = track;
            private int _index = -1;

            public readonly TimedValue<T> Current => _track[_index];
            readonly object? IEnumerator.Current => Current;

            public readonly void Dispose()
            {
            }

            public bool MoveNext()
            {
                int newIndex = _index + 1;
                bool result = newIndex < _track.Length;
                if (result)
                    _index = newIndex;
                return result;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        private struct ValueEnumerator(TimedValue<T>[] track) : IEnumerator<T>
        {
            private readonly TimedValue<T>[] _track = track;
            private int _index = -1;

            public readonly T Current => _track[_index].Value;
            readonly object? IEnumerator.Current => Current;

            public readonly void Dispose()
            {
            }

            public bool MoveNext()
            {
                int newIndex = _index + 1;
                bool result = newIndex < _track.Length;
                if (result)
                    _index = newIndex;
                return result;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        public IEnumerator<TimedValue<T>> GetEnumerator() => new Enumerator(_track);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ValueEnumerator(_track);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private TimedTrack()
        {
            _track = [];
            _count = 0;
        }

        private static class EmptyTrack
        {
            public static readonly TimedTrack<T> Value = new();
        }

        public static TimedTrack<T> Empty => EmptyTrack.Value;

        public TimedTrack(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity, nameof(capacity));
            _track = new TimedValue<T>[capacity];
            _count = 0;
        }

        public TimedTrack(IEnumerable<TimedValue<T>> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            _track = new TimedValue<T>[items.Count()];
            _count = 0;

            Append(items);
        }

        public TimedTrack(ICollection<TimedValue<T>> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            _count = items.Count;
            _track = new TimedValue<T>[_count];
            items.CopyTo(_track, 0);
        }

        public TimedTrack(params TimedValue<T>[] items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            _count = items.Length;
            _track = new TimedValue<T>[_count];
            Array.Copy(items, 0, _track, 0, _count);
        }
    }
}
