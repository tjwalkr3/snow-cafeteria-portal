echo "Running docker compose down and rebuild..."

# Run docker compose commands
docker compose down -v && docker compose up --build
