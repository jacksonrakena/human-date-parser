# 🖍 Human-readable date parsing for .NET
A .NET date parser that can parse human-readable relative and absolute dates to `System.DateTime` instances. This is a completely rewritten version of [ti-ka/human-date-parser](https://github.com/ti-ka/human-date-parser).

### ✍ Usage
(C#)  
Assuming current time is October 23rd, 2019, 9:39pm (23/10/2019, 2139h)...
```csharp
HumanDateParser.Parse("1 month ago") // 23/09/2019, 2139h
HumanDateParser.Parse("In 15 days") // 07/11/2019, 2139h
HumanDateParser.Parse("In 4h3m2s") // 24/09/2019, 0142h (1:42:02 AM)
HumanDateParser.Parse("Tomorrow") // 24/09/2019, 2139h
HumanDateParser.Parse("Tomorrow at 4pm") // 24/09/2019, 1600h (4PM)
HumanDateParser.Parse("Yesterday") // 22/09/2019, 2139h
HumanDateParser.Parse("24h") // 24/09/2019, 2139h
HumanDateParser.Parse("Last thursday at 3:23:44 AM") // 17/10/2019, 0323h (03:23:44 AM)
HumanDateParser.Parse("Last year") // 23/09/2018, 2139h
HumanDateParser.Parse("now") // 23/10/2019, 2139h
HumanDateParser.Parse("") // 23/10/2019, 2139h
```
More functionality is being implemented over time.  
You can add the package to your project from [NuGet under Abyssal.HumanDateParser](https://www.nuget.org/packages/Abyssal.HumanDateParser). It depends on .NET Standard 2.1.

### Contributing
Feel free to contribute.

### Copyright
Copyright (c) 2019 Abyssal 💛
