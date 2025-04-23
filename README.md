# GDN - Cosa è, cosa non è
GDN è un progetto pilota creato con l'obiettivo di utilizzare le ultime tecnologie del mondo .NET per modellare tipici processi gestionali.
Quante volte ci è capitato di dover ridefinire i processi più comuni? 
Con GDN abbiamo la possibilità come community di modellare questi processi ed averli pronti all'uso.

Ma tenete sempre presente che GDN:

- ✅ E' un progetto dove si sperimenta e si apprende.
- ✅ E' un progetto dove si utilizzano le ultime novità, senza paura.
- ✅ E' un progetto dove ci si confronta e si cercano le migliori soluzioni possibili.
- ✅ E' un progetto dove ci si diverte.
- ⛔ Non è un gestionale che vuole competere nel mercato dei gestionali commerciali.

## Funzioni
### Fatturazione
Nelle prime versioni l'obiettivo è quello di costruire un modulo di fatturazione di base che dia la possibilità di:
- Compilare intestazione e corpo fattura con selezione da tabelle di lookup di cliente, unità di misura e aliquota iva.
- Generare un report di stampa fattura in PDF.
- Generare ed esportare il file xml di fatturazione elettronica per la trasmissione al sistema di interscambio.

## Tecnologie e pattern coinvolti
- [.NET Aspire](https://github.com/dotnet/aspire)
- [Entity Framework Core](https://github.com/dotnet/efcore)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-9.0)
- [ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-9.0)
- [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)

## Progetti di supporto

- [FatturaElettronica.NET](https://github.com/FatturaElettronica/FatturaElettronica.NET): per la generazione dei file xml di fatturazione elettronica.
- [QuestPDF](https://github.com/QuestPDF/QuestPDF): per la generazione dei report PDF.
- [TinyHelpers](https://github.com/marcominerva/TinyHelpers): utilities di configurazione per ASP.NET Core.
