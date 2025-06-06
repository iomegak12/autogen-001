# Use official Python image as base
FROM python:3.11-slim

# Set work directory
WORKDIR /app

# Install uv
RUN pip install uv

# Copy requirements and install dependencies
COPY requirements.txt ./
RUN uv pip install -r requirements.txt

# Copy project files
COPY . .

# Expose port (if needed)
EXPOSE 8000

# Set environment variables
ENV PYTHONUNBUFFERED=1

# Run the application
CMD ["uv", "pip", "run", "src/main.py"]
