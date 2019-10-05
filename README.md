# chronicler
Chronicle parsing library for Crusader Kings II

## How to use
1. Convert a CK2 save to JSON, using a utility like [ck2json](https://github.com/scorpdx/ck2json)
2. Load the JSON save using [JsonDocument](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument?view=netcore-3.0).Parse or .ParseAsync
3. Parse the JsonDocument with Chronicler
```csharp
var chronicleCollection = Chronicler.ChronicleCollection.ParseJson(jsonDocument);
```
4. View individual Chronicles, Chronicle Chapters, and Chapter Entries using the Chronicler POCOs
```csharp
var chronicle = chronicleCollection.Chronicles.First();
```
```csharp
{Chronicler.Chronicle}
		Chapters	Count = 6	System.Collections.Generic.List<Chronicler.ChronicleChapter>
		Character	6392	int
```


```csharp
var chapter = chronicle.Chapters.First();
```
```csharp
{Chronicler.ChronicleChapter}
		Entries	Count = 5	System.Collections.Generic.List<Chronicler.ChronicleEntry>
		Year	769	int
```


```csharp
var entry = chapter.Entries.Last();
```
```csharp
{Chronicler.ChronicleEntry}
		Picture	null	string
		Portrait	194170	int
		PortraitCulture	"old_frankish"	string
		PortraitGovernment	"feudal_government"	string
		PortraitTitleTier	3	int
		Text	"West Francia was attacked by the Frankish realm of West-Frisia, ruled by Duke Anselm."	string
```

