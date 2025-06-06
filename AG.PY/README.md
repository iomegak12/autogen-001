# agentic-ai

## Overview

agentic-ai is a Python project template designed for rapid prototyping and deployment. It features a clean project structure, Docker support, environment configuration, and a simple welcome message to get you started quickly.

## Features
- MIT License
- Python 3.x
- Dependency management with [uv](https://github.com/astral-sh/uv)
- Docker and docker-compose support
- GitHub Actions workflow for Docker image builds
- Environment configuration via `.env`
- Organized folders for logs, data, uploads, docs, and source code
- Welcome message on startup

## Project Structure
```
agentic-ai/
├── LICENSE
├── README.md
├── .gitignore
├── .env
├── requirements.txt
├── Dockerfile
├── docker-compose.yml
├── .github/
│   └── workflows/
│       └── docker-image.yml
├── logs/
├── data/
├── uploads/
├── docs/
└── src/
    └── main.py
```

## Getting Started

### Prerequisites
- [Python 3.8+](https://www.python.org/downloads/)
- [uv](https://github.com/astral-sh/uv) (for dependency management)
- [Docker](https://www.docker.com/)

### Setup
1. **Clone the repository**
2. **Install dependencies and create a virtual environment using uv:**
   ```sh
   uv venv .venv
   uv pip install -r requirements.txt
   ```
3. **Copy `.env.example` to `.env` and update as needed**
4. **Run the application:**
   ```sh
   uv pip run src/main.py
   ```

### Using Docker
- **Build the image:**
  ```sh
  docker build -t agentic-ai .
  ```
- **Run with docker-compose:**
  ```sh
  docker-compose up
  ```

## Configuration
- All secrets and configuration should be placed in the `.env` file.

## License
MIT
