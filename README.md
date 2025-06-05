# TrainingAgent

## Project Description

**TrainingAgent** is a C#/.NET Core-based agent framework designed for extensible, modular, and secure agent development. The project is structured for best practices in maintainability, testability, and deployment, and is ready for containerization and CI/CD workflows.

### Features
- .NET 8.0/9.0 compatible
- Clean separation of main and test projects
- xUnit for unit testing
- Docker and Docker Compose support
- GitHub Actions workflow for Docker image build and push
- MIT License

## Project Structure

```
TrainingAgent.sln
│
├── TrainingAgent/           # Main agent project (C#)
│   └── appsettings.json     # Configuration and secrets (e.g., OPENAI_API_KEY)
│
├── TrainingAgentTests/      # xUnit test project
│
├── Dockerfile               # Docker build file
├── docker-compose.yml        # Docker Compose file
├── .gitignore                # C#/.NET specific ignore rules
├── .github/
│   └── workflows/
│       └── docker-image.yml  # GitHub Actions workflow for Docker
├── LICENSE                   # MIT License
└── README.md                 # Project documentation
```

## Getting Started

### Prerequisites
- .NET 8.0 or 9.0 SDK
- Docker
- (Optional) GitHub account for CI/CD

### Build and Run

```sh
# Build the solution
 dotnet build

# Run the main project
 dotnet run --project TrainingAgent

# Run tests
 dotnet test
```

### Configuration
All configuration and secrets (e.g., `OPENAI_API_KEY`) should be placed in `TrainingAgent/appsettings.json`:

```
{
  "OPENAI_API_KEY": "your-api-key-here"
}
```

**Never commit real secrets to version control. Use environment variables or GitHub secrets for production.**

### Docker

Build and run the container:
```sh
docker build -t trainingagent .
docker run --rm -e OPENAI_API_KEY=your-api-key-here trainingagent
```

Or use Docker Compose:
```sh
docker-compose up --build
```

### GitHub Actions
The workflow in `.github/workflows/docker-image.yml` will build and push the Docker image on push to `main`.

---

## License
MIT
