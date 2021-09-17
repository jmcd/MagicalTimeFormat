namespace MagicalTimeFormat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class SpecifierCollections
    {
        private static readonly Specifier[] Full =
        {
            Specifier.d,
            Specifier.dd,
            Specifier.ddd,
            Specifier.dddd,
            Specifier.g,
            Specifier.h,
            Specifier.hh,
            Specifier.HH,
            Specifier.m,
            Specifier.mm,
            Specifier.M,
            Specifier.MM,
            Specifier.MMM,
            Specifier.MMMM,
            Specifier.s,
            Specifier.ss,
            Specifier.t,
            Specifier.tt,
            Specifier.y,
            Specifier.yy,
            Specifier.yyyy,
            Specifier.z,
            Specifier.zz,
            Specifier.zzz,
        };

        private static readonly Specifier[] DateOnly =
        {
            Specifier.d,
            Specifier.dd,
            Specifier.ddd,
            Specifier.dddd,
            Specifier.g,
            Specifier.M,
            Specifier.MM,
            Specifier.MMM,
            Specifier.MMMM,
            Specifier.y,
            Specifier.yy,
            Specifier.yyyy,
        };

        private static readonly Specifier[] TimeOnly =
        {
            Specifier.h,
            Specifier.hh,
            Specifier.HH,
            Specifier.m,
            Specifier.mm,
            Specifier.s,
            Specifier.ss,
            Specifier.t,
            Specifier.tt,
        };

        private static readonly HashSet<string> AllSpecifierStrings = Enum.GetValues(typeof(Specifier)).Cast<Specifier>().Select(sp => sp.ToString()).ToHashSet();

        internal static IEnumerable<Specifier> ByKind(ReferenceKind kind)
        {
            return kind switch
            {
                ReferenceKind.Full => Full,
                ReferenceKind.DateOnly => DateOnly,
                ReferenceKind.TimeOnly => TimeOnly,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
            };
        }

        internal static bool IsASpecifier(string s)
        {
            return AllSpecifierStrings.Any(sp => s.Contains(sp.ToString()));
        }
    }
}