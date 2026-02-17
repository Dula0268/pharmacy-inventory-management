# Pharmacy Inventory Management (Pharmly)

![Pharmly](docs/assets/pharmly-logo.png "Pharmly")

Compact, modern WPF application for managing pharmacy inventory and sales. Built with .NET 8, Entity Framework Core and SQLite.

---

## Table of contents
- [Features](#features)
- [Getting started](#getting-started)
- [Development](#development)
- [Database & Migrations](#database--migrations)
- [Packaging & Delivery](#packaging--delivery)
- [Recommended Git workflow](#recommended-git-workflow)
- [Contributing](#contributing)
- [License](#license)

---

## Features
- Role-based login (Admin / Cashier)
- Inventory management (products, expirations)
- Sales and receipts (cashier workflow)
- Built-in reporting and exports
- Modern theming via `Themes/PharmlyTheme.xaml`

## Getting started

Prerequisites

- Windows 10 or later
- [.NET 8 SDK](https://dotnet.microsoft.com/)
- (Optional) Visual Studio 2022/2023 for debugging and designer support

Clone and open the solution or run from the command line:

```powershell
git clone <your-repo-url>
cd "PharmacyInventory"
dotnet build
dotnet run --project PharmacyInventory.csproj
```

Or open `PharmacyInventory.sln` in Visual Studio and run the project.

## Development

- The application follows MVVM. ViewModels are under `ViewModels/`, views under `Views/` and services under `Services/`.
- UI styling is in `Themes/PharmlyTheme.xaml`.
- Use the `Host` configuration in `App.xaml.cs` to register services and the DbContext.

Useful commands

```powershell
# build
dotnet build
# run
dotnet run --project PharmacyInventory.csproj
# run tests (if added later)
dotnet test
```

## Database & Migrations

- The app uses SQLite. At runtime it expects a `pharmacy.db` file in the app working directory.
- `Migrations/` contains EF migrations; the app applies pending migrations on startup.
- The DB file is excluded from source control. Do not commit `pharmacy.db`.

To create or update migrations locally:

```powershell
# create a new migration
dotnet ef migrations add <MigrationName> --project PharmacyInventory.csproj
# apply migrations
dotnet ef database update --project PharmacyInventory.csproj
```

## Packaging & Client Delivery

Use the included `PrepareClientDelivery.ps1` to produce a publishable delivery folder without the database file.

```powershell
# run the packaging script from project root
./PrepareClientDelivery.ps1
```

The script produces `PharmacyInventory_ClientDelivery` on your Desktop (see script for options such as runtime identifier and single-file publish).

## Recommended Git workflow

- Keep commits small and focused. Suggested logical groups:
  - `ui: add theme and views`
  - `data: models, DbContext, migrations`
  - `feat(services): business logic services`
  - `feat(viewmodels): screens and bindings`
  - `chore: scripts, tooling`

Set the remote and push to `main` (repository default branch):

```powershell
git remote add origin https://github.com/Dula0268/pharmacy-inventory-management.git
git branch -M main
git push -u origin main
```

Replace the URL above with your repository URL.

## Contributing

- Open issues for bugs and feature requests.
- Send pull requests against the `main` branch.
- Include descriptive commit messages and follow the commit grouping above.

## License

Add a `LICENSE` file at the repository root to declare the project's license. If none is present, contact the project owner for clarification.

---

For questions or help, open an issue on the repository or contact the maintainer.
