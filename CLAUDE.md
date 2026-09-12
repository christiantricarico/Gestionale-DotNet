# GDN - Gestionale DotNet

Progetto pilota .NET per sperimentare tecnologie recenti modellando processi gestionali (fatturazione, note di credito, interventi, clienti, prodotti/servizi). Non è un prodotto commerciale: è un progetto dove si sperimenta, si impara e ci si diverte. Dettagli di dominio in [README.md](README.md).

L'utente sviluppa con **Visual Studio 2026**; il progetto di avvio (startup project) è **`Gdn.AppHost`** (Aspire), che avvia insieme API e Blazor — mantenere questo comportamento anche quando si eseguono le cose da riga di comando.

## Stack e architettura

- .NET 10 / C# 13, `.slnx` come formato solution (`Gestionale DotNet.slnx`)
- **.NET Aspire** per l'orchestrazione locale: `src/Gdn.AppHost` avvia sia `Gdn.Web.Api.Vs` che `Gdn.Web.MudBlazor`
- **EF Core 10 + SQL Server** in `src/Gdn.Persistence` (migrations, repository, entity type configurations)
- **ASP.NET Core Minimal API** in `src/Gdn.Web.Api.Vs` con **Vertical Slice Architecture**: ogni funzionalità è un file sotto `Features/<Area>/<Azione>.cs` (es. `Features/Invoices/CreateInvoice.cs`) che contiene endpoint, request/response, validazione ed eventuale handler. Non separare in layer orizzontali (controller/service/repository) dentro una feature: la vertical slice va mantenuta.
- **Blazor WebAssembly + MudBlazor** in `src/Gdn.Web.MudBlazor`
- `src/Gdn.Domain` = Core (entità, interfacce repository); `src/Gdn.ServiceDefaults` = defaults Aspire (telemetry, health checks, resilience)

Progetti nella solution (tutti quelli sotto `src/`, tutti referenziati dalla `.slnx`):
`Gdn.Domain`, `Gdn.Persistence`, `Gdn.Web.Api.Vs`, `Gdn.Web.MudBlazor`, `Gdn.AppHost`, `Gdn.ServiceDefaults`.

(Le vecchie cartelle orfane `Gdn.Application`, `Gdn.Application.UnitTests`, `Gdn.Presentation.Shared`, `Gdn.Web.Fluentblazor` — residui net8/net9 senza `.csproj` attivo — sono state rimosse.)

**Non esiste attualmente nessun progetto di test** nella solution. Se si aggiungono test, creare un progetto xUnit (v3) coerente con lo stack (vedi sezione Testing).

## Build e test

```bash
dotnet build "Gestionale DotNet.slnx"
```

Per eseguire l'app in locale (equivalente a "Avvia" in Visual Studio con `Gdn.AppHost` come startup project):

```bash
dotnet run --project src/Gdn.AppHost
```

La connection string SQL Server per lo sviluppo locale è già configurata in `src/Gdn.Web.Api.Vs/appsettings.Development.json` (SQL Express locale, Trusted Connection) — non serve chiederla all'utente, ma non stamparla né copiarla in altri file. `appsettings.json` (senza `.Development`) contiene solo placeholder ed è quello usato in produzione/deploy: non inserire mai credenziali reali lì.

**CORS locale:** l'API legge le origini ammesse da `AppSettings.AllowedOrigins` (`Program.cs`) e applica la policy con `app.UseCors()`. Quando Blazor viene avviato dall'AppHost usa la porta del profilo `http` di `Gdn.Web.MudBlazor/Properties/launchSettings.json` (`http://localhost:5149`): questa porta deve essere presente in `AllowedOrigins` di `appsettings.Development.json`, altrimenti le chiamate della dashboard falliscono con errore CORS. Se si cambia porta/profilo di lancio del Blazor, aggiornare di conseguenza `AllowedOrigins`.

Se in futuro esiste un progetto di test, eseguire con `dotnet test`, usando `--filter` per test mirati e controllando il conteggio dei test eseguiti nei log invece di fidarsi solo dell'exit code.

## Stile del codice

- Applicare lo stile definito in `.editorconfig`.
- C# moderno: primary constructor, file-scoped namespace, using single-line, collection expression, pattern matching e switch expression dove applicabile, `is`/`is not` invece di `as` + null check, `nameof` invece di stringhe letterali, `?.` dove applicabile, `ArgumentNullException.ThrowIfNull`, `ObjectDisposedException.ThrowIf`.
- Newline prima della graffa di apertura di blocchi (`if`, `for`, `while`, `foreach`, `using`, `try`, ecc.).
- L'ultimo `return` di un metodo va sulla propria riga.
- Nullable reference types abilitato ovunque: dichiarare variabili non-nullable, controllare null solo ai punti di ingresso, usare `is null`/`is not null`, fidarsi delle annotazioni del compilatore senza aggiungere controlli ridondanti.
- Commenti solo quando il *perché* non è ovvio dal codice; niente commenti che spiegano il *cosa*.
- Se si aggiungono nuovi file `.cs`, assicurarsi che siano inclusi correttamente nel `.csproj` quando il progetto usa un'inclusione esplicita dei file.
- Non modificare `global.json`, `NuGet.config` o i file `package.json`/`package-lock.json` a meno che non venga esplicitamente richiesto.

### Async

- Suffisso `Async` per i metodi asincroni, che ritornano `Task`/`ValueTask`.
- Propagare `CancellationToken` dove sensato.
- Evitare `async void` (eccetto handler di eventi).

### Error handling

- Usare tipi di eccezione appropriati con messaggi utili; non catturare eccezioni senza rilanciarle (o gestirle in modo esplicito).
- Nella vertical slice, gli errori applicativi (validazione, not found, conflitti) sono modellati con il tipo `Error`/`ResultHelper` già presente in `Gdn.Web.Api.Vs` (non lanciare eccezioni per flussi di controllo previsti).

### UI (MudBlazor)

- Usare sempre i componenti MudBlazor invece di HTML puro quando esiste un componente adatto (`MudButton`, `MudTextField`, `MudTable`, `MudGrid`, `MudCard`, ecc.).
- Layout responsive: `MudGrid`/`MudItem` con breakpoint `xs`/`sm`/`md`/`lg`; `MudHidden` per mostrare/nascondere in base al breakpoint; preferire `Width="100%"`/`FullWidth="true"` a larghezze fisse in pixel.

### Testing (quando si aggiungono test)

- xUnit **v3**, mocking con **NSubstitute**.
- Preferire aggiungere test a file esistenti piuttosto che crearne di nuovi, se un file pertinente esiste già.
- Niente commenti "Act"/"Arrange"/"Assert".
- Non lasciare test commentati o disabilitati che prima non lo erano.

### Documentazione

- XML doc su tutte le API pubbliche, spiegando *scopo e perché*, non ciò che è ovvio; `<see langword="null|true|false"/>` per le keyword; `/// <inheritdoc />` sui membri che sovrascrivono la classe base.

## CI/CD

Push su `dev` triggera [.github/workflows/deploy-to-azure.yml](.github/workflows/deploy-to-azure.yml): build+publish di `Gdn.Web.Api.Vs` su Azure Web App, poi build+publish di `Gdn.Web.MudBlazor` su Azure Static Web Apps (con sostituzione dinamica del `BackendUrl` in `wwwroot/appsettings.json`).
