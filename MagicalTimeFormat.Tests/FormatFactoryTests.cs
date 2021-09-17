namespace MagicalTimeFormat.Tests
{
    using System;
    using System.Globalization;
    using Shouldly;
    using Xunit;

    public class FormatFactoryTests
    {
        private static readonly DateTimeOffset AmbiguousReference = new DateTimeOffset(2006, 1, 2, 1, 2, 2, new TimeSpan(0, -1, -2, 0));

        private readonly CultureInfo enGbCulture = CultureInfo.GetCultureInfo("en-GB");

        [Theory]
        [InlineData("Mon Jan 2 15:04:05 -07:00 2006", "ddd MMM d HH:mm:ss zzz yyyy")]
        [InlineData("2006-01-02", "yyyy-MM-dd")]
        [InlineData("20060102", "yyyyMMdd")]
        [InlineData("January 02, 2006", "MMMM dd, yyyy")]
        [InlineData("02 January 2006", "dd MMMM yyyy")]
        [InlineData("02-Jan-2006", "dd-MMM-yyyy")]
        [InlineData("01/02/06", "MM/dd/yy")]
        [InlineData("01/02/2006", "MM/dd/yyyy")]
        [InlineData("010206", "MMddyy")]
        [InlineData("Jan-02-06", "MMM-dd-yy")]
        [InlineData("Jan-02-2006", "MMM-dd-yyyy")]
        [InlineData("06", "yy")]
        [InlineData("Mon", "ddd")]
        [InlineData("Monday", "dddd")]
        [InlineData("Jan-06", "MMM-yy")]
        [InlineData("15:04", "HH:mm")]
        [InlineData("15:04:05", "HH:mm:ss")]
        [InlineData("3:04 pm", "h:mm tt")]
        [InlineData("03:04:05 pm", "hh:mm:ss tt")]
        [InlineData("2006-01-02T15:04:05", "yyyy-MM-ddTHH:mm:ss")]
        [InlineData("2006-01-02T15:04:05-07", "yyyy-MM-ddTHH:mm:sszz")]
        [InlineData("2 Jan 2006 15:04:05", "d MMM yyyy HH:mm:ss")]
        [InlineData("2 Jan 2006 15:04", "d MMM yyyy HH:mm")]
        [InlineData("  foo Bar Mon BAZ, 2 Jan 2006 15:04:05 BUZZ!  ", "\"  foo Bar \"ddd BAZ, d MMM yyyy HH:mm:ss BUZZ!  ")]
        [InlineData(" BUZZ!  ", " BUZZ!  ")]
        [InlineData("MMMM Mon ddd Mon ddd", "\"MMMM \"ddd\" ddd \"ddd\" ddd\"")]
        [InlineData("MMMM", "\"MMMM\"")]
        [InlineData("X", "\"X\"")]
        [InlineData("XXX", "XXX")]
        public void CreatedFormatMatchesExpected(string layout, string expectedFormat)
        {
            var actualFormat = new FormatFactory(null, ReferenceKind.Full, enGbCulture).FormatByParsing(layout);
            actualFormat.ShouldBe(expectedFormat);

            // A sanity check on the expectation value
            var s = FormatFactory.StandardReferenceTime.ToString(actualFormat, enGbCulture);
            s.ShouldBe(layout);
        }

        [Theory]
        [InlineData("2006-01-02T15:04:05-07", "yyyy-MM-ddTHH:mm:sszz")]
        [InlineData("Jan-02-06", "MMM-dd-yy")]
        [InlineData("Jan-06", "MMM-yy")]
        [InlineData("Mon Jan 2 15:04:05 -07:38 2006", "ddd MMM d HH:mm:ss zzz yyyy")]
        public void CreatedFormatMatchesExpectedWhenTimezoneHasMinutes(string layout, string expectedFormat)
        {
            var dateTimeOffset = new DateTimeOffset(2006, 1, 2, 15, 4, 5, new TimeSpan(0, -7, -38, 0));
            var actualFormat = new FormatFactory(dateTimeOffset).FormatByParsing(layout);
            actualFormat.ShouldBe(expectedFormat);
        }

        [Fact]
        public void SingleLetterFormatUsesPercent()
        {
            var actualFormat = new FormatFactory(null, ReferenceKind.Full, enGbCulture).FormatByParsing("1");
            actualFormat.ShouldBe("%M");
        }

        [Fact]
        public void AmbiguousReferenceShouldCauseException()
        {
            Should.Throw<ArgumentException>(() => new FormatFactory(AmbiguousReference, ReferenceKind.Full, enGbCulture));
        }

        [Fact]
        public void CanFormatWithDateOnly()
        {
            var ff = new FormatFactory(new DateTime(2006, 1, 2), ReferenceKind.DateOnly, enGbCulture);
            ff.FormatByParsing("Jan-02-06").ShouldBe("MMM-dd-yy");
        }

        [Fact]
        public void CanFormatWithTimeOnly()
        {
            var ff = new FormatFactory(new DateTime(1900, 1, 1, 13, 5, 6), ReferenceKind.TimeOnly, enGbCulture);
            ff.FormatByParsing("1:05 pm").ShouldBe("h:mm tt");
        }

        [Fact]
        public void Can()
        {
            var frenchCulture = CultureInfo.GetCultureInfo("fr");
            var actualFormat = new FormatFactory(null, ReferenceKind.Full, frenchCulture).FormatByParsing("lundi janvier");
            actualFormat.ShouldBe("dddd MMMM");
        }
    }
}