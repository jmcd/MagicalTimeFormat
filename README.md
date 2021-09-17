Inspired by Go's [Time.Format](https://pkg.go.dev/time#Time.Format) [Layout](https://pkg.go.dev/time#Layout), this package allows you to produce a [custom .NET date/time format](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) by providing the layout of a reference date formatted example.

```
var formatFactory = new FormatFactory();

var dateFormat = formatFactory.FormatByParsing("January 02, 2006"); // Produces "MMMM dd, yyyy"
```

By default, the factory uses the current culture, and the reference date/time

```
Mon Jan 2 15:04:05 -07:00 2006
```

You can use specific cultures ([`IFormatProvider`](https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider?view=net-5.0)s), and another reference date. You can also specify that you only care about formatting date components or time components.

```
var frenchCulture = CultureInfo.GetCultureInfo("fr");
var formatFactory = new FormatFactory(new DateTime(2006, 1, 2), ReferenceKind.DateOnly, frenchCulture);

var format = formatFactory.FormatByParsing("janvier '06"); // Produces "MMMM 'yy"
```
