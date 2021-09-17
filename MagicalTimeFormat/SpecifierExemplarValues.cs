namespace MagicalTimeFormat
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    internal class SpecifierExemplarValues
    {
        private static readonly IDictionary<ReferenceKind, Dictionary<Specifier, string>> ReferenceKindToSpecifierToString =
            Enum.GetValues(typeof(ReferenceKind)).Cast<ReferenceKind>().ToDictionary(rk => rk, rk =>
                SpecifierCollections.ByKind(rk)
                    .ToDictionary(sp => sp, sp => sp.ToString()));

        private SpecifierExemplarValues(IDictionary<string, Specifier> exemplarToSpecifier)
        {
            ExemplarToSpecifier = exemplarToSpecifier.ToImmutableDictionary();
            ExemplarOrderedByDecreasingLength = ExemplarToSpecifier.Keys.OrderByDescending(ex => ex.Length).ToImmutableList();
            Specifiers =  ExemplarToSpecifier.Select(kvp=>kvp.Value).ToImmutableHashSet();
        }

        internal ImmutableHashSet<Specifier> Specifiers { get; }

        internal ImmutableList<string> ExemplarOrderedByDecreasingLength { get; }

        internal ImmutableDictionary<string, Specifier> ExemplarToSpecifier { get; }

        internal static SpecifierExemplarValues From(DateTimeOffset reference, ReferenceKind kind, IFormatProvider? formatProvider)
        {
            var specifierStrings = ReferenceKindToSpecifierToString[kind];

            var ex2Sp = new Dictionary<string, Specifier>();

            foreach (var (specifier, specifierString) in specifierStrings)
            {
                var sp = specifierString.Length == 1 ? $"%{specifierString}" : specifierString;
                var ex = reference.ToString(sp, formatProvider);

                if (ex2Sp.TryGetValue(ex, out var existingSp))
                {
                    var msg = $"Ambiguous reference time - the specifiers \"{existingSp}\" and \"{sp}\" both produce \"{ex}\"";
                    throw new ArgumentException(msg, nameof(reference));
                }

                ex2Sp[ex] = specifier;
            }

            return new SpecifierExemplarValues(ex2Sp);
        }
    }
}