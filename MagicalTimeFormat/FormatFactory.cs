namespace MagicalTimeFormat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///     Creates date/time formats by user supplied layout examples.
    /// </summary>
    /// <example>
    ///     <code>
    ///
    /// string myLayout = "Jan-02-2006";
    ///
    /// string format = new FormatFactory().FormatByParsing(myLayout);
    ///
    /// new DateTime(2021, 9, 17).ToString(format);  // "Sep-17-2021"
    /// </code>
    /// </example>
    public class FormatFactory
    {
        // Mon Jan 2 15:04:05 -07:00 2006
        /// <summary>
        ///     The standard reference date/time. Mon Jan 2 15:04:05 -07:00 2006
        /// </summary>
        public static readonly DateTimeOffset StandardReferenceTime = new DateTimeOffset(2006, 1, 2, 15, 4, 5, TimeSpan.FromHours(-7));

        private readonly SpecifierExemplarValues specifierExemplarValues;

        /// <summary>
        ///     Initializes a new instance of the <c>FormatFactory</c> class using the supplied reference time.
        /// </summary>
        /// <param name="referenceTime">The reference to which layouts are parsed against</param>
        /// <param name="kind">
        ///     The kind of value that referenceTime represents, by default a full date-time value. By specifying
        ///     date or time only, an otherwise ambiguous date-time may be supplied.
        /// </param>
        /// <param name="formatProvider">The format provider used to parse date/time. Defaults to the current culture.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when the supplied <see cref="referenceTime" /> does not provide unique
        ///     values for each of the supported specifiers.
        /// </exception>
        /// <see cref="FormatByParsing" />
        /// <see cref="Specifier" />
        public FormatFactory(DateTimeOffset? referenceTime = default, ReferenceKind kind = ReferenceKind.Full, IFormatProvider? formatProvider = default) => specifierExemplarValues = SpecifierExemplarValues.From(referenceTime ?? StandardReferenceTime, kind, formatProvider);

        /// <summary>
        ///     Creates a date/time format by parsing a layout, matching components of the layout with those provided in the
        ///     reference provided int the constructor.
        ///     Supported custom date/time format specifiers are enumerated in <see cref="Specifier" />
        ///     <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings" />
        /// </summary>
        /// <param name="layout">The example layout</param>
        /// <returns>A date/time format</returns>
        public string FormatByParsing(ReadOnlySpan<char> layout)
        {
            var builder = new StringBuilder();

            var remainder = layout;

            while (!remainder.IsEmpty)
            {
                var chunk = NextChunk(remainder);

                var prefix = remainder[chunk.PrefixRange.LowerBound..chunk.PrefixRange.UpperBound].ToString();
                var prefixNeedsQuoted = SpecifierCollections.IsASpecifier(prefix);
                if (prefixNeedsQuoted)
                {
                    builder.Append('"').Append(prefix).Append('"');
                }
                else
                {
                    builder.Append(prefix);
                }

                if (chunk.Specifier.HasValue)
                {
                    builder.Append(chunk.Specifier.ToString());
                }

                remainder = remainder[chunk.SuffixRange.LowerBound..chunk.SuffixRange.UpperBound];
            }

            if (builder.Length == 1)
            {
                if (SpecifierCollections.IsASpecifier(builder.ToString()))
                {
                    builder.Insert(0, '%');
                }
                else
                {
                    builder.Insert(0, '"').Append('"');
                }
            }

            return builder.ToString();
        }

        private Chunk NextChunk(ReadOnlySpan<char> layout)
        {
            for (var i = 0; i < layout.Length; i++)
            {
                foreach (var ex in specifierExemplarValues.ExemplarOrderedByDecreasingLength)
                {
                    if (!StartsWith(layout[i..], ex))
                    {
                        continue;
                    }

                    var sp = specifierExemplarValues.ExemplarToSpecifier[ex];

                    return new Chunk(new Chunk.Range(0, i), sp, new Chunk.Range(i + ex.Length, layout.Length));
                }
            }

            return new Chunk(new Chunk.Range(0, layout.Length), null, new Chunk.Range(layout.Length, layout.Length));
        }

        private static bool StartsWith(ReadOnlySpan<char> s, ReadOnlySpan<char> target)
        {
            if (s.Length < target.Length)
            {
                return false;
            }

            for (var i = 0; i < target.Length; i++)
            {
                if (s[i] != target[i])
                {
                    return false;
                }
            }

            return true;
        }

        private readonly struct Chunk
        {
            public readonly Range PrefixRange;
            public readonly Specifier? Specifier;
            public readonly Range SuffixRange;

            public Chunk(Range prefixRange, Specifier? specifier, Range suffixRange)
            {
                PrefixRange = prefixRange;
                Specifier = specifier;
                SuffixRange = suffixRange;
            }

            public readonly struct Range
            {
                public readonly int LowerBound;
                public readonly int UpperBound;

                public Range(int lowerBound, int upperBound)
                {
                    LowerBound = lowerBound;
                    UpperBound = upperBound;
                }
            }
        }
    }
}