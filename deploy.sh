#!/bin/bash

# AgileBoard Deployment Script
set -e

echo "Starting AgileBoard deployment..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "Docker is not running. Please start Docker first."
    exit 1
fi

# Load environment variables if .env file exists
if [ -f .env ]; then
    echo "Loading environment variables from .env file..."
    export $(cat .env | xargs)
fi

# Build and start services
echo "Building and starting services..."
docker-compose down
docker-compose build --no-cache
docker-compose up -d

# Wait for services to be healthy
echo "Waiting for services to be healthy..."
sleep 30

# Check if API is responding
echo "Checking API health..."
for i in {1..10}; do
    if curl -f http://localhost:${API_PORT:-8080}/health > /dev/null 2>&1; then
        echo "API is healthy!"
        break
    else
        echo "Waiting for API... (attempt $i/10)"
        sleep 10
    fi
done

echo "Deployment completed!"
echo "API Documentation: http://localhost:${API_PORT:-8080}/swagger"
echo "Health Check: http://localhost:${API_PORT:-8080}/health"